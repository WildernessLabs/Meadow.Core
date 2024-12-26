using Meadow.Foundation.Displays;
using Meadow.Hardware;
using Meadow.Networking;
using Meadow.Peripherals.Displays;
using Meadow.Units;
using System;
using System.Diagnostics;
using System.Linq;

namespace Meadow;

/// <summary>
/// Represents a Linux-based Meadow device.
/// </summary>
public abstract class Linux : IMeadowDevice
#if NET7_0
    , IPixelDisplayProvider
#endif
{
    private SysFsGpioDriver? _sysfs = null!;
    private Gpiod? _gpiod = null!;
    private NmCliNetworkAdapterCollection? _networkAdapters;

#pragma warning disable CS0067
    /// <inheritdoc/>
    public event PowerTransitionHandler? BeforeReset;
    /// <inheritdoc/>
    public event PowerTransitionHandler? BeforeSleep;
    /// <inheritdoc/>
    public event PowerTransitionHandler? AfterWake;
    /// <inheritdoc/>
    public event NetworkConnectionHandler? NetworkConnected;
    /// <inheritdoc/>
    public event NetworkDisconnectionHandler? NetworkDisconnected;
#pragma warning restore CS0067 

    /// <inheritdoc/>
    public virtual DeviceCapabilities Capabilities { get; }
    /// <inheritdoc/>
    public virtual IPlatformOS PlatformOS { get; }
    /// <inheritdoc/>
    public virtual IDeviceInformation Information { get; }
    /// <inheritdoc/>
    public virtual INetworkAdapterCollection NetworkAdapters => _networkAdapters ??= new NmCliNetworkAdapterCollection();

    /// <summary>
    /// Creates the Meadow on Linux infrastructure instance
    /// </summary>
    public Linux()
    {
        PlatformOS = new LinuxPlatformOS();

        Information = new LinuxDeviceInfo();

        Capabilities = new DeviceCapabilities(
            new AnalogCapabilities(false, null),
            new NetworkCapabilities(false, true),
            new StorageCapabilities(true)
            );
    }

    /// <inheritdoc/>
    public IResizablePixelDisplay CreateDisplay(int? width = null, int? height = null)
    {
        return new SilkDisplay(width ?? 320, height ?? 240);
    }

    /// <inheritdoc/>
    public virtual void Initialize(MeadowPlatform detectedPlatform)
    {
        _sysfs = new SysFsGpioDriver();

        try
        {
            _gpiod = new Gpiod(Resolver.Log);
            Resolver.Log.Debug("Platform will use gpiod for GPIO");
        }
        catch
        {
            Resolver.Log.Debug("Platform does not support gpiod. Sysfs will be used for GPIO");
        }
    }

    /// <inheritdoc/>
    public virtual IPin GetPin(string pinName)
    {
        throw new PlatformNotSupportedException("This platform has no IPins");
    }

    /// <inheritdoc/>
    public virtual II2cBus CreateI2cBus(int busNumber = 1)
    {
        return CreateI2cBus(busNumber, II2cController.DefaultI2cBusSpeed);
    }

    /// <inheritdoc/>
    public II2cBus CreateI2cBus(int busNumber, I2cBusSpeed busSpeed)
    {
        return new I2CBus(busNumber, busSpeed);
    }

    /// <inheritdoc/>
    public II2cBus CreateI2cBus(IPin[] pins, I2cBusSpeed busSpeed)
    {
        return CreateI2cBus(pins[0], pins[1], busSpeed);
    }

    /// <inheritdoc/>
    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] suffixDelimiter, bool preserveDelimiter, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
    {
        var classicPort = CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        return SerialMessagePort.From(classicPort, suffixDelimiter, preserveDelimiter);
    }

    /// <inheritdoc/>
    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] prefixDelimiter, bool preserveDelimiter, int messageLength, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
    {
        var classicPort = CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        return SerialMessagePort.From(classicPort, prefixDelimiter, preserveDelimiter, messageLength);
    }

    /// <inheritdoc/>
    public ISerialPort CreateSerialPort(SerialPortName portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 1024)
    {
        return new LinuxSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
    }

    /// <inheritdoc/>
    public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
    {
        if (pin is RaspberryPi.LedPin)
        {
            return new RaspberryPi.LedOutputPort(pin, initialState);
        }
        else if (_gpiod != null)
        {
            return new GpiodDigitalOutputPort(_gpiod, pin, initialState);
        }
        else if (_sysfs != null)
        {
            return new SysFsDigitalOutputPort(_sysfs, pin, initialState);
        }
        throw new Exception("No GPIO driver available");
    }

    /// <inheritdoc/>
    public IDigitalInputPort CreateDigitalInputPort(IPin pin)
    {
        return CreateDigitalInputPort(pin, ResistorMode.Disabled);
    }

    /// <inheritdoc/>
    public IDigitalInputPort CreateDigitalInputPort(IPin pin, ResistorMode resistorMode)
    {
        if (_gpiod != null)
        {
            return new GpiodDigitalInputPort(_gpiod, pin, new GpiodDigitalChannelInfo(pin.Name), resistorMode);
        }
        else if (_sysfs != null)
        {
            return new SysFsDigitalInputPort(_sysfs, pin, new SysFsDigitalChannelInfo(pin.Name), resistorMode);
        }
        throw new Exception("No GPIO driver available");
    }

    /// <inheritdoc/>
    public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
    {
        if (_gpiod != null)
        {
            return new GpiodDigitalInterruptPort(_gpiod, pin, new GpiodDigitalChannelInfo(pin.Name), interruptMode, resistorMode, debounceDuration, glitchDuration);
        }
        else if (_sysfs != null)
        {
            return new SysFsDigitalInterruptPort(_sysfs, pin, new SysFsDigitalChannelInfo(pin.Name), interruptMode, resistorMode, debounceDuration, glitchDuration);
        }
        throw new Exception("No GPIO driver available");
    }

    /// <inheritdoc/>
    public virtual ISpiBus CreateSpiBus(int busNumber, Units.Frequency speed)
    {
        return new SpiBus(busNumber, 0, SpiBus.SpiMode.Mode0, speed);
    }

    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, SpiClockConfiguration config)
    {
        return CreateSpiBus(clock, mosi, miso, config.SpiMode, config.Speed);
    }

    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, Units.Frequency speed)
    {
        return CreateSpiBus(clock, mosi, miso, SpiClockConfiguration.Mode.Mode0, speed);
    }

    /// <inheritdoc/>
    public virtual ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, SpiClockConfiguration.Mode mode, Units.Frequency speed)
    {
        // verify pins are SPI capable
        var clockChannel = clock.SupportedChannels
            ?.OfType<ISpiChannelInfo>()
            .Where(c => c.LineTypes == SpiLineType.Clock)
            .FirstOrDefault();
        if (clockChannel == null)
        {
            throw new ArgumentException($"Pin {clock.Name} does not support SPI Clock");
        }

        var mosiChannel = mosi.SupportedChannels
            ?.OfType<ISpiChannelInfo>()
            .Where(c => c.LineTypes == SpiLineType.MOSI)
            .FirstOrDefault();
        if (mosiChannel == null)
        {
            throw new ArgumentException($"Pin {mosi.Name} does not support SPI MOSI");
        }

        var misoChannel = miso.SupportedChannels
            ?.OfType<ISpiChannelInfo>()
            .Where(c => c.LineTypes == SpiLineType.MISO)
            .FirstOrDefault();
        if (misoChannel == null)
        {
            throw new ArgumentException($"Pin {miso.Name} does not support SPI MISO");
        }

        if ((clockChannel.BusNumber != mosiChannel.BusNumber)
            || (clockChannel.BusNumber != misoChannel.BusNumber)
            || (mosiChannel.BusNumber != misoChannel.BusNumber))
        {
            throw new ArgumentException($"Pins {clock.Name}, {mosi.Name} and {miso.Name} are on different SPI buses");
        }

        return CreateSpiBus(clockChannel.BusNumber, speed);
    }

    /// <inheritdoc/>
    public IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, float voltageReference = 3.3F)
    {
        return CreateAnalogInputPort(pin, sampleCount, sampleInterval, voltageReference.Volts());
    }

    /// <inheritdoc/>
    public virtual IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
    {
        throw new PlatformNotSupportedException("This platform does not support analog inputs.  Use an IO Extender.");
    }

    /// <inheritdoc/>
    public virtual IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool invert = false)
    {
        throw new PlatformNotSupportedException("This platform does not support PWMs.  Use an IO Extender.");
    }

    /// <inheritdoc/>
    public virtual II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
    {
        // verify pins are I2C capable
        var clockChannel = clock.SupportedChannels
            ?.OfType<II2cChannelInfo>()
            .Where(c => c.ChannelFunction == I2cChannelFunctionType.Clock)
            .FirstOrDefault();

        if (clockChannel == null)
        {
            throw new ArgumentException($"Pin {clock.Name} does not support I2C Clock");
        }

        var dataChannel = data.SupportedChannels
            ?.OfType<II2cChannelInfo>()
            .Where(c => c.ChannelFunction == I2cChannelFunctionType.Data)
            .FirstOrDefault();

        if (dataChannel == null)
        {
            throw new ArgumentException($"Pin {data.Name} does not support I2C Data");
        }

        if (clockChannel.BusNumber != dataChannel.BusNumber)
        {
            throw new ArgumentException($"Pins {data.Name} and {clock.Name} are on different I2C buses");
        }

        return CreateI2cBus(clockChannel.BusNumber, busSpeed);
    }

    /// <inheritdoc/>
    public BatteryInfo GetBatteryInfo()
    {
        return new BatteryInfo
        {
            Voltage = Voltage.Zero
        };
    }

    internal static string ExecuteCommandLine(string command, string args)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);

        process?.WaitForExit();

        return process?.StandardOutput.ReadToEnd() ?? string.Empty;
    }

    // ----------------------------------------------
    // ----------------------------------------------
    // ----- BELOW HERE ARE NOT YET IMPLEMENTED -----
    // ----------------------------------------------
    // ----------------------------------------------

    /// <inheritdoc/>
    public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState = false, InterruptMode interruptMode = InterruptMode.None, ResistorMode resistorMode = ResistorMode.Disabled, PortDirectionType initialDirection = PortDirectionType.Input, double debounceDuration = 0, double glitchDuration = 0, OutputType output = OutputType.PushPull)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void OnReset()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetClock(DateTime dateTime)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void WatchdogEnable(TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void WatchdogReset()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void OnSleep()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Reset()
    {
        // TODO: $ sudo reboot
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Sleep(TimeSpan duration)
    {
        // not supported on RasPi
        throw new PlatformNotSupportedException();
    }

    /// <inheritdoc/>
    public void OnShutdown(out bool complete, Exception? e = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void OnError(Exception e, out bool recovered)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void OnResume()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void OnRecovery(Exception e)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void OnUpdate(Version newVersion, out bool approveUpdate)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void OnUpdateComplete(Version oldVersion, out bool rollbackUpdate)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ICounter CreateCounter(IPin pin, InterruptMode edge)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IAnalogInputArray CreateAnalogInputArray(params IPin[] pins)
    {
        throw new NotImplementedException();
    }
}
