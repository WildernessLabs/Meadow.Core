using Meadow.Devices;
using System;
using System.Collections.Generic;
using static Meadow.Core.Interop;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a serial port on STM32F7 devices
    /// </summary>
    public class F7SerialPort : SerialPortBase
    {
        private const int TCSANOW = 0x00;
        private const int TCGETS = 0x0101;
        private const int TCSETS = 0x0102;
        private const string DriverFolder = "/dev";
        private const string SerialPortDriverPrefix = "tty";

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
            var allDevices = F7FeatherBase.FileSystem.EnumDirectory(DriverFolder);
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

        /// <summary>
        /// Opens the OS hardware port
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        /// <exception cref="NativeException"></exception>
        protected override IntPtr OpenHardwarePort(string portName)
        {
            var handle = Nuttx.open($"/dev/{PortName}", Nuttx.DriverFlags.ReadWrite | Nuttx.DriverFlags.SynchronizeOutput);
            if (handle.ToInt32() < 0)
            {
                throw new NativeException(UPD.GetLastError().ToString());
            }
            return handle;
        }

        /// <summary>
        /// Closes the OS hardware port
        /// </summary>
        /// <param name="handle"></param>
        protected override void CloseHardwarePort(IntPtr handle)
        {
            Nuttx.close(handle);
        }

        /// <summary>
        /// Writes data to the OS hardware port
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="NativeException"></exception>
        protected override int WriteHardwarePort(IntPtr handle, byte[] data, int count)
        {
            var result = Nuttx.write(handle, data, count);

            if (result < 0)
            {
                throw new NativeException(UPD.GetLastError().ToString());
            }

            return result;
        }

        /// <summary>
        /// Reads data from the OS hardware port
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="readBuffer"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="NativeException"></exception>
        protected override int ReadHardwarePort(IntPtr handle, byte[] readBuffer, int count)
        {
            int result;
            do
            {
                result = Nuttx.read(handle, readBuffer, count);
            } while (result < 0 && UPD.GetLastError() == Nuttx.ErrorCode.InterruptedSystemCall);

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


        /// <summary>
        /// Calls the OS to set serial port settings (e.g. parity and stop bits)
        /// </summary>
        /// <param name="handle"></param>
        /// <exception cref="NativeException"></exception>
        protected override unsafe void SetHardwarePortSettings(IntPtr handle)
        {
            var settings = new Nuttx.Termios();

            // get the current settings           
            var result = Nuttx.tcgetattr(handle, ref settings);
            if (result != 0)
            {
                var error = UPD.GetLastError();
                throw new NativeException(error.ToString());
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

            Nuttx.cfsetspeed(ref settings, BaudRate);

            result = Nuttx.tcsetattr(handle, TCSANOW, ref settings);

            if (result != 0)
            {
                throw new NativeException(UPD.GetLastError().ToString());
            }

            if (Resolver.Log.LogLevel <= Logging.LogLevel.Debug)
            {
                // get the settings again
                result = Nuttx.tcgetattr(handle, ref settings);
                if (result != 0)
                {
                    var error = UPD.GetLastError();
                    throw new NativeException(error.ToString());
                }
            }
        }

        private unsafe void SetPortSettingsIoctl(IntPtr handle)
        {
            var settings = new Nuttx.Termios();

            // get the current settings
            var p = (IntPtr)(&settings);

            var result = Nuttx.ioctl(handle, TCGETS, p);
            if (result != 0)
            {
                var error = UPD.GetLastError();
                throw new NativeException(error.ToString());
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

            result = Nuttx.ioctl(handle, TCSETS, p);
            if (result != 0)
            {
                throw new NativeException(UPD.GetLastError().ToString());
            }

            if (Resolver.Log.LogLevel <= Logging.LogLevel.Debug)
            {
                // get the settings again
                result = Nuttx.ioctl(handle, TCGETS, p);
                if (result != 0)
                {
                    var error = UPD.GetLastError();
                    throw new NativeException(error.ToString());
                }
            }
        }
    }
}
