using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using Meadow.Units;
using System;

namespace Meadow;

public class Desktop : IMeadowDevice
{
    private IMeadowDevice _implementation;

    public Desktop()
    {
    }

    public void Initialize(MeadowPlatform detectedPlatform)
    {
        _implementation = detectedPlatform switch
        {
            MeadowPlatform.Windows => new Windows(),
            MeadowPlatform.OSX => new Mac(),
            MeadowPlatform.DesktopLinux => new Linux(),

            _ => throw new ArgumentException($"Desktop cannot run on {detectedPlatform}"),
        };

        _implementation.Initialize(detectedPlatform);
    }

    public IPlatformOS PlatformOS => _implementation.PlatformOS;
    public IDeviceInformation Information => _implementation.Information;
    public DeviceCapabilities Capabilities => _implementation.Capabilities;
    public INetworkAdapterCollection NetworkAdapters => _implementation.NetworkAdapters;

    public IGraphicsDisplay Display { get; }

    /////////////////////////////////////////////////
    /////////////////////////////////////////////////
    /////////////////////////////////////////////////
    public event NetworkConnectionHandler NetworkConnected;
    public event NetworkDisconnectionHandler NetworkDisconnected;

    public IPin GetPin(string name)
    {
        throw new NotImplementedException();
    }

    public BatteryInfo? GetBatteryInfo()
    {
        throw new NotImplementedException();
    }

    public IDigitalInputPort CreateDigitalInputPort(IPin pin, ResistorMode resistorMode)
    {
        throw new NotImplementedException();
    }

    public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
    {
        throw new NotImplementedException();
    }

    public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
    {
        throw new NotImplementedException();
    }

    public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState)
    {
        throw new NotImplementedException();
    }

    public IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
    {
        throw new NotImplementedException();
    }

    public IAnalogInputArray CreateAnalogInputArray(params IPin[] pins)
    {
        throw new NotImplementedException();
    }

    public IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool invert = false)
    {
        throw new NotImplementedException();
    }

    public ISerialPort CreateSerialPort(SerialPortName portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 1024)
    {
        throw new NotImplementedException();
    }

    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] suffixDelimiter, bool preserveDelimiter, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
    {
        throw new NotImplementedException();
    }

    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] prefixDelimiter, bool preserveDelimiter, int messageLength, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
    {
        throw new NotImplementedException();
    }

    public ISpiBus CreateSpiBus(IPin clock, IPin copi, IPin cipo, SpiClockConfiguration config)
    {
        throw new NotImplementedException();
    }

    public ISpiBus CreateSpiBus(IPin clock, IPin copi, IPin cipo, Frequency speed)
    {
        throw new NotImplementedException();
    }

    public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
    {
        throw new NotImplementedException();
    }

    public II2cBus CreateI2cBus(int busNumber = 1, I2cBusSpeed busSpeed = I2cBusSpeed.Standard)
    {
        throw new NotImplementedException();
    }

    public II2cBus CreateI2cBus(IPin[] pins, I2cBusSpeed busSpeed)
    {
        throw new NotImplementedException();
    }

    public II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
    {
        throw new NotImplementedException();
    }

    public void WatchdogEnable(TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    public void WatchdogReset()
    {
        throw new NotImplementedException();
    }

    public ICounter CreateCounter(IPin pin, InterruptMode edge)
    {
        throw new NotImplementedException();
    }
}



