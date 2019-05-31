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
        private PortDirectionType _currentDirection;

        protected IIOController IOController { get; }

        public override PortDirectionType Direction {
            get => _currentDirection;
            set
            {
                // since we're overriding a virtual, which actually gets called in the base ctor, we need to ignore that ctor call (the IO Controller will be null)
                if ((IOController == null) || (value == Direction)) return;
                if (value == PortDirectionType.Input)
                {
                    this.IOController.ConfigureInput(this.Pin, this.Resistor, InterruptMode.None);
                }
                else
                {
                    this.IOController.ConfigureOutput(this.Pin, this.InitialState);
                }
                _currentDirection = value;
            }
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
            this.IOController = gpioController;

            // attempt to reserve the pin - we'll reserve it as an input even though we use it for bi-directional
            // TODO: should we validate that it has both in and out caps?
            var result = DeviceChannelManager.ReservePin(
                this.Pin, 
                ChannelConfigurationType.DigitalInput);

            if(result.Item1)
            {
                _currentDirection = initialDirection;

                // make sure the pin direction (and state for outputs) is configured as desired
                if (_currentDirection == PortDirectionType.Input)
                {
                    this.IOController.ConfigureInput(this.Pin, this.Resistor, InterruptMode.None);
                }
                else
                {
                    this.IOController.ConfigureOutput(this.Pin, this.InitialState);
                }
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

        public override bool State
        {
            get
            {
                Direction = PortDirectionType.Input;
                return IOController.GetDiscrete(this.Pin);
            }
            set 
            {
                Direction = PortDirectionType.Output;
                IOController.SetDiscrete(this.Pin, value);
            }
        }

    }
}
