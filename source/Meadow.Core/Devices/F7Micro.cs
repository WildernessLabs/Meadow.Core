using System.Collections.Generic;
using Meadow.Hardware;
using Meadow.Gateway.WiFi;

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

        internal IIOController IoController { get; private set; }

        // private static
        static F7Micro() { }

        public F7Micro()
        {
            this.Capabilities = new DeviceCapabilities();
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
            float dutyCycle = 0
            /*bool inverted = false*/)
        {
            return PwmPort.From(pin, this.IoController, frequency, dutyCycle);
        }

        public ISpiBus CreateSpiBus(
            ushort speed = 1000
        )
        {
            return CreateSpiBus(Pins.SCK, Pins.MOSI, Pins.MISO, speed);
        }

        public ISpiBus CreateSpiBus(
            IPin[] pins, ushort speed = 1000
        )
        {
            return CreateSpiBus(pins[0], pins[1], pins[2], speed);
        }

        public ISpiBus CreateSpiBus(
            IPin clock,
            IPin mosi,
            IPin miso,
            ushort speed = 1000
        )
        {
            return SpiBus.From(clock, mosi, miso, speed);
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
