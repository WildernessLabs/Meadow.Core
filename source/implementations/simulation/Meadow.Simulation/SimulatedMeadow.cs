using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Units;
using System;
using System.Linq;

namespace Meadow.Simulation
{
    public partial class SimulatedMeadow<TPinDefinitions> : ISimulatedDevice<TPinDefinitions>
        where TPinDefinitions : IPinDefinitions, new()
    {
        private readonly SimulationEngine<TPinDefinitions> _simulationEngine;
        private readonly IPlatformOS _platformOS;

        /// <inheritdoc/>
        public event PowerTransitionHandler BeforeReset;
        /// <inheritdoc/>
        public event PowerTransitionHandler BeforeSleep;
        /// <inheritdoc/>
        public event PowerTransitionHandler AfterWake;
        /// <inheritdoc/>
        public event NetworkConnectionHandler NetworkConnected;
        /// <inheritdoc/>
        public event NetworkDisconnectionHandler NetworkDisconnected;

        public Logger Logger { get; }

        public SimulatedMeadow()
        {
            Logger = new Logger(new ConsoleLogProvider());
            Logger.LogLevel = LogLevel.Information;

            Pins = new TPinDefinitions();
            _platformOS = new SimulatedPlatformOS();
            _simulationEngine = new SimulationEngine<TPinDefinitions>(this, Logger);
            Information = new SimulationInformation();
        }

        public TPinDefinitions Pins { get; }
        public IDeviceInformation Information { get; }

        public IPlatformOS PlatformOS => _platformOS;
        public DeviceCapabilities Capabilities { get; private set; } = new DeviceCapabilities(
                new AnalogCapabilities(false, null),
                new NetworkCapabilities(false, true),
                new StorageCapabilities(true)
                );

        public INetworkAdapterCollection NetworkAdapters { get; private set; } = new NativeNetworkAdapterCollection();

        private void LaunchUI()
        {

        }

        public void DrivePinVoltage(IPin pin, Voltage voltage)
        {
            _simulationEngine.SetPinVoltage(pin, voltage);
        }

        public void DrivePinState(IPin pin, bool state)
        {
            _simulationEngine.SetDiscrete(pin, state);
        }

        public bool ReadPinState(IPin pin)
        {
            return _simulationEngine.GetDiscrete(pin);
        }

        public IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Meadow.Units.Voltage voltageReference)
        {
            var dc = pin.SupportedChannels?.FirstOrDefault(i => i is IAnalogChannelInfo) as AnalogChannelInfo;
            if (dc != null)
            {
                return new SimulatedAnalogInputPort(
                    pin as SimulatedPin ?? throw new ArgumentException("pin must be a SimulatedPin"),
                    dc, sampleCount, sampleInterval, voltageReference);
            }

            throw new NotSupportedException();
        }

        public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
        {
            var dc = pin.SupportedChannels?.FirstOrDefault(i => i is IDigitalChannelInfo) as DigitalChannelInfo;
            if (dc != null)
            {
                return new SimulatedBiDirectionalPort(pin, dc, initialState, resistorMode, initialDirection);
            }

            throw new NotSupportedException();
        }

        public IDigitalInputPort CreateDigitalInputPort(IPin pin)
        {
            return CreateDigitalInputPort(pin, ResistorMode.Disabled);
        }

        public IDigitalInputPort CreateDigitalInputPort(IPin pin, ResistorMode resistorMode)
        {
            var dci = pin.SupportedChannels?.FirstOrDefault(i => i is IDigitalChannelInfo) as DigitalChannelInfo;
            if (dci != null)
            {
                return new SimulatedDigitalInputPort(
                    pin as SimulatedPin ?? throw new ArgumentException("pin must be a SimulatedPin"),
                    dci);
            }

            throw new NotSupportedException();
        }

        public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
        {
            var dco = pin.SupportedChannels?.FirstOrDefault(i => i is IDigitalChannelInfo) as DigitalChannelInfo;
            if (dco != null)
            {
                var p = pin as SimulatedPin;
                p.VoltageChanged += (s, e) =>
                    {
                        _simulationEngine.SetPinVoltage(pin, p.Voltage);
                    };

                return new SimulatedDigitalOutputPort(pin, dco, false, OutputType.PushPull);
            }

            throw new NotSupportedException();
        }

        public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] suffixDelimiter, bool preserveDelimiter, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
        {
            return SerialMessagePort.From(
                new SerialPortProxy(portName, baudRate, dataBits, parity, stopBits, readBufferSize),
                suffixDelimiter,
                preserveDelimiter);
        }

        public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] prefixDelimiter, bool preserveDelimiter, int messageLength, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
        {
            return SerialMessagePort.From(
                new SerialPortProxy(portName, baudRate, dataBits, parity, stopBits, readBufferSize),
                prefixDelimiter,
                preserveDelimiter,
                messageLength);
        }

        public ISerialPort CreateSerialPort(SerialPortName portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 1024)
        {
            return new SerialPortProxy(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        }

        public IPin GetPin(string name)
        {
            return Pins[name];
        }


        // ========= not implemented below here =========

        public II2cBus CreateI2cBus(int busNumber = 0)
        {
            throw new NotImplementedException();
        }

        public IPwmPort CreatePwmPort(IPin pin, float frequency = 100, float dutyCycle = 0.5F, bool invert = false)
        {
            throw new NotImplementedException();
        }

        public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, SpiClockConfiguration config)
        {
            throw new NotImplementedException();
        }

        public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, Meadow.Units.Frequency speed)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
        }

        public void Reset()
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

        public void Sleep(int seconds = -1)
        {
            throw new NotImplementedException();
        }

        public BatteryInfo GetBatteryInfo()
        {
            throw new NotImplementedException();
        }

        public Temperature GetProcessorTemperature()
        {
            throw new NotImplementedException();
        }

        public IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool invert = false)
        {
            throw new NotImplementedException();
        }

        public ICounter CreateCounter(IPin pin, InterruptMode edge)
        {
            throw new NotImplementedException();
        }

        public II2cBus CreateI2cBus(int busNumber, I2cBusSpeed busSpeed)
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

        public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode)
        {
            return CreateDigitalInterruptPort(
                pin,
                interruptMode,
                ResistorMode.Disabled,
                TimeSpan.Zero,
                TimeSpan.Zero);
        }

        public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
        {
            throw new NotImplementedException();
        }

        public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(IPin pin)
        {
            return CreateBiDirectionalInterruptPort(
                pin,
                false,
                InterruptMode.EdgeRising,
                ResistorMode.InternalPullDown,
                PortDirectionType.Input,
                TimeSpan.Zero,
                TimeSpan.Zero);
        }

        public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
        {
            throw new NotImplementedException();
        }

        public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState = false)
        {
            throw new NotImplementedException();
        }

        public IAnalogInputArray CreateAnalogInputArray(params IPin[] pins)
        {
            throw new NotImplementedException();
        }
    }
}
