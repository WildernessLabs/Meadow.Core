using System;
using System.IO;
using System.Runtime.InteropServices;
using static Meadow.Core.Interop;

namespace Meadow.Hardware
{
    public enum StopBits
    {
        One,
//        OnePointFive,
        Two
    }

    public enum Parity
    {
        None,
        Odd,
        Even,
//        Mark,
//        Space
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
        public int BaudRate { get; }
        public Parity Parity { get; }
        public int DataBits { get; }
        public StopBits StopBits { get; }

        public SerialPort(string portName, int baudRate, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            if (baudRate <= 0) throw new ArgumentOutOfRangeException("Invalid baud rate");
            if (dataBits < 5 || dataBits > 8) throw new ArgumentOutOfRangeException("Invalid dataBits");

            PortName = portName;
            BaudRate = baudRate;
            Parity = Parity;
            DataBits = dataBits;
            StopBits = stopBits;
        }

        public override string ToString()
        {
            char p;
            switch (Parity)
            {
                case Parity.Even:
                    p = 'e';
                    break;
                case Parity.Odd:
                    p = 'o';
                    break;
                default:
                    p = 'n';
                    break;
            }

            return $"{PortName},{DataBits},{p},{(StopBits == StopBits.Two ? 2 : 1)}";
        }

        public void Dispose()
        {
            Close();
        }

        public void Open()
        {
            var handle = Nuttx.open($"/dev/{PortName}", Nuttx.DriverFlags.ReadWrite | Nuttx.DriverFlags.SynchronizeOutput);
            if (handle.ToInt32() < 0)
            {
                var errno = Devices.UPD.GetLastError();
                throw new Exception($"Unable to open {PortName} ({errno.ToString()})");
            }
            _driverHandle = handle;
            SetPortSettings();
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

            if (result < 0)
            {
                // TODO: handle error
                var errno = Devices.UPD.GetLastError();
                Console.WriteLine($"  rx result: {result} errno: {errno}");

                return 0;
            }
            else
            {
                Array.Copy(buf, 0, buffer, offset, result);
            }

            return result;

        }

        private const int TCGETS = 1;
        private const int TCSETS = 2;

        private void ShowSettings(Nuttx.Termios settings)
        {
            Console.WriteLine($"  Speed: {settings.c_speed}");
            Console.WriteLine($"  Input Flags: 0x{settings.c_iflag:x}");
            Console.WriteLine($"  OutputFlags: 0x{settings.c_oflag:x}");
            Console.WriteLine($"  Local Flags: {settings.c_lflag}");
            Console.WriteLine($"  Control Flags: {settings.c_cflag}");
        }

        private void SetPortSettings()
        {
            var settings = new Nuttx.Termios();
            // get the current settings
            var gch = GCHandle.Alloc(settings, GCHandleType.Pinned);

            try
            {
                Console.WriteLine($"  Getting port settings...");
                var result = Nuttx.ioctl(_driverHandle, TCGETS, gch.AddrOfPinnedObject());
                if (result < 0)
                {
                    var errno = Devices.UPD.GetLastError();
                    Console.WriteLine($"  ioctl: {result} errno: {errno}");
                }

                ShowSettings(settings);

                // clear stuff that should be off
                settings.c_iflag &= ~(Nuttx.InputFlags.IGNBRK | Nuttx.InputFlags.BRKINT | Nuttx.InputFlags.PARMRK | Nuttx.InputFlags.ISTRIP | Nuttx.InputFlags.INLCR | Nuttx.InputFlags.IGNCR | Nuttx.InputFlags.ICRNL | Nuttx.InputFlags.IXON);
                settings.c_oflag &= ~Nuttx.OutputFlags.OPOST;
                settings.c_lflag &= ~(Nuttx.LocalFlags.ECHO | Nuttx.LocalFlags.ECHONL | Nuttx.LocalFlags.ICANON | Nuttx.LocalFlags.ISIG | Nuttx.LocalFlags.IEXTEN);
                settings.c_cflag &= ~(Nuttx.ControlFlags.CSIZE | Nuttx.ControlFlags.PARENB);

                // now set the user-requested settings
                switch (DataBits)
                {
                    case 5:
                        settings.c_cflag |= Nuttx.ControlFlags.CS5;
                        break;
                    case 6:
                        settings.c_cflag |= Nuttx.ControlFlags.CS6;
                        break;
                    case 7:
                        settings.c_cflag |= Nuttx.ControlFlags.CS7;
                        break;
                    case 8:
                        settings.c_cflag |= Nuttx.ControlFlags.CS8;
                        break;
                }
                switch (Parity)
                {
                    case Parity.Odd:
                        settings.c_cflag |= (Nuttx.ControlFlags.PARENB | Nuttx.ControlFlags.PARODD);
                        break;
                    case Parity.Even:
                        settings.c_cflag |= Nuttx.ControlFlags.PARENB;
                        break;
                }
                switch (StopBits)
                {
                    case StopBits.Two:
                        settings.c_cflag |= Nuttx.ControlFlags.CSTOPB;
                        break;
                }

                settings.c_speed = BaudRate;

                Console.WriteLine($"  Setting port settings...");
                result = Nuttx.ioctl(_driverHandle, TCSETS, gch.AddrOfPinnedObject());
                if (result < 0)
                {
                    var errno = Devices.UPD.GetLastError();
                    Console.WriteLine($"  ioctl: {result} errno: {errno}");
                }
            }
            finally
            {
                if (gch.IsAllocated) gch.Free();
            }
        }

        //define TCGETS          _TIOC(0x0001)  /* Get serial port settings: FAR struct termios* */
        //#define TCSETS          _TIOC(0x0002)  /* Set serial port settings: FAR const struct termios* */
        //#define TCSETSW         _TIOC(0x0003)  /* Drain output and set serial port settings: FAR const struct termios* */
        //#define TCSETSF         _TIOC(0x0004)  /* Drain output, discard intput, and set serial port settings: FAR const struct termios* */
        //#define TCGETA          _TIOC(0x0005)  /* See TCGETS: FAR struct termio* */

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
