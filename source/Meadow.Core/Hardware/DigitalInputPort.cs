using System;
using System.Linq;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of reading digital input.
    /// </summary>
    public class DigitalInputPort : DigitalInputPortBase
    {
        protected IGpioController GpioController { get; set; }

        public bool GlitchFilter { get; set; }
        public ResistorMode Resistor { get; set; }

        protected DigitalInputPort(
            IPin pin,
            IGpioController gpioController,
            IDigitalChannelInfo channel,
            bool interruptEnabled = true,
            bool glitchFilter = false,
            ResistorMode resistorMode = ResistorMode.Disabled
            ) : base(pin, channel, interruptEnabled )
        {
            //// attempt to reserve the pin
            //var result = DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.DigitalInput);

            //if(result.Item1)
            //{
            //    this._pin = pin;

            //    // make sure the pin is configured as a digital input
            //    _pin.GPIOManager.ConfigureInput(_pin, glitchFilter, resistorMode, false);
            //}
            //else
            //{
            //    throw new PortInUseException();
            //}
        }

        public static DigitalInputPort From(
            IPin pin,
            IGpioController gpioController,
            bool interruptEnabled = true,
            bool glitchFilter = false,
            ResistorMode resistorMode = ResistorMode.Disabled
            )
        {
            var chan = pin.SupportedChannels.OfType<IDigitalChannelInfo>().First();
            if (chan != null) {
                //TODO: need other checks here.
                if(interruptEnabled && (!chan.InterrruptCapable)) {
                    throw new Exception("Unable to create input; channel is not capable of interrupts");
                }
                return new DigitalInputPort(pin, gpioController, chan, interruptEnabled, glitchFilter, resistorMode);
            } else {
                throw new Exception("Unable to create an output port on the pin, because it doesn't have a digital channel");
            }
        }

        public override void Dispose()
        {
            //TODO: implement full pattern
        }

        public override bool State
        {
            //get => _pin.GPIOManager.GetDiscrete(_pin);
            get => false;
            //protected set { throw new Exception(); }
        }

    }
}