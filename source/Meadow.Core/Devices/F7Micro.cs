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
        public new F7MicroPinDefinitions Pins { get; protected set; }
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
            bool interruptEnabled = true,
            bool glitchFilter = false,
            ResistorMode resistorMode = ResistorMode.Disabled
            )
        {
            return DigitalInputPort.From(pin, this.IoController, interruptEnabled, glitchFilter, resistorMode);
        }
    }


}
