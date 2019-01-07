using System;

namespace Meadow.Hardware
{
    public class DigitalInputPort : DigitalInputPortBase
    {
        protected IDigitalPin _pin;
        protected bool _disposed;

        public bool GlitchFilter { get; set; }
        public ResistorMode Resistor { get; set; }

        public DigitalInputPort(IDigitalPin pin, bool glitchFilter = false, ResistorMode resistorMode = ResistorMode.Disabled)
        {
            // attempt to reserve the pin
            var result = DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.DigitalInput);

            if(result.Item1)
            {
                this._pin = pin;

                // make sure the pin is configured as a digital output
                _pin.GPIOManager.Configure(_pin, glitchFilter, resistorMode);
            }
            else
            {
                throw new PortInUseException();
            }
        }

        public override bool Value
        {
            get => _pin.GPIOManager.GetDiscrete(_pin);
        }
    }
}