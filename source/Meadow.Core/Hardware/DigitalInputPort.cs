using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of reading digital input.
    /// </summary>
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

                // make sure the pin is configured as a digital input
                _pin.GPIOManager.ConfigureInput(_pin, glitchFilter, resistorMode, false);
            }
            else
            {
                throw new PortInUseException();
            }
        }

        public override bool State
        {
            get => _pin.GPIOManager.GetDiscrete(_pin);
        }
    }
}