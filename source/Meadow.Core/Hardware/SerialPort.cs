using Meadow.Devices;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
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
        private bool _showSerialDebug = false;

        public string PortName { get; }
        public bool IsOpen { get => _driverHandle != IntPtr.Zero; }
        public int BaudRate { get; }
        public Parity Parity { get; }
        public int DataBits { get; }
        public StopBits StopBits { get; }

        public SerialPort(string portName, int baudRate, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
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

        public int Write(byte[] buffer)
        {
            return Write(buffer, 0, buffer.Length);
        }

        public int Write(byte[] buffer, int offset, int count)
        {
            if (!IsOpen)
            {
                throw new Exception("Cannot write to a closed port");
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
                return result;
            }
            else
            {
                var result = Nuttx.write(_driverHandle, buffer, count);
                return result;
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
                Console.WriteLine($" Read port failed.  Error: {errno}");

                return 0;
            }
            else
            {
                Array.Copy(buf, 0, buffer, offset, result);
            }

            return result;

        }

        private const int TCGETS = 0x0101;
        private const int TCSETS = 0x0102;

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
                    var errno = Devices.UPD.GetLastError();
                    Console.WriteLine($"Failed to set serial port settings. Error: {errno}");
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
    }

}
