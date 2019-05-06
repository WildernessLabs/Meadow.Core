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

            this.Hubs = new F7CommHubs();
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
            InterruptMode interruptMode = InterruptMode.EdgeBoth,
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
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input)
        {
            return BiDirectionalPort.From(pin, this.IoController, initialState, glitchFilter, resistorMode, initialDirection);
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
            NamedPinGroup pins
        )
        {
            
        }

        public ISpiBus CreateSpiBus(
            IPin clock,
            IPin mosi,
            IPin miso
        );

    }
}
