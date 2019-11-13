using Meadow.Devices;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using static Meadow.Core.Interop;

namespace Meadow.Hardware
{
    public delegate void SerialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e);

    /// <summary>
    /// Represents a port that is capable of serial (UART) communications.
    /// </summary>
    public class SerialPort : IDisposable
    {
        private const int TCGETS = 0x0101;
        private const int TCSETS = 0x0102;

        private IntPtr _driverHandle = IntPtr.Zero;
        private bool _showSerialDebug = false;
        private CircularBuffer<byte> _readBuffer;
        private Thread _readThread;
        private int _readTimeout;

        /// <summary>
        /// Indicates that data has been received through a port represented by the SerialPort object.
        /// </summary>
        public event SerialDataReceivedEventHandler DataReceived;

        /// <summary>
        /// Gets the port name used for communications.
        /// </summary>
        public string PortName { get; }
        /// <summary>
        /// Gets a value indicating the open or closed status of the SerialPort object.
        /// </summary>
        public bool IsOpen { get => _driverHandle != IntPtr.Zero; }
        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        public int BaudRate { get; }
        /// <summary>
        /// Gets or sets the parity-checking protocol.
        /// </summary>
        public Parity Parity { get; }
        /// <summary>
        /// Gets or sets the standard length of data bits per byte.
        /// </summary>
        public int DataBits { get; }
        /// <summary>
        /// Gets or sets the standard number of stopbits per byte.
        /// </summary>
        public StopBits StopBits { get; }
        /// <summary>
        /// The number of milliseconds before a time-out occurs when a read operation does not finish.
        /// </summary>
        /// <remarks>The time-out can be set to any value greater than zero, or set to InfiniteTimeout, in which case no time-out occurs. InfiniteTimeout is the default.</remarks>
        public int ReadTimeout
        {
            get => _readTimeout;
            set
            {
                if (value == 0 || value < -1)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _readTimeout = value;
            }
        }

        /// <summary>
        /// Gets the number of bytes of data in the receive buffer.
        /// </summary>
        public int BytesToRead
        {
            get => _readBuffer == null ? 0 : _readBuffer.Count;
        }

        /// <summary>
        /// Initializes a new instance of the SerialPort class.
        /// </summary>
        /// <param name="portName">The port to use (for example, 'ttyS1').</param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="readBufferSize"></param>
        public SerialPort(string portName, int baudRate, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, int readBufferSize = 4096)
        {
#if !DEBUG
            // ensure this is off in release (in case a dev sets it to true and fogets during check-in
            _showSerialDebug = false;
#endif
            if (baudRate <= 0) throw new ArgumentOutOfRangeException("Invalid baud rate");
            if (dataBits < 5 || dataBits > 8) throw new ArgumentOutOfRangeException("Invalid dataBits");

            PortName = portName;
            BaudRate = baudRate;
            Parity = Parity;
            DataBits = dataBits;
            StopBits = stopBits;
            ReadTimeout = Timeout.Infinite;
            ReadBufferSize = readBufferSize;
        }

        /// <summary>
        /// The buffer size, in bytes.
        /// </summary>
        public int ReadBufferSize
        {
            get => _readBuffer == null ? 0 : _readBuffer.MaxElements;
            private set
            {
                if (value == ReadBufferSize) return;
                // TODO: if there are events wired, we need to handle that.
                // for now this is private to prevent resizing
                _readBuffer = new CircularBuffer<byte>(value);
            }
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

            return $"{PortName}: {BaudRate},{DataBits},{p},{(StopBits == StopBits.Two ? 2 : 1)}";
        }

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Discards data from the serial driver's receive buffer.
        /// </summary>
        public void DiscardInBuffer()
        {
            if (_readBuffer != null)
            {
                _readBuffer.Clear();
            }
        }

        /// <summary>
        /// Opens a new serial port connection.
        /// </summary>
        public void Open()
        {
            if (IsOpen) throw new InvalidOperationException("Port is already open");

            var handle = Nuttx.open($"/dev/{PortName}", Nuttx.DriverFlags.ReadWrite | Nuttx.DriverFlags.SynchronizeOutput);
            if (handle.ToInt32() < 0)
            {
                throw new NativeException(UPD.GetLastError());
            }
            _driverHandle = handle;
            SetPortSettings();
            _readThread = new Thread(ReadThreadProc)
            {
                IsBackground = true,
                Name = "Serial Read Thread"
            };
            _readThread.Start();
        }

        /// <summary>
        /// Closes the port connection and sets the IsOpen property to false.
        /// </summary>
        public void Close()
        {
            if (!IsOpen) return;

            Nuttx.close(_driverHandle);
            _driverHandle = IntPtr.Zero;
        }

        /// <summary>
        /// Writes data to the serial port.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int Write(byte[] buffer)
        {
            return Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes a specified number of bytes to the serial port using data from a buffer.
        /// </summary>
        /// <param name="buffer">The byte array that contains the data to write to the port.</param>
        /// <param name="offset">The zero-based byte offset in the buffer parameter at which to begin copying bytes to the port.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <returns></returns>
        public int Write(byte[] buffer, int offset, int count)
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("Cannot write to a closed port");
            }

            if (buffer == null) throw new ArgumentNullException();
            if (count > (buffer.Length - offset)) throw new ArgumentException("Count is larger than available data");
            if (offset < 0) throw new ArgumentException("Invalid offset");
            if (count == 0) return 0;

            Output.WriteLineIf(_showSerialDebug, $"Writing {count} bytes to {PortName}...");
            if (offset > 0)
            {
                // we need to make a copy
                var tmp = new byte[count];
                Array.Copy(buffer, offset, tmp, 0, count);
                var result = Nuttx.write(_driverHandle, tmp, count);
                if (result < 0)
                {
                    throw new NativeException(UPD.GetLastError());
                }
                return result;
            }
            else
            {
                var result = Nuttx.write(_driverHandle, buffer, count);
                return result;
            }
        }

        private void ReadThreadProc()
        {
            var readBuffer = new byte[4096];

            while (IsOpen)
            {
                var result = Nuttx.read(_driverHandle, readBuffer, readBuffer.Length);

                if (result < 0)
                {
                    throw new NativeException(UPD.GetLastError());
                }
                else
                {
                    _readBuffer.Enqueue(readBuffer, 0, result);
                    DataReceived?.Invoke(this, new SerialDataReceivedEventArgs(SerialDataType.Chars));
                }

                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Synchronously reads one byte from the SerialPort input buffer.
        /// </summary>
        /// <returns>The byte, cast to an Int32, or -1 if the end of the stream has been read.</returns>
        public int ReadByte()
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("Cannot read from a closed port");
            }

            if (_readBuffer.Count == 0) return -1;
            return _readBuffer.Dequeue();
        }

        /// <summary>
        /// Reads a number of bytes from the SerialPort input buffer and writes those bytes into a byte array at the specified offset.
        /// </summary>
        /// <param name="buffer">The byte array to write the input to.</param>
        /// <param name="offset">The offset in buffer at which to write the bytes.</param>
        /// <param name="count">The maximum number of bytes to read. Fewer bytes are read if count is greater than the number of bytes in the input buffer.</param>
        /// <returns>The number of bytes read.</returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("Cannot read from a closed port");
            }

            if (buffer == null) throw new ArgumentNullException();
            if (count > (buffer.Length - offset)) throw new ArgumentException("Count is larger than available buffer size");
            if (offset < 0) throw new ArgumentException("Invalid offset");
            if (count == 0) return 0;

            var read = 0;

            Stopwatch sw = null;

            if (ReadTimeout > 0)
            {
                while (_readBuffer.Count == 0)
                {
                    if (sw == null)
                    {
                        sw = new Stopwatch();
                        sw.Start();
                    }
                    else
                    {
                        if (sw.ElapsedMilliseconds > ReadTimeout)
                        {
                            throw new TimeoutException("Serial port read timeout");
                        }
                    }
                    Thread.Sleep(10);
                }
                if (sw != null)
                {
                    sw.Stop();
                }
            }

            if (count < _readBuffer.Count)
            {
                read = count;
                Array.Copy(_readBuffer.Dequeue(count), 0, buffer, offset, count);
            }
            else
            {
                read = _readBuffer.Count;
                Array.Copy(_readBuffer.Dequeue(read), 0, buffer, offset, read);
            }

            return read;
        }

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
                Output.WriteLineIf(_showSerialDebug, $"  Getting port settings for driver handle {_driverHandle}...");
                var result = Nuttx.ioctl(_driverHandle, TCGETS, gch.AddrOfPinnedObject());
                if (result < 0)
                {
                    var errno = Devices.UPD.GetLastError();
                    Output.WriteLineIf(_showSerialDebug, $"  ioctl: {result} errno: {errno}");
                }

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

                Output.WriteLineIf(_showSerialDebug, $"  Setting port settings...");
                result = Nuttx.ioctl(_driverHandle, TCSETS, gch.AddrOfPinnedObject());
                if (result < 0)
                {
                    throw new NativeException(UPD.GetLastError());
                }

                if (_showSerialDebug)
                {
                    Console.WriteLine($"  Verifying...");
                    result = Nuttx.ioctl(_driverHandle, TCGETS, gch.AddrOfPinnedObject());
                    ShowSettings(settings);
                }
            }
            finally
            {
                if (gch.IsAllocated) gch.Free();
            }
        }
    }

}
