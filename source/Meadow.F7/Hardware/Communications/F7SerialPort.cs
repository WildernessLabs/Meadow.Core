using Meadow.Devices;
using System;
using System.Collections.Generic;
using System.Threading;
using static Meadow.Core.Interop;

namespace Meadow.Hardware
{
    public class F7SerialPort : SerialPortBase
    {
        protected const int TCSANOW = 0x00;
        protected const int TCGETS = 0x0101;
        protected const int TCSETS = 0x0102;
        protected const string DriverFolder = "/dev";
        protected const string SerialPortDriverPrefix = "tty";

        internal static F7SerialPort From(
            SerialPortName portName,
            int baudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096)
        {

            return new F7SerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);

        }

        /// <summary>
        /// Initializes a new instance of a legacy `ISerialPort`. `ISerialPort`
        /// is provided for legacy compatibility, we recommend using the more
        /// modern, thread-safe `ISerialMessagePort`.
        /// </summary>
        /// <param name="portName">The 'SerialPortName` of port to use.</param>
        /// <param name="baudRate">Speed, in bits per second, of the serial port.</param>
        /// <param name="parity">`Parity` enum describing what type of
        /// cyclic-redundancy-check (CRC) bit, if any, should be expected in the
        /// serial message frame. Default is `Parity.None`.</param>
        /// <param name="dataBits">Number of data bits expected in the serial
        /// message frame. Default is `8`.</param>
        /// <param name="stopBits">`StopBits` describing how many bits should be
        /// expected at the end of every character in the serial message frame.
        /// Default is `StopBits.One`.</param>
        /// <param name="readBufferSize">Size, in bytes, of the read buffer. Default
        /// is 1024.</param>
        /// <returns></returns>
        protected F7SerialPort(
            SerialPortName portName,
            int baudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096)
            : base(portName, baudRate, dataBits, parity, stopBits, readBufferSize)
        {
        }

        /// <summary>
        /// Gets an array of serial port names for the current device.
        /// </summary>
        /// <returns></returns>
        public static string[] GetPortNames()
        {
            var allDevices = F7Micro.FileSystem.EnumDirectory(DriverFolder);
            var list = new List<string>();
            foreach (var s in allDevices)
            {
                if (s.StartsWith(SerialPortDriverPrefix))
                {
                    list.Add(s);
                }
            }

            return list.ToArray();
        }

        protected override IntPtr OpenHardwarePort(string portName)
        {
            var handle = Nuttx.open($"/dev/{PortName}", Nuttx.DriverFlags.ReadWrite | Nuttx.DriverFlags.SynchronizeOutput);
            if (handle.ToInt32() < 0)
            {
                throw new NativeException(UPD.GetLastError().ToString());
            }
            return handle;
        }

        protected override void CloseHardwarePort(IntPtr handle)
        {
            Nuttx.close(handle);
        }

        protected override int WriteHardwarePort(IntPtr handle, byte[] data, int count)
        {
            var result = Nuttx.write(handle, data, count);

            if (result < 0)
            {
                throw new NativeException(UPD.GetLastError().ToString());
            }

            return result;
        }

        protected override int ReadHardwarePort(IntPtr handle, byte[] readBuffer, int count)
        {
            var result = Nuttx.read(handle, readBuffer, count);

            if (result < 0)
            {
                var ec = UPD.GetLastError();

                if (ec == Nuttx.ErrorCode.TryAgain)
                {
                    // no data available, just wait
                    return 0;
                }
                else
                {
                    throw new NativeException(ec.ToString());
                }
            }

            return result;
        }

        private void ShowSettings(Nuttx.Termios settings)
        {
            Console.WriteLine($"  Speed: {settings.c_speed}");
            Console.WriteLine($"  Input Flags: 0x{settings.c_iflag:x}");
            Console.WriteLine($"  OutputFlags: 0x{settings.c_oflag:x}");
            Console.WriteLine($"  Local Flags: 0x{settings.c_lflag:x}");
            Console.WriteLine($"  Control Flags: 0x{settings.c_cflag:x}");
        }

        protected override unsafe void SetHardwarePortSettings(IntPtr handle)
        {
            var settings = new Nuttx.Termios();

            // get the current settings           
            Output.WriteLineIf(_showSerialDebug, $"  Getting port settings for driver handle {handle}...");
            var result = Nuttx.tcgetattr(handle, ref settings);
            if (result != 0)
            {
                var error = UPD.GetLastError();
                throw new NativeException(error.ToString());
            }

            if (_showSerialDebug)
            {
                ShowSettings(settings);
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

            Output.WriteLineIf(_showSerialDebug, $"  Setting port settings at {BaudRate}...");
            Nuttx.cfsetspeed(ref settings, BaudRate);
            if (_showSerialDebug)
            {
                ShowSettings(settings);
            }

            result = Nuttx.tcsetattr(handle, TCSANOW, ref settings);

            if (result != 0)
            {
                throw new NativeException(UPD.GetLastError().ToString());
            }

            if (_showSerialDebug)
            {
                // get the settings again
                result = Nuttx.tcgetattr(handle, ref settings);
                if (result != 0)
                {
                    var error = UPD.GetLastError();
                    throw new NativeException(error.ToString());
                }
                ShowSettings(settings);
            }
        }

        private unsafe void SetPortSettingsIoctl(IntPtr handle)
        {
            var settings = new Nuttx.Termios();

            // get the current settings
            var p = (IntPtr)(&settings);

            Output.WriteLineIf(_showSerialDebug, $"  Getting port settings for driver handle {handle}...");
            var result = Nuttx.ioctl(handle, TCGETS, p);
            if (result != 0)
            {
                var error = UPD.GetLastError();
                throw new NativeException(error.ToString());
            }

            if (_showSerialDebug)
            {
                ShowSettings(settings);
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

            Output.WriteLineIf(_showSerialDebug, $"  Setting port settings at {BaudRate}...");
            if (_showSerialDebug)
            {
                ShowSettings(settings);
            }

            result = Nuttx.ioctl(handle, TCSETS, p);
            if (result != 0)
            {
                throw new NativeException(UPD.GetLastError().ToString());
            }

            if (_showSerialDebug)
            {
                // get the settings again
                result = Nuttx.ioctl(handle, TCGETS, p);
                if (result != 0)
                {
                    var error = UPD.GetLastError();
                    throw new NativeException(error.ToString());
                }
                ShowSettings(settings);
            }
        }
    }
}
