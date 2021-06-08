using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Runtime.InteropServices;

namespace Meadow
{
    public class LinuxSerialPort : SerialPortBase
    {
        public LinuxSerialPort(
            SerialPortName portName,
            int baudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096)
            : base(portName, baudRate, dataBits, parity, stopBits, readBufferSize)
        {
        }

        protected override void CloseHardwarePort(IntPtr handle)
        {
            Interop.close(handle.ToInt32());
        }

        protected override IntPtr OpenHardwarePort(string portName)
        {
            var driverName = $"/dev/{PortName}";
            var handle = Interop.open(driverName, Interop.DriverFlags.O_RDWR);

            if (handle < 0)
            {
                throw new NativeException($"Unable to open port '{driverName}'. Native error {Marshal.GetLastWin32Error()}");
            }

            return new IntPtr(handle);
        }

        protected override int ReadHardwarePort(IntPtr handle, byte[] readBuffer, int count)
        {
            var result = Interop.read(handle.ToInt32(), readBuffer, count);

            if (result < 0)
            {
                // will we potentially get E_TRY_AGAIN? (11)
                throw new NativeException($"Unable to read from port. Native error {Marshal.GetLastWin32Error()}");
            }

            return result;
        }

        protected override void SetHardwarePortSettings(IntPtr handle)
        {
            // get the current settings
            var settings = new Interop.termios(); 
            Interop.tcgetattr(handle.ToInt32(), ref settings);


            // clear stuff that should be off
            settings.c_iflag &= ~(Interop.InputFlags.IGNBRK | Interop.InputFlags.BRKINT | Interop.InputFlags.PARMRK | Interop.InputFlags.ISTRIP | Interop.InputFlags.INLCR | Interop.InputFlags.IGNCR | Interop.InputFlags.ICRNL | Interop.InputFlags.IXON);
            settings.c_oflag &= ~Interop.OutputFlags.OPOST;
            settings.c_lflag &= ~(Interop.LocalFlags.ECHO | Interop.LocalFlags.ECHONL | Interop.LocalFlags.ICANON | Interop.LocalFlags.ISIG | Interop.LocalFlags.IEXTEN);
            settings.c_cflag &= ~(Interop.ControlFlags.CSIZE | Interop.ControlFlags.PARENB);

            // now set the user-requested settings
            switch (DataBits)
            {
                case 5:
                    settings.c_cflag |= Interop.ControlFlags.CS5;
                    break;
                case 6:
                    settings.c_cflag |= Interop.ControlFlags.CS6;
                    break;
                case 7:
                    settings.c_cflag |= Interop.ControlFlags.CS7;
                    break;
                case 8:
                    settings.c_cflag |= Interop.ControlFlags.CS8;
                    break;
            }
            switch (Parity)
            {
                case Parity.Odd:
                    settings.c_cflag |= (Interop.ControlFlags.PARENB | Interop.ControlFlags.PARODD);
                    break;
                case Parity.Even:
                    settings.c_cflag |= Interop.ControlFlags.PARENB;
                    break;
            }
            switch (StopBits)
            {
                case StopBits.Two:
                    settings.c_cflag |= Interop.ControlFlags.CSTOPB;
                    break;
            }

            Output.WriteLineIf(_showSerialDebug, $"  Setting port settings at {BaudRate}...");
            Interop.cfsetspeed(ref settings, BaudRate);

            /*
            if (_showSerialDebug)
            {
                ShowSettings(settings);
            }
            */

            var result = Interop.tcsetattr(handle.ToInt32(), Interop.TCSANOW, ref settings);

            if (result != 0)
            {
//                throw new NativeException(UPD.GetLastError().ToString());
            }
        }

        protected override int WriteHardwarePort(IntPtr handle, byte[] writeBuffer, int count)
        {
            var result = Interop.write(handle.ToInt32(), writeBuffer, count);

            if (result < 0)
            {
                throw new NativeException($"Unable to write to port. Native error {Marshal.GetLastWin32Error()}");
            }

            return result;
        }
    }
}
