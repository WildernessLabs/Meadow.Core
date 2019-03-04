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

        internal IGpioController GpioController { get; private set; }

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

            this.GpioController = new F7GPIOManager();
            this.GpioController.Initialize();

            // 
            this.Pins = new F7MicroPinDefinitions();
        }

        public IDigitalOutputPort CreateDigitalOutputPort(
            IPin pin, 
            bool initialState = false)
        {
            return DigitalOutputPort.From(pin, this.GpioController, initialState);
        }

        public IDigitalInputPort CreateDigitalInputPort(
            IPin pin,
            bool interruptEnabled = true,
            bool glitchFilter = false,
            ResistorMode resistorMode = ResistorMode.Disabled
            )
        {
            return DigitalInputPort.From(pin, this.GpioController, interruptEnabled, glitchFilter, resistorMode);
        }
    }


}
