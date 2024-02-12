using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Units;
using System;

namespace Meadow;

public class Desktop : IMeadowDevice
{
    private IMeadowDevice _implementation;

    public event NetworkConnectionHandler NetworkConnected;
    public event NetworkDisconnectionHandler NetworkDisconnected;

    public Desktop()
    {
    }

    public void Initialize(MeadowPlatform detectedPlatform)
    {
        _implementation = detectedPlatform switch
        {
#if WINDOWS
            MeadowPlatform.Windows => new Windows(),
#endif
            MeadowPlatform.OSX => new Mac(),
            MeadowPlatform.DesktopLinux => new Linux(),

            _ => throw new ArgumentException($"Desktop cannot run on {detectedPlatform}"),
        };

        _implementation.Initialize(detectedPlatform);

        if (_implementation is IPixelDisplayProvider displayProvider)
        {
            Display = displayProvider.CreateDisplay();
        }

        _implementation.NetworkConnected += (s, e) => NetworkConnected?.Invoke(s, e);
        _implementation.NetworkDisconnected += (s) => NetworkDisconnected?.Invoke(s);
    }

    public virtual IPixelDisplay? Display { get; private set; }

    public IPlatformOS PlatformOS => _implementation.PlatformOS;
    public IDeviceInformation Information => _implementation.Information;
    public DeviceCapabilities Capabilities => _implementation.Capabilities;
    public INetworkAdapterCollection NetworkAdapters => _implementation.NetworkAdapters;
    public IPin GetPin(string name) => _implementation.GetPin(name);
    public BatteryInfo? GetBatteryInfo() => _implementation.GetBatteryInfo();
    public IDigitalInputPort CreateDigitalInputPort(IPin pin, ResistorMode resistorMode)
        => _implementation.CreateDigitalInputPort(pin, resistorMode);
    public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
        => _implementation.CreateDigitalInterruptPort(pin, interruptMode, resistorMode, debounceDuration, glitchDuration);
    public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
        => _implementation.CreateBiDirectionalInterruptPort(pin, initialState, interruptMode, resistorMode, initialDirection, debounceDuration, glitchDuration, output);
    public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState)
        => _implementation.CreateBiDirectionalPort(pin, initialState);
    public IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
        => _implementation.CreateAnalogInputPort(pin, sampleCount, sampleInterval, voltageReference);
    public IAnalogInputArray CreateAnalogInputArray(params IPin[] pins)
        => _implementation.CreateAnalogInputArray(pins);
    public IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool invert = false)
        => _implementation.CreatePwmPort(pin, frequency, dutyCycle, invert);
    public ISerialPort CreateSerialPort(SerialPortName portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 1024)
        => _implementation.CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] suffixDelimiter, bool preserveDelimiter, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
        => _implementation.CreateSerialMessagePort(portName, suffixDelimiter, preserveDelimiter, baudRate, dataBits, parity, stopBits, readBufferSize);
    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] prefixDelimiter, bool preserveDelimiter, int messageLength, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
        => _implementation.CreateSerialMessagePort(portName, prefixDelimiter, preserveDelimiter, messageLength, baudRate, dataBits, parity, stopBits, readBufferSize);
    public ISpiBus CreateSpiBus(IPin clock, IPin copi, IPin cipo, SpiClockConfiguration config)
        => _implementation.CreateSpiBus(clock, copi, cipo, config);
    public ISpiBus CreateSpiBus(IPin clock, IPin copi, IPin cipo, Frequency speed)
        => _implementation.CreateSpiBus(clock, copi, cipo, speed);
    public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
        => _implementation.CreateDigitalOutputPort(pin, initialState, initialOutputType);
    public II2cBus CreateI2cBus(int busNumber = 1, I2cBusSpeed busSpeed = I2cBusSpeed.Standard)
        => _implementation.CreateI2cBus(busNumber, busSpeed);
    public II2cBus CreateI2cBus(IPin[] pins, I2cBusSpeed busSpeed)
        => _implementation.CreateI2cBus(pins, busSpeed);
    public II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
        => _implementation.CreateI2cBus(clock, data, busSpeed);
    public void WatchdogEnable(TimeSpan timeout)
        => _implementation.WatchdogEnable(timeout);
    public void WatchdogReset()
        => _implementation.WatchdogReset();
    public ICounter CreateCounter(IPin pin, InterruptMode edge)
        => _implementation.CreateCounter(pin, edge);
}



