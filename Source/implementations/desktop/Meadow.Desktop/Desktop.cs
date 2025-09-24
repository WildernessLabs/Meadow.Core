using Meadow;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Pinouts;
using Meadow.Units;
using System;

/// <summary>
/// Represents a desktop implementation of the Meadow device.
/// </summary>
public class Desktop : IMeadowDevice
{
    /// <summary>
    /// Gets or sets the underlying Meadow device implementation.
    /// </summary>
    protected IMeadowDevice Implementation { get; set; } = default!;

    private IResizablePixelDisplay? _display;

    /// <inheritdoc/>
    public event NetworkConnectionHandler? NetworkConnected;
    /// <inheritdoc/>
    public event NetworkDisconnectionHandler? NetworkDisconnected;

    /// <summary>
    /// Gets or sets the display associated with the desktop.
    /// </summary>
    public virtual IPixelDisplay? Display
    {
        get
        {
            if (Implementation is IPixelDisplayProvider displayProvider)
            {
                return _display ??= displayProvider.CreateDisplay();
            }

            return null;
        }
    }

    /// <summary>
    /// Initializes a new instance of the Desktop class.
    /// </summary>
    public Desktop()
    {
    }

    /// <inheritdoc/>
    public void Initialize(MeadowPlatform detectedPlatform)
    {
        Implementation = detectedPlatform switch
        {
            MeadowPlatform.OSX => new Mac(),
            MeadowPlatform.DesktopLinux => new DesktopLinux(),
            MeadowPlatform.Windows => new Windows(),
            _ => throw new ArgumentException($"Desktop cannot run on {detectedPlatform}"),
        };

        Implementation.Initialize(detectedPlatform);

        Implementation.NetworkConnected += (s, e) => NetworkConnected?.Invoke(s, e);
        Implementation.NetworkDisconnected += (s, e) => NetworkDisconnected?.Invoke(s, e);
    }

    /// <inheritdoc/>
    public IPlatformOS PlatformOS => Implementation.PlatformOS;
    /// <inheritdoc/>
    public IDeviceInformation Information => Implementation.Information;
    /// <inheritdoc/>
    public DeviceCapabilities Capabilities => Implementation.Capabilities;
    /// <inheritdoc/>
    public INetworkAdapterCollection NetworkAdapters => Implementation.NetworkAdapters;
    /// <inheritdoc/>
    public IPin GetPin(string name) => Implementation.GetPin(name);
    /// <inheritdoc/>
    public BatteryInfo? GetBatteryInfo() => Implementation.GetBatteryInfo();
    /// <inheritdoc/>
    public IDigitalInputPort CreateDigitalInputPort(IPin pin, ResistorMode resistorMode)
        => Implementation.CreateDigitalInputPort(pin, resistorMode);
    /// <inheritdoc/>
    public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
        => Implementation.CreateDigitalInterruptPort(pin, interruptMode, resistorMode, debounceDuration, glitchDuration);
    /// <inheritdoc/>
    public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
        => Implementation.CreateBiDirectionalInterruptPort(pin, initialState, interruptMode, resistorMode, initialDirection, debounceDuration, glitchDuration, output);
    /// <inheritdoc/>
    public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState)
        => Implementation.CreateBiDirectionalPort(pin, initialState);
    /// <inheritdoc/>
    public IAnalogInputPort CreateAnalogInputPort(IPin pin, Voltage? voltageReference)
        => Implementation.CreateAnalogInputPort(pin, voltageReference);
    /// <inheritdoc/>
    public IObservableAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
        => Implementation.CreateAnalogInputPort(pin, sampleCount, sampleInterval, voltageReference);
    /// <inheritdoc/>
    public IAnalogInputArray CreateAnalogInputArray(params IPin[] pins)
        => Implementation.CreateAnalogInputArray(pins);
    /// <inheritdoc/>
    public IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool invert = false)
        => Implementation.CreatePwmPort(pin, frequency, dutyCycle, invert);
    /// <inheritdoc/>
    public ISerialPort CreateSerialPort(SerialPortName portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 1024)
        => Implementation.CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
    /// <inheritdoc/>
    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] suffixDelimiter, bool preserveDelimiter, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
        => Implementation.CreateSerialMessagePort(portName, suffixDelimiter, preserveDelimiter, baudRate, dataBits, parity, stopBits, readBufferSize);
    /// <inheritdoc/>
    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] prefixDelimiter, bool preserveDelimiter, int messageLength, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
        => Implementation.CreateSerialMessagePort(portName, prefixDelimiter, preserveDelimiter, messageLength, baudRate, dataBits, parity, stopBits, readBufferSize);
    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(int busNumber, Frequency speed)
        => Implementation.CreateSpiBus(busNumber, speed);
    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(IPin clock, IPin copi, IPin cipo, SpiClockConfiguration config)
        => Implementation.CreateSpiBus(clock, copi, cipo, config);
    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(IPin clock, IPin copi, IPin cipo, Frequency speed)
        => Implementation.CreateSpiBus(clock, copi, cipo, speed);
    /// <inheritdoc/>
    public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
        => Implementation.CreateDigitalOutputPort(pin, initialState, initialOutputType);
    /// <inheritdoc/>
    public II2cBus CreateI2cBus(int busNumber = 1, I2cBusSpeed busSpeed = I2cBusSpeed.Standard)
        => Implementation.CreateI2cBus(busNumber, busSpeed);
    /// <inheritdoc/>
    public II2cBus CreateI2cBus(IPin[] pins, I2cBusSpeed busSpeed)
        => Implementation.CreateI2cBus(pins, busSpeed);
    /// <inheritdoc/>
    public II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
        => Implementation.CreateI2cBus(clock, data, busSpeed);
    /// <inheritdoc/>
    public void WatchdogEnable(TimeSpan timeout)
        => Implementation.WatchdogEnable(timeout);
    /// <inheritdoc/>
    public void WatchdogReset()
        => Implementation.WatchdogReset();
    /// <inheritdoc/>
    public ICounter CreateCounter(IPin pin, InterruptMode edge)
        => Implementation.CreateCounter(pin, edge);
    /// <inheritdoc/>
    public IDigitalSignalAnalyzer CreateDigitalSignalAnalyzer(IPin pin, bool captureDutyCycle)
        => Implementation.CreateDigitalSignalAnalyzer(pin, captureDutyCycle);
}