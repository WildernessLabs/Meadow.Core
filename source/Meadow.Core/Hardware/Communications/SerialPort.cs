using Meadow.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using static Meadow.Core.Interop;

namespace Meadow.Hardware
{
    public delegate void SerialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e);

    /// <summary>
    /// Represents a port that is capable of serial (UART) communications.
    /// Preserved for legacy API compatibility. For a more modern approach, use
    /// `SerialMessagePort`.
    /// </summary>
    public class SerialPort : IDisposable, ISerialPort
    {
        protected const int TCSANOW = 0x00;
        protected const int TCGETS = 0x0101;
        protected const int TCSETS = 0x0102;
        protected const string DriverFolder = "/dev";
        protected const string SerialPortDriverPrefix = "tty";

        protected IntPtr _driverHandle = IntPtr.Zero;
        protected bool _showSerialDebug = false;
        protected CircularBuffer<byte> _readBuffer;
        protected Thread _readThread;
        protected int _readTimeout;
        protected int _baudRate;

        protected object _accessLock = new object();

        /// <summary>
        /// Indicates that data has been received through a port represented by the SerialPort object.
        /// </summary>
        public event SerialDataReceivedEventHandler DataReceived;

        /// <summary>
        /// Indicates that the internal data buffer has overrun and data has been lost.
        /// </summary>
        public event EventHandler BufferOverrun;

        /// <summary>
        /// Gets the port name used for communications.
        /// </summary>
        public string PortName { get; }
        /// <summary>
        /// Gets a value indicating the open or closed status of the SerialPort object.
        /// </summary>
        public bool IsOpen { get => _driverHandle != IntPtr.Zero; }
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
        public int ReadTimeout {
            get => _readTimeout;
            set {
                if (value == 0 || value < -1) {
                    throw new ArgumentOutOfRangeException();
                }
                _readTimeout = value;
            }
        }

        /// <summary>
        /// Gets the number of bytes of data in the receive buffer.
        /// </summary>
        public int BytesToRead {
            get => _readBuffer == null ? 0 : _readBuffer.Count;
        }

        internal static SerialPort From(
            SerialPortName portName,
            int baudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096)
        {

            return new SerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);

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
        protected SerialPort(
            SerialPortName portName,
            int baudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096)
        {
#if !DEBUG
            // ensure this is off in release (in case a dev sets it to true and fogets during check-in
            _showSerialDebug = false;
#endif
            if (baudRate <= 0) throw new ArgumentOutOfRangeException("Invalid baud rate");
            if (dataBits < 5 || dataBits > 8) throw new ArgumentOutOfRangeException("Invalid dataBits");

            PortName = portName.SystemName;
            BaudRate = baudRate;
            Parity = Parity;
            DataBits = dataBits;
            StopBits = stopBits;
            ReadTimeout = Timeout.Infinite;
            ReceiveBufferSize = readBufferSize;
        }

        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        public int BaudRate {
            get => _baudRate;
            set {
                if (value == BaudRate) return;
                if (value <= 0) throw new ArgumentOutOfRangeException();
                if (IsOpen) throw new IOException("You cannot change BaudRate on an Open port");

                _baudRate = value;
            }
        }

        /// <summary>
        /// Gets an array of serial port names for the current device.
        /// </summary>
        /// <returns></returns>
        public static string[] GetPortNames()
        {
            var allDevices = F7Micro.FileSystem.EnumDirectory(DriverFolder);
            var list = new List<string>();
            foreach (var s in allDevices) {
                if (s.StartsWith(SerialPortDriverPrefix)) {
                    list.Add(s);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Gets an array of supported baud rates
        /// </summary>
        /// <returns></returns>
        public int[] GetSupportedBaudRates()
        {
            return new int[]
                {
                    300,
                    600,
                    1200,
                    2400,
                    4800,
                    9600,
                    19200,
                    38400,
                    57600,
                    115200
                };
        }

        /// <summary>
        /// The buffer size, in bytes.
        /// </summary>
        public int ReceiveBufferSize {
            get => _readBuffer == null ? 0 : _readBuffer.MaxElements;
            private set {
                if (value == ReceiveBufferSize) return;
                // TODO: if there are events wired, we need to handle that.
                // for now this is private to prevent resizing
                _readBuffer = new CircularBuffer<byte>(value);
                _readBuffer.Overrun += OnReadBufferOverrun;
            }
        }

        private void OnReadBufferOverrun(object sender, EventArgs e)
        {
            try {
                BufferOverrun?.Invoke(this, EventArgs.Empty);
            } catch (Exception ex) {
                // ignore errors in the consumer's code
                Console.WriteLine($"Error in BufferOverrun handler: {ex.Message}");
            }
        }

        public override string ToString()
        {
            char p;
            switch (Parity) {
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
        public void ClearReceiveBuffer()
        {
            if (_readBuffer != null) {
                _readBuffer.Clear();
                _readBuffer.Overrun -= OnReadBufferOverrun;
            }
        }

        /// <summary>
        /// Opens a new serial port connection.
        /// </summary>
        public void Open()
        {
            if (IsOpen) throw new InvalidOperationException("Port is already open");

            var handle = Nuttx.open($"/dev/{PortName}", Nuttx.DriverFlags.ReadWrite | Nuttx.DriverFlags.SynchronizeOutput | Nuttx.DriverFlags.NonBlocking);
            if (handle.ToInt32() < 0) {
                throw new NativeException(UPD.GetLastError());
            }
            _driverHandle = handle;
            SetPortSettings();
            _readThread = new Thread(ReadThreadProc) {
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
            if (!IsOpen) {
                throw new InvalidOperationException("Cannot write to a closed port");
            }

            if (buffer == null) throw new ArgumentNullException();
            if (count > (buffer.Length - offset)) throw new ArgumentException("Count is larger than available data");
            if (offset < 0) throw new ArgumentException("Invalid offset");
            if (count == 0) return 0;

            Output.WriteLineIf(_showSerialDebug, $"Writing {count} bytes to {PortName}...");
            if (offset > 0) {
                // we need to make a copy
                var tmp = new byte[count];
                Array.Copy(buffer, offset, tmp, 0, count);
                var result = Nuttx.write(_driverHandle, tmp, count);
                if (result < 0) {
                    throw new NativeException(UPD.GetLastError());
                }
                return result;
            } else {
                var result = Nuttx.write(_driverHandle, buffer, count);
                if (result < 0) {
                    throw new NativeException(UPD.GetLastError());
                }
                return result;
            }
        }

        private void ReadThreadProc()
        {
            var readBuffer = new byte[4096];

            Output.WriteLineIf(_showSerialDebug, $"ReadThreadProc: {IsOpen}");

            while (IsOpen) {
                try {
                    var result = Nuttx.read(_driverHandle, readBuffer, readBuffer.Length);

                    if (result < 0) {
                        var ec = UPD.GetLastError();

                        Output.WriteLineIf(_showSerialDebug, $"Nuttx read returned {ec}");

                        if (ec == Nuttx.ErrorCode.TryAgain) {
                            // no data available, just wait
                            Thread.Sleep(200);
                        } else {
                            throw new NativeException(ec);
                        }
                    } else {
                        Output.WriteLineIf(_showSerialDebug, $"reading");

                        if (result > 0) {
                            //                            Output.WriteLineIf(_showSerialDebug, $"Enqueuing {result} bytes");
                            Output.WriteLineIf(_showSerialDebug, $"Enqueuing {BitConverter.ToString(readBuffer, 0, result)}");

                            _readBuffer.Enqueue(readBuffer, 0, result);

                            if (DataReceived != null) {
                                // put on a worker thread, else if the handler goes into some wait, we'll never process data
                                Task.Run(() => {
                                    try {
                                        Output.WriteLineIf(_showSerialDebug, $"Calling event handlers");
                                        DataReceived?.Invoke(this, new SerialDataReceivedEventArgs(SerialDataType.Chars));
                                        Output.WriteLineIf(_showSerialDebug, $"event handler complete");
                                    } catch (Exception ex) {
                                        // if the event handler throws, we don't want this to die
                                        Output.WriteLine($"Serial event handler threw: {ex.Message}");
                                    }
                                });
                            }
                        }
                    }
                } catch (Exception ex) {
                    Output.WriteLine($"ReadThreadProc error: {ex.Message}");
                }
            }

            Output.WriteLineIf(_showSerialDebug, $"ReadThreadProc: port closed");
        }

        /// <summary>
        /// Returns the next available by in the input buffer but does not consume it.
        /// </summary>
        /// <returns>The byte, cast to an Int32, or -1 if there is no data available in the input buffer.</returns>
        public int Peek()
        {
            if (_readBuffer == null || _readBuffer.Count == 0) return -1;
            return _readBuffer.Peek();
        }


        /// <summary>
        /// Synchronously reads one byte from the SerialPort input buffer.
        /// </summary>
        /// <returns>The byte, cast to an Int32, or -1 if the end of the stream has been read.</returns>
        public int ReadByte()
        {
            if (!IsOpen) {
                throw new InvalidOperationException("Cannot read from a closed port");
            }

            if (_readBuffer.Count == 0) return -1;
            return _readBuffer.Dequeue();
        }

        public byte[] ReadAll()
        {
            if (!IsOpen) {
                throw new InvalidOperationException("Cannot read from a closed port");
            }
            return _readBuffer.Dequeue(_readBuffer.Count);
        }

        /// <summary>
        /// Reads a number of bytes from the SerialPort input buffer and writes those bytes into a byte array at the specified offset.
        /// </summary>
        /// <param name="buffer">The byte array to write the input to.</param>
        /// <param name="offset">The offset in buffer at which to write the bytes.</param>
        /// <param name="count">The maximum number of bytes to read. Fewer bytes are read if count is greater than the number of bytes in the input buffer.</param>
        /// <returns>The number of bytes read.</returns>
        /// <exception cref="TimeoutException">No bytes were available to read.</exception>
        public int Read(byte[] buffer, int offset, int count)
        {
            if (!IsOpen) {
                throw new InvalidOperationException("Cannot read from a closed port");
            }

            if (buffer == null) throw new ArgumentNullException();
            if (count > (buffer.Length - offset)) throw new ArgumentException("Count is larger than available buffer size");
            if (offset < 0) throw new ArgumentException("Invalid offset");
            if (count == 0) return 0;

            var read = 0;

            Stopwatch sw = null;

            if (ReadTimeout > 0) {
                while (_readBuffer.Count == 0) {
                    if (sw == null) {
                        sw = new Stopwatch();
                        sw.Start();
                    } else {
                        if (sw.ElapsedMilliseconds > ReadTimeout) {
                            Output.WriteLineIf(_showSerialDebug, $"  Read timeout...");
                            throw new TimeoutException("Serial port read timeout");
                        }
                    }
                    Thread.Sleep(10);
                }
                if (sw != null) {
                    sw.Stop();
                }
            }

            if (count < _readBuffer.Count) {
                read = count;
            } else {
                read = _readBuffer.Count;
            }

            Array.Copy(_readBuffer.Dequeue(read), 0, buffer, offset, read);

            return read;
        }

        private void ShowSettings(Nuttx.Termios settings)
        {
            Console.WriteLine($"  Speed: {settings.c_speed}");
            Console.WriteLine($"  Input Flags: 0x{settings.c_iflag:x}");
            Console.WriteLine($"  OutputFlags: 0x{settings.c_oflag:x}");
            Console.WriteLine($"  Local Flags: 0x{settings.c_lflag:x}");
            Console.WriteLine($"  Control Flags: 0x{settings.c_cflag:x}");
        }

        private unsafe void SetPortSettings()
        {
            var settings = new Nuttx.Termios();

            // get the current settings           
            Output.WriteLineIf(_showSerialDebug, $"  Getting port settings for driver handle {_driverHandle}...");
            var result = Nuttx.tcgetattr(_driverHandle, ref settings);
            if (result != 0) {
                var error = UPD.GetLastError();
                throw new NativeException(error);
            }

            if (_showSerialDebug) {
                ShowSettings(settings);
            }

            // clear stuff that should be off
            settings.c_iflag &= ~(Nuttx.InputFlags.IGNBRK | Nuttx.InputFlags.BRKINT | Nuttx.InputFlags.PARMRK | Nuttx.InputFlags.ISTRIP | Nuttx.InputFlags.INLCR | Nuttx.InputFlags.IGNCR | Nuttx.InputFlags.ICRNL | Nuttx.InputFlags.IXON);
            settings.c_oflag &= ~Nuttx.OutputFlags.OPOST;
            settings.c_lflag &= ~(Nuttx.LocalFlags.ECHO | Nuttx.LocalFlags.ECHONL | Nuttx.LocalFlags.ICANON | Nuttx.LocalFlags.ISIG | Nuttx.LocalFlags.IEXTEN);
            settings.c_cflag &= ~(Nuttx.ControlFlags.CSIZE | Nuttx.ControlFlags.PARENB);

            // now set the user-requested settings
            switch (DataBits) {
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
            switch (Parity) {
                case Parity.Odd:
                    settings.c_cflag |= (Nuttx.ControlFlags.PARENB | Nuttx.ControlFlags.PARODD);
                    break;
                case Parity.Even:
                    settings.c_cflag |= Nuttx.ControlFlags.PARENB;
                    break;
            }
            switch (StopBits) {
                case StopBits.Two:
                    settings.c_cflag |= Nuttx.ControlFlags.CSTOPB;
                    break;
            }

            Output.WriteLineIf(_showSerialDebug, $"  Setting port settings at {BaudRate}...");
            Nuttx.cfsetspeed(ref settings, BaudRate);
            if (_showSerialDebug) {
                ShowSettings(settings);
            }

            result = Nuttx.tcsetattr(_driverHandle, TCSANOW, ref settings);

            if (result != 0) {
                throw new NativeException(UPD.GetLastError());
            }

            if (_showSerialDebug) {
                // get the settings again
                result = Nuttx.tcgetattr(_driverHandle, ref settings);
                if (result != 0) {
                    var error = UPD.GetLastError();
                    throw new NativeException(error);
                }
                ShowSettings(settings);
            }
        }

        private unsafe void SetPortSettingsIoctl()
        {
            var settings = new Nuttx.Termios();

            // get the current settings
            var p = (IntPtr)(&settings);

            Output.WriteLineIf(_showSerialDebug, $"  Getting port settings for driver handle {_driverHandle}...");
            var result = Nuttx.ioctl(_driverHandle, TCGETS, p);
            if (result != 0) {
                var error = UPD.GetLastError();
                throw new NativeException(error);
            }

            if (_showSerialDebug) {
                ShowSettings(settings);
            }

            // clear stuff that should be off
            settings.c_iflag &= ~(Nuttx.InputFlags.IGNBRK | Nuttx.InputFlags.BRKINT | Nuttx.InputFlags.PARMRK | Nuttx.InputFlags.ISTRIP | Nuttx.InputFlags.INLCR | Nuttx.InputFlags.IGNCR | Nuttx.InputFlags.ICRNL | Nuttx.InputFlags.IXON);
            settings.c_oflag &= ~Nuttx.OutputFlags.OPOST;
            settings.c_lflag &= ~(Nuttx.LocalFlags.ECHO | Nuttx.LocalFlags.ECHONL | Nuttx.LocalFlags.ICANON | Nuttx.LocalFlags.ISIG | Nuttx.LocalFlags.IEXTEN);
            settings.c_cflag &= ~(Nuttx.ControlFlags.CSIZE | Nuttx.ControlFlags.PARENB);

            // now set the user-requested settings
            switch (DataBits) {
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
            switch (Parity) {
                case Parity.Odd:
                    settings.c_cflag |= (Nuttx.ControlFlags.PARENB | Nuttx.ControlFlags.PARODD);
                    break;
                case Parity.Even:
                    settings.c_cflag |= Nuttx.ControlFlags.PARENB;
                    break;
            }
            switch (StopBits) {
                case StopBits.Two:
                    settings.c_cflag |= Nuttx.ControlFlags.CSTOPB;
                    break;
            }

            settings.c_speed = BaudRate;

            Output.WriteLineIf(_showSerialDebug, $"  Setting port settings at {BaudRate}...");
            if (_showSerialDebug) {
                ShowSettings(settings);
            }

            result = Nuttx.ioctl(_driverHandle, TCSETS, p);
            if (result != 0) {
                throw new NativeException(UPD.GetLastError());
            }

            if (_showSerialDebug) {
                // get the settings again
                result = Nuttx.ioctl(_driverHandle, TCGETS, p);
                if (result != 0) {
                    var error = UPD.GetLastError();
                    throw new NativeException(error);
                }
                ShowSettings(settings);
            }
        }
    }
}
