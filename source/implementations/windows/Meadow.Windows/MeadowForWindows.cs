using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Units;
using System;

namespace Meadow
{
    public class MeadowForWindows<TIoProvider, TPinDefinitions> : IMeadowDevice
        where TIoProvider : IIoDevice, new()
        where TPinDefinitions : IPinDefinitions, new()
    {
        private TIoProvider _ioProvider;

        public Logger Logger { get; }
        public TPinDefinitions Pins { get; }

        public IPlatformOS PlatformOS { get; }
        public DeviceCapabilities Capabilities { get; private set; }
        public IDeviceInformation Information { get; private set; }

        public MeadowForWindows()
        {
            Logger = new Logger(new ConsoleLogProvider());
            Logger.Loglevel = LogLevel.Information;

            Pins = new TPinDefinitions();
            _ioProvider = new TIoProvider();
            PlatformOS = new WindowsPlatformOS();
        }

        public void Initialize()
        {
            // TODO: populate actual capabilities
            Capabilities = new DeviceCapabilities(
                new AnalogCapabilities(false, null),
                new NetworkCapabilities(false, false),
                new StorageCapabilities(false));

            // TODO: populate this with appropriate data
            Information = new WindowsDeviceInformation();
        }

        public II2cBus CreateI2cBus(int busNumber = 0)
        {
            return _ioProvider.CreateI2cBus(busNumber);
        }










        // TODO: implement everything below here


        public INetworkAdapterCollection NetworkAdapters => throw new NotImplementedException();

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
}
