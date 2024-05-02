using Meadow.Devices;
using Meadow.Foundation.Displays;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Units;
using System;

namespace Meadow;

public class Mac : IMeadowDevice, IPixelDisplayProvider
{
    private readonly Lazy<NativeNetworkAdapterCollection> _networkAdapters;

    /// <inheritdoc/>
    public IPlatformOS PlatformOS { get; }
    /// <inheritdoc/>
    public DeviceCapabilities Capabilities { get; private set; }
    /// <inheritdoc/>
    public IDeviceInformation Information { get; private set; }
    /// <inheritdoc/>
    public INetworkAdapterCollection NetworkAdapters => _networkAdapters.Value;

    public Mac()
    {
        PlatformOS = new MacPlatformOS();
        _networkAdapters = new Lazy<NativeNetworkAdapterCollection>(
            new NativeNetworkAdapterCollection());
    }

    /// <inheritdoc/>
    public void Initialize(MeadowPlatform detectedPlatform)
    {
        // TODO: populate actual capabilities
        Capabilities = new DeviceCapabilities(
            new AnalogCapabilities(false, null),
            new NetworkCapabilities(false, false),
            new StorageCapabilities(false));

        // TODO: populate this with appropriate data
        Information = new MacDeviceInformation();
    }

    /// <inheritdoc/>
    public IResizablePixelDisplay CreateDisplay(int? width = null, int? height = null)
    {
        return new SilkDisplay(width ?? 320, height ?? 240);
    }

    /// <inheritdoc/>
    public II2cBus CreateI2cBus(int busNumber = 0)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public II2cBus CreateI2cBus(int busNumber, I2cBusSpeed busSpeed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public II2cBus CreateI2cBus(IPin[] pins, I2cBusSpeed busSpeed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, SpiClockConfiguration config)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, Frequency speed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public IDigitalInputPort CreateDigitalInputPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }


    /// <inheritdoc/>
    public ISerialPort CreateSerialPort(string portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 1024)
    {
        if (PlatformOS.GetSerialPortName(portName) is { } name)
        {
            return CreateSerialPort(name, baudRate, dataBits, parity, stopBits, readBufferSize);
        }

        throw new ArgumentException($"Port name '{portName}' not found");
    }

    /// <inheritdoc/>
    public ISerialPort CreateSerialPort(SerialPortName portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 1024)
    {
        return new MacSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
    }

    /// <inheritdoc/>
    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] suffixDelimiter, bool preserveDelimiter, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
    {
        var port = CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        return SerialMessagePort.From(port, suffixDelimiter, preserveDelimiter);
    }

    /// <inheritdoc/>
    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] prefixDelimiter, bool preserveDelimiter, int messageLength, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
    {
        var port = CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        return SerialMessagePort.From(port, prefixDelimiter, preserveDelimiter, messageLength);
    }






    // TODO: implement everything below here


    /// <inheritdoc/>
    public event NetworkConnectionHandler NetworkConnected = default!;
    /// <inheritdoc/>
    public event NetworkDisconnectionHandler NetworkDisconnected = default!;


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
    public IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool invert = false)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public BatteryInfo GetBatteryInfo()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IPin GetPin(string name)
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
    public IDigitalInputPort CreateDigitalInputPort(IPin pin, ResistorMode resistorMode)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IAnalogInputArray CreateAnalogInputArray(params IPin[] pins)
    {
        throw new NotImplementedException();
    }
}
