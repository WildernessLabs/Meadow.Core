using Meadow.Hardware;
using System;
using System.Runtime.InteropServices;

namespace Meadow;

/// <summary>
/// Represents a serial port on a Linux system.
/// </summary>
public class LinuxSerialPort : SerialPortBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinuxSerialPort"/> class.
    /// </summary>
    /// <param name="portName">The name of the serial port.</param>
    /// <param name="baudRate">The baud rate for the serial port communication.</param>
    /// <param name="dataBits">The number of data bits per byte.</param>
    /// <param name="parity">The parity bit for the serial port communication.</param>
    /// <param name="stopBits">The number of stop bits per byte.</param>
    /// <param name="readBufferSize">The size of the buffer for reading data.</param>
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

    /// <inheritdoc/>
    protected override void CloseHardwarePort(IntPtr handle)
    {
        Interop.close(handle.ToInt32());
    }

    /// <inheritdoc/>
    protected override IntPtr OpenHardwarePort(string portName)
    {
        string driverName;

        if (portName.StartsWith("/"))
        {
            driverName = portName;
        }
        else
        {
            driverName = $"/dev/{portName}";
        }

        Resolver.Log.Trace($"Opening linux serial port {driverName}");

        var handle = Interop.open(driverName, Interop.DriverFlags.O_RDWR | Interop.DriverFlags.O_NOCTTY);

        if (handle < 0)
        {
            var err = Marshal.GetLastWin32Error();
            switch (err)
            {
                case 13:
                    throw new NativeException($"Unable to open port '{driverName}'. Permission Denied ({err})");
                default:
                    throw new NativeException($"Unable to open port '{driverName}'. Native error {err}");
            }
        }

        return new IntPtr(handle);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    protected override void SetHardwarePortSettings(IntPtr handle)
    {
        // get the current settings
        var settings = new Interop.termios();
        Interop.tcgetattr(handle.ToInt32(), ref settings);


        // clear stuff that should be off
        settings.c_iflag &= ~(Interop.InputFlags.IGNBRK | Interop.InputFlags.BRKINT | Interop.InputFlags.PARMRK | Interop.InputFlags.ISTRIP | Interop.InputFlags.INLCR | Interop.InputFlags.IGNCR | Interop.InputFlags.ICRNL | Interop.InputFlags.IXON);
        settings.c_oflag &= ~Interop.OutputFlags.OPOST;
        settings.c_lflag &= ~(Interop.LocalFlags.ECHO | Interop.LocalFlags.ECHONL | Interop.LocalFlags.ICANON | Interop.LocalFlags.ISIG | Interop.LocalFlags.IEXTEN);
        settings.c_cflag &= ~(Interop.ControlFlags.CSIZE | Interop.ControlFlags.PARENB | Interop.ControlFlags.CRTSCTS);

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

        settings.c_cflag |= Interop.ControlFlags.CREAD | Interop.ControlFlags.CLOCAL;

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

        var result = Interop.cfsetspeed(ref settings, BaudRate);
        if (result != 0)
        {
            throw new NativeException($"cfsetspeed failed: {Marshal.GetLastWin32Error()}");
        }

        result = Interop.tcsetattr(handle.ToInt32(), Interop.TCSANOW, ref settings);

        if (result != 0)
        {
            throw new NativeException($"tcsetattr failed: {Marshal.GetLastWin32Error()}");
        }
    }

    /// <inheritdoc/>
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
