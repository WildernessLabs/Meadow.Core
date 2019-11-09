using System;
using System.IO;
using static Meadow.Core.Interop;

namespace Meadow.Hardware
{
    public class NixDriverBasedSerialPort : SerialPort
    {
        public NixDriverBasedSerialPort(string portName)
            : base(portName)
        {
        }
    }

    /// <summary>
    /// Represents a port that is capable of serial (UART) communications.
    /// 
    /// NOTE: This class is not yet implemented. 
    /// </summary>
    public class SerialPort : IDisposable
    {
        private IntPtr _driverHandle = IntPtr.Zero;

        public string PortName { get; }
        public bool IsOpen { get => _driverHandle != IntPtr.Zero; }

        public SerialPort(string portName)
        {
            PortName = portName;
        }

        public void Dispose()
        {
            Close();
        }

        public void Open()
        {
            var handle = Nuttx.open($"/dev/{PortName}", Nuttx.DriverFlags.ReadWrite);
            if (handle.ToInt32() < 0)
            {
                // TODO: determine the reason for failure and pass that upstream
                throw new Exception($"Unable to open {PortName}");
            }
            _driverHandle = handle;
        }

        public void Close()
        {
            if (!IsOpen) return;

            Nuttx.close(_driverHandle);
            _driverHandle = IntPtr.Zero;
        }

        public void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (!IsOpen)
            {
                throw new Exception("Cannot write to a closed port");
            }

            if (buffer == null) throw new ArgumentNullException();
            if (count > (buffer.Length - offset)) throw new ArgumentException("Count is larger than available data");
            if (offset < 0) throw new ArgumentException("Invalid offset");
            if (count == 0) return;

            Console.WriteLine($"Writing {count} bytes to {PortName}...");
            if (offset > 0)
            {
                // we need to make a copy
                var tmp = new byte[count];
                Array.Copy(buffer, offset, tmp, 0, count);
                var result = Nuttx.write(_driverHandle, tmp, count);
                Console.WriteLine($"  result: {result}");
            }
            else
            {
                var result = Nuttx.write(_driverHandle, buffer, count);
                Console.WriteLine($"  result: {result}");
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (!IsOpen)
            {
                throw new Exception("Cannot read from a closed port");
            }

            if (buffer == null) throw new ArgumentNullException();
            if (count > (buffer.Length - offset)) throw new ArgumentException("Count is larger than available buffer size");
            if (offset < 0) throw new ArgumentException("Invalid offset");
            if (count == 0) return 0;

            var buf = new byte[count];

            var result = Nuttx.read(_driverHandle, buf, buf.Length);
            Console.WriteLine($"  rx result: {result}");
            if (result < 0)
            {
                // TODO: handle error
                return 0;
            }
            else
            {
                Array.Copy(buf, 0, buffer, offset, result);
            }

            return result;

        }

        //struct termios2 tio;

        //ioctl(fd, TCGETS2, &tio);
        //tio.c_cflag &= ~CBAUD;
        //tio.c_cflag |= BOTHER;
        //tio.c_ispeed = 12345;
        //tio.c_ospeed = 12345;
        //ioctl(fd, TCSETS2, &tio);

        // ////////////////////////////////////////////////////////////////
        /*
        public Stream BaseStream { get; }
        public int BaudRate { get; set; }
        public int BytesToRead { get; }
        public int BytesToWrite { get; }
        public int DataBits { get; set; }
        public FlowControlType Handshake { get; set; }
        public ParityType Parity { get; set; }
        public NumberOfStopBits StopBits { get; set; }

        public override bool CanRead => throw new System.NotImplementedException();

        public override bool CanSeek => throw new System.NotImplementedException();

        public override bool CanWrite => throw new System.NotImplementedException();

        public override long Length => throw new System.NotImplementedException();

        public override long Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override int WriteTimeout { get; set; }

        public delegate void SerialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e);
        public delegate void SerialErrorReceivedEventHandler(object sender, SerialErrorReceivedEventArgs e);

        public event SerialDataReceivedEventHandler DataReceived;
        public event SerialErrorReceivedEventHandler ErrorReceived;

        public SerialPort(string portName, int baudRate) { }
        public SerialPort(string portName, int baudRate, ParityType parity) { }
        public SerialPort(string portName, int baudRate, ParityType parity, int dataBits) { }
        public SerialPort(string portName, int baudRate, ParityType parity, int dataBits, NumberOfStopBits stopBits) { }

        public void DiscardInBuffer() { }
        public void DiscardOutBuffer() { }
        
        public override void Flush()
        {
            throw new System.NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }
        */
    }

}
