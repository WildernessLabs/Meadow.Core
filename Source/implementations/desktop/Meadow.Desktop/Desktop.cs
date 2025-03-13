using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Pinouts;
using Meadow.Units;
using System;

namespace Meadow;

/// <summary>
/// Represents a desktop implementation of the Meadow device.
/// </summary>
public class Desktop : IMeadowDevice
{
    private IMeadowDevice _implementation = default!;
    private IResizablePixelDisplay? _display;

    /// <inheritdoc/>
    public event NetworkConnectionHandler? NetworkConnected;
    /// <inheritdoc/>
    public event NetworkDisconnectionHandler? NetworkDisconnected;

    /// <summary>
    /// Gets or sets the display associated with the desktop.
    /// </summary>
    public virtual IResizablePixelDisplay? Display
    {
        get
        {
            if (_implementation is IPixelDisplayProvider displayProvider)
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
        _implementation = detectedPlatform switch
        {
            MeadowPlatform.OSX => new Mac(),
            MeadowPlatform.DesktopLinux => new DesktopLinux(),
            MeadowPlatform.Windows => new Windows(),
            _ => throw new ArgumentException($"Desktop cannot run on {detectedPlatform}"),
        };

        _implementation.Initialize(detectedPlatform);

        _implementation.NetworkConnected += (s, e) => NetworkConnected?.Invoke(s, e);
        _implementation.NetworkDisconnected += (s, e) => NetworkDisconnected?.Invoke(s, e);
    }

    /// <inheritdoc/>
    public IPlatformOS PlatformOS => _implementation.PlatformOS;
    /// <inheritdoc/>
    public IDeviceInformation Information => _implementation.Information;
    /// <inheritdoc/>
    public DeviceCapabilities Capabilities => _implementation.Capabilities;
    /// <inheritdoc/>
    public INetworkAdapterCollection NetworkAdapters => _implementation.NetworkAdapters;
    /// <inheritdoc/>
    public IPin GetPin(string name) => _implementation.GetPin(name);
    /// <inheritdoc/>
    public BatteryInfo? GetBatteryInfo() => _implementation.GetBatteryInfo();
    /// <inheritdoc/>
    public IDigitalInputPort CreateDigitalInputPort(IPin pin, ResistorMode resistorMode)
        => _implementation.CreateDigitalInputPort(pin, resistorMode);
    /// <inheritdoc/>
    public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
        => _implementation.CreateDigitalInterruptPort(pin, interruptMode, resistorMode, debounceDuration, glitchDuration);
    /// <inheritdoc/>
    public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
        => _implementation.CreateBiDirectionalInterruptPort(pin, initialState, interruptMode, resistorMode, initialDirection, debounceDuration, glitchDuration, output);
    /// <inheritdoc/>
    public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState)
        => _implementation.CreateBiDirectionalPort(pin, initialState);
    /// <inheritdoc/>
    public IObservableAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
        => _implementation.CreateAnalogInputPort(pin, sampleCount, sampleInterval, voltageReference);
    /// <inheritdoc/>
    public IAnalogInputArray CreateAnalogInputArray(params IPin[] pins)
        => _implementation.CreateAnalogInputArray(pins);
    /// <inheritdoc/>
    public IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool invert = false)
        => _implementation.CreatePwmPort(pin, frequency, dutyCycle, invert);
    /// <inheritdoc/>
    public ISerialPort CreateSerialPort(SerialPortName portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 1024)
        => _implementation.CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
    /// <inheritdoc/>
    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] suffixDelimiter, bool preserveDelimiter, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
        => _implementation.CreateSerialMessagePort(portName, suffixDelimiter, preserveDelimiter, baudRate, dataBits, parity, stopBits, readBufferSize);
    /// <inheritdoc/>
    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] prefixDelimiter, bool preserveDelimiter, int messageLength, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
        => _implementation.CreateSerialMessagePort(portName, prefixDelimiter, preserveDelimiter, messageLength, baudRate, dataBits, parity, stopBits, readBufferSize);
    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(int busNumber, Frequency speed)
        => _implementation.CreateSpiBus(busNumber, speed);
    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(IPin clock, IPin copi, IPin cipo, SpiClockConfiguration config)
        => _implementation.CreateSpiBus(clock, copi, cipo, config);
    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(IPin clock, IPin copi, IPin cipo, Frequency speed)
        => _implementation.CreateSpiBus(clock, copi, cipo, speed);
    /// <inheritdoc/>
    public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
        => _implementation.CreateDigitalOutputPort(pin, initialState, initialOutputType);
    /// <inheritdoc/>
    public II2cBus CreateI2cBus(int busNumber = 1, I2cBusSpeed busSpeed = I2cBusSpeed.Standard)
        => _implementation.CreateI2cBus(busNumber, busSpeed);
    /// <inheritdoc/>
    public II2cBus CreateI2cBus(IPin[] pins, I2cBusSpeed busSpeed)
        => _implementation.CreateI2cBus(pins, busSpeed);
    /// <inheritdoc/>
    public II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
        => _implementation.CreateI2cBus(clock, data, busSpeed);
    /// <inheritdoc/>
    public void WatchdogEnable(TimeSpan timeout)
        => _implementation.WatchdogEnable(timeout);
    /// <inheritdoc/>
    public void WatchdogReset()
        => _implementation.WatchdogReset();
    /// <inheritdoc/>
    public ICounter CreateCounter(IPin pin, InterruptMode edge)
        => _implementation.CreateCounter(pin, edge);
    /// <inheritdoc/>
    public IDigitalSignalAnalyzer CreateDigitalSignalAnalyzer(IPin pin, bool captureDutyCycle)
        => _implementation.CreateDigitalSignalAnalyzer(pin, captureDutyCycle);
}



