using System;
using System.Linq;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of reading and writing digital input
    /// and output.
    /// </summary>
    public class BiDirectionalPort : BiDirectionalPortBase
    {
        protected IIOController GpioController { get; set; }

        public override PortDirectionType Direction {
            get;
            set; // TODO: shouldn't the direction logic go here?
        }


        protected BiDirectionalPort(
            IPin pin,
            IIOController gpioController,
            IDigitalChannelInfo channel,
            bool initialState = false, 
            bool glitchFilter = false, 
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input)
            : base (pin, channel, initialState, glitchFilter, resistorMode, initialDirection)
        {
            // attempt to reserve the pin
            var result = DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.DigitalInput);

            if (result.Item1)
            {
                // make sure the pin is configured as a digital input
                //this.Pin.GPIOManager.ConfigureInput(_pin, glitchFilter, resistorMode, false);
            }
            else
            {
                throw new PortInUseException();
            }
        }

        public static BiDirectionalPort From(
            IPin pin,
            IIOController ioController,
            bool initialState = false,
            bool glitchFilter = false,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input)
        {
            var chan = pin.SupportedChannels.OfType<IDigitalChannelInfo>().First();
            if (chan != null) {
                //TODO: need other checks here.
                return new BiDirectionalPort(pin, ioController, chan, initialState, glitchFilter, resistorMode, initialDirection);
            } else {
                throw new Exception("Unable to create an output port on the pin, because it doesn't have a digital channel");
            }

        }

        protected override void Dispose(bool disposing)
        {
            // TODO: we should consider moving this logic to the finalizer
            // but the problem with that is that we don't know when it'll be called
            // but if we do it in here, we may need to check the _disposed field
            // elsewhere

            if (!disposed)
            {
                if (disposing)
                {
                    bool success = DeviceChannelManager.ReleasePin(this.Pin);
                }
                disposed = true;
            }
        }

        /// <summary>
        /// True if the port is currently an output; otherwise false
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active
        {
            get => this.Direction == PortDirectionType.Output ? true : false;
            set
            {
                if (value == Active) return;

                //if(value)
                //{
                //    this.Pin.GPIOManager.ConfigureOutput(this.Pin, _currentState);
                //}
                //else
                //{
                //    this.Pin.GPIOManager.ConfigureInput(this.Pin, GlitchFilter, Resistor, _interruptEnabled);
                //}

                this.Direction = value ? PortDirectionType.Output : PortDirectionType.Input;
            }
        }


        public override bool State
        {
            get
            {
                //return _pin.GPIOManager.GetDiscrete(_pin);
                return false;
            }
            set
            {
                //_pin.GPIOManager.SetDiscrete(_pin, value);
                _currentState = value;
            }
        }

    }
}
