using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Units;
using Meadow.Update;

Resolver.Services.Add(new Logger(new ConsoleLogProvider()));
Resolver.Services.Add<IMeadowDevice>(new PCDevice());

Resolver.Log.Loglevel = LogLevel.Trace;

Resolver.Log.Info("Updater Test Harness");

var svc = new UpdateService(new UpdateSettings());
Resolver.Services.Add<IUpdateService>(svc);

svc.ClearUpdates(); // uncomment to clear persisted info
svc.Start();

svc.OnUpdateAvailable += (updateService, info) =>
{
    Resolver.Log.Info("Update available!");

    // queue it for retreival "later"
    Task.Run(async () =>
    {
        await Task.Delay(5000);
        updateService.RetrieveUpdate(info);
    });
};

svc.OnUpdateRetrieved += (updateService, info) =>
{
    Resolver.Log.Info("Update retrieved!");

    Task.Run(async () =>
    {
        await Task.Delay(5000);
        updateService.ApplyUpdate(info);
    });
};

while (true)
{
    await Task.Delay(1000);
}

class PCDevice : IMeadowDevice
{
    public INetworkAdapterCollection NetworkAdapters { get; }
    public IDeviceInformation Information { get; }

    public PCDevice()
    {
        Information = new PCInfo();

        var adapters = new Meadow.NetworkAdapterCollection();
        adapters.Add(new WiredNetworkAdapter());
        NetworkAdapters = adapters;
    }

    public class PCInfo : IDeviceInformation
    {
        public string DeviceName { get; set; } = "Unknown";
        public string Model => "Unknown";
        public MeadowPlatform Platform => MeadowPlatform.Unknown;
        public string ProcessorType => "Unknown";
        public string ProcessorSerialNumber => "Unknown";
        public string UniqueID => "Unknown";
        public string CoprocessorType => "Unknown";
        public string? CoprocessorOSVersion => "Unknown";
        public string OSVersion => "Unknown";
    }

    // -----------------------------------------------
    public IPlatformOS PlatformOS => throw new NotImplementedException();

    public DeviceCapabilities Capabilities => throw new NotImplementedException();

    public event NetworkConnectionHandler NetworkConnected;
    public event NetworkDisconnectionHandler NetworkDisconnected;

    public IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
    {
        throw new NotImplementedException();
    }

    public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
    {
        throw new NotImplementedException();
    }

    public ICounter CreateCounter(IPin pin, InterruptMode edge)
    {
        throw new NotImplementedException();
    }

    public IDigitalInputPort CreateDigitalInputPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
    {
        throw new NotImplementedException();
    }

    public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
    {
        throw new NotImplementedException();
    }

    public II2cBus CreateI2cBus(int busNumber = 0)
    {
        throw new NotImplementedException();
    }

    public II2cBus CreateI2cBus(int busNumber, Frequency frequency)
    {
        throw new NotImplementedException();
    }

    public II2cBus CreateI2cBus(IPin[] pins, Frequency frequency)
    {
        throw new NotImplementedException();
    }

    public II2cBus CreateI2cBus(IPin clock, IPin data, Frequency frequency)
    {
        throw new NotImplementedException();
    }

    public IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool invert = false)
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

    public ISerialPort CreateSerialPort(SerialPortName portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 1024)
    {
        throw new NotImplementedException();
    }

    public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, SpiClockConfiguration config)
    {
        throw new NotImplementedException();
    }

    public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, Frequency speed)
    {
        throw new NotImplementedException();
    }

    public BatteryInfo GetBatteryInfo()
    {
        throw new NotImplementedException();
    }

    public IPin GetPin(string name)
    {
        throw new NotImplementedException();
    }

    public Temperature GetProcessorTemperature()
    {
        throw new NotImplementedException();
    }

    public void Initialize()
    {
        throw new NotImplementedException();
    }

    public void SetClock(DateTime dateTime)
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
}