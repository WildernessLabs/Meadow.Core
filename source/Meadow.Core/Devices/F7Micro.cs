using System.Collections.Generic;
using Meadow.Hardware;
using Meadow.Gateway.WiFi;
using System;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Meadow F7 micro device. Includes device-specific IO mapping,
    /// capabilities and provides access to the various device-specific features.
    /// </summary>
    public partial class F7Micro : IIODevice
    {
        //public List<WiFiAdapter> WiFiAdapters { get; }

        public DeviceCapabilities Capabilities { get; protected set; }

        /// <summary>
        /// Gets the pins.
        /// </summary>
        /// <value>The pins.</value>
        public F7MicroPinDefinitions Pins { get; protected set; }
        //IPinDefinitions IDevice.Pins => throw new System.NotImplementedException();


        public SerialPortNameDefinitions SerialPortNames { get; protected set; }
            = new SerialPortNameDefinitions();


        internal IIOController IoController { get; private set; }

        // private static
        static F7Micro() { }

        public F7Micro()
        {
            this.Capabilities = new DeviceCapabilities(
                new AnalogCapabilities(true, 12),
                new NetworkCapabilities(true, true)
                );
            //this.WiFiAdapters = new List<WiFiAdapter>
            //{
            //    // TODO: stuff.
            //    new WiFiAdapter()
            //};

            this.IoController = new F7GPIOManager();
            this.IoController.Initialize();

            // 
            this.Pins = new F7MicroPinDefinitions();

        }


        //public C CreatePort<C>(P portConfig) where P : IPortConfig, where C : IPort {}

        public IDigitalOutputPort CreateDigitalOutputPort(
            IPin pin,
            bool initialState = false)
        {
            return DigitalOutputPort.From(pin, this.IoController, initialState);
        }

        public IDigitalInputPort CreateDigitalInputPort(
            IPin pin,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            int debounceDuration = 0,
            int glitchFilterCycleCount = 0
            )
        {
            return DigitalInputPort.From(pin, this.IoController, interruptMode, resistorMode, debounceDuration, glitchFilterCycleCount);
        }

        public IBiDirectionalPort CreateBiDirectionalPort(
            IPin pin,
            bool initialState = false,
            bool glitchFilter = false,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input)
        {
            return BiDirectionalPort.From(pin, this.IoController, initialState, glitchFilter, interruptMode, resistorMode, initialDirection);
        }

        public IAnalogInputPort CreateAnalogInputPort(
            IPin pin,
            float voltageReference = 3.3f)
        {
            return AnalogInputPort.From(pin, this.IoController, voltageReference);
        }

        public IPwmPort CreatePwmPort(
            IPin pin,
            float frequency = 100,
            float dutyCycle = 0.5f,
            bool inverted = false)
        {
            bool isOnboard = IsOnboardLed(pin);
            return PwmPort.From(pin, this.IoController, frequency, dutyCycle, inverted, isOnboard);
        }

        /// <summary>
        /// Tests whether or not the pin passed in belongs to an onboard LED
        /// component. Used for a dirty dirty hack.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns>whether or no the pin belons to the onboard LED</returns>
        protected bool IsOnboardLed(IPin pin)
        {
            return (
                pin == Pins.OnboardLedBlue ||
                pin == Pins.OnboardLedGreen ||
                pin == Pins.OnboardLedRed
                );
        }

        public ISerialPort CreateSerialPort(
            SerialPortName portName,
            int baudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096)
        {
            return SerialPort.From(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        }

        public ISpiBus CreateSpiBus(
            long speed = 375 // this will default to the minimum capable speed of 375kHz
        )
        {
            return CreateSpiBus(Pins.SCK, Pins.MOSI, Pins.MISO, speed);
        }

        public ISpiBus CreateSpiBus(
            IPin[] pins,
            long speed = 375// this will default to the minimum capable speed of 375kHz
        )
        {
            return CreateSpiBus(pins[0], pins[1], pins[2], speed);
        }

        public ISpiBus CreateSpiBus(
            IPin clock,
            IPin mosi,
            IPin miso,
            long speed = 375// this will default to the minimum capable speed of 375kHz
        )
        {
            var bus = SpiBus.From(clock, mosi, miso);
            bus.BusNumber = GetSpiBusNumberForPins(clock, mosi, miso);
            bus.Configuration.SpeedKHz = speed;
            return bus;
        }

        public ISpiBus CreateSpiBus(
            IPin clock,
            IPin mosi,
            IPin miso,
            SpiClockConfiguration config
        )
        {
            var bus = SpiBus.From(clock, mosi, miso);
            bus.BusNumber = GetSpiBusNumberForPins(clock, mosi, miso);
            bus.Configuration = config;
            return bus;
        }

        private int GetSpiBusNumberForPins(IPin clock, IPin mosi, IPin miso)
        {
            // we're only looking at clock pin.  
            // For the F7 meadow it's enough to know and any attempt to use other pins will get caught by other sanity checks
            if (clock == Pins.ESP_CLK)
            {
                return 2;
            }
            else if (clock == Pins.SCK)
            {
                return 3;
            }

            // this is an unsupported bus, but will get caught elsewhere
            return -1;
        }

        public II2cBus CreateI2cBus(
            ushort speed = 1000
        )
        {
            return CreateI2cBus(Pins.I2C_SCL, Pins.I2C_SDA, speed);
        }

        public II2cBus CreateI2cBus(
            IPin[] pins,
            ushort speed
        )
        {
            return CreateI2cBus(pins[0], pins[1], speed);
        }


        public II2cBus CreateI2cBus(
            IPin clock,
            IPin data,
            ushort speed
        )
        {
            return I2cBus.From(this.IoController, clock, data, speed);
        }

    }
}
