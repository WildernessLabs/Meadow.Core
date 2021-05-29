using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace Meadow
{
    public class MeadowOnLinux<TPinout> : IMeadowDevice
        where TPinout : IPinDefinitions, new()
    {
        private SynchronizationContext? _context;

        public IPinDefinitions Pins { get; }
        public DeviceCapabilities Capabilities { get; }

        public MeadowOnLinux()
        {
            Pins = new TPinout();
            Capabilities = new DeviceCapabilities(
                new AnalogCapabilities(false, null),
                new NetworkCapabilities(false, true)
                );
        }

        public void Initialize()
        {
//            IoController.Initialize();
        }

        public II2cBus CreateI2cBus(int busNumber = 0)
        {
            return CreateI2cBus(busNumber, II2cController.DefaultI2cBusSpeed);
        }

        public II2cBus CreateI2cBus(int busNumber, Frequency frequency)
        {
            return new I2CBus(busNumber, frequency);
        }

        public II2cBus CreateI2cBus(IPin[] pins, Frequency frequency)
        {
            return CreateI2cBus(pins[0], pins[1], frequency);
        }

        public II2cBus CreateI2cBus(IPin clock, IPin data, Frequency frequency)
        {
            if (clock == Pins["PIN05"] && data == Pins["PIN03"])
            {
                return new I2CBus(1, frequency);
            }
            else if (clock == Pins["PIN28"] && data == Pins["PIN27"])
            {
                return new I2CBus(0, frequency);
            }

            throw new ArgumentOutOfRangeException("Requested pins are not I2C bus pins");
        }

        // TODO: this should move to the MeadowOS class.
        public void SetSynchronizationContext(SynchronizationContext context)
        {
            _context = context;
        }

        // TODO: this should move to the MeadowOS class.
        public void BeginInvokeOnMainThread(Action action)
        {
            if (_context == null) {
                action();
            } else {
                _context.Send(delegate { action(); }, null);
            }
        }

        // ----- BELOW HERE ARE NOT YET IMPLEMENTED -----

        public IAnalogInputPort CreateAnalogInputPort(IPin pin, float voltageReference = 3.3F)
        {
            throw new NotImplementedException();
        }

        public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState = false, InterruptMode interruptMode = InterruptMode.None, ResistorMode resistorMode = ResistorMode.Disabled, PortDirectionType initialDirection = PortDirectionType.Input, double debounceDuration = 0, double glitchDuration = 0, OutputType output = OutputType.PushPull)
        {
            throw new NotImplementedException();
        }

        public IDigitalInputPort CreateDigitalInputPort(IPin pin, InterruptMode interruptMode = InterruptMode.None, ResistorMode resistorMode = ResistorMode.Disabled, double debounceDuration = 0, double glitchDuration = 0)
        {
            throw new NotImplementedException();
        }

        public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
        {
            throw new NotImplementedException();
        }

        public IPwmPort CreatePwmPort(IPin pin, float frequency = 100, float dutyCycle = 0.5F, bool invert = false)
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

        public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, long speedkHz = 375)
        {
            throw new NotImplementedException();
        }

        public IPin GetPin(string pinName)
        {
            throw new NotImplementedException();
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
    }
}
