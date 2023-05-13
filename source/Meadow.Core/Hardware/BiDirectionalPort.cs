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
        protected IMeadowIOController IOController { get; }
        protected DateTime LastEventTime { get; set; } = DateTime.MinValue;

        // Direction change
        public override PortDirectionType Direction
        {
            get => _currentDirection;
            set
            {
                // since we're overriding a virtual, which actually gets called in the base ctor, we need to ignore that ctor call (the IO Controller will be null)
                if ((IOController == null) || (value == Direction)) return;

                // InterruptMode.None disables interrupts within Nuttx via WireInterrupt
                this.IOController.ConfigureInput(this.Pin, this.Resistor, InterruptMode.None, TimeSpan.Zero, TimeSpan.Zero);

                if (value == PortDirectionType.Output)
                {
                    this.IOController.ConfigureOutput(this.Pin, this.InitialState, InitialOutputType);
                }
                _currentDirection = value;
            }
        }

        protected BiDirectionalPort(
            IPin pin,
            IMeadowIOController gpioController,
            IDigitalChannelInfo channel,
            bool initialState,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input,
            OutputType outputType = OutputType.PushPull
            )
            : base(pin, channel, initialState, resistorMode, initialDirection, outputType)
        {
            this.IOController = gpioController ?? throw new ArgumentNullException(nameof(gpioController));

            // attempt to reserve the pin - we'll reserve it as an input even though we use it for bi-directional
            var result = this.IOController.DeviceChannelManager.ReservePin(
                this.Pin,
                ChannelConfigurationType.DigitalInput);

            if (result.Item1)
            {
                Direction = initialDirection;
            }
            else
            {
                throw new PortInUseException($"{this.GetType().Name}: Pin {pin.Name} is already in use");
            }
        }

        public static BiDirectionalPort From(
            IPin pin,
            IMeadowIOController ioController,
            bool initialState = false,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input
            )
        {
            return From(pin, ioController, initialState, resistorMode, initialDirection);
        }

        public static BiDirectionalPort From(
            IPin pin,
            IMeadowIOController ioController,
            bool initialState,
            ResistorMode resistorMode,
            PortDirectionType initialDirection,
            OutputType outputType
            )
        {
            var chan = pin.SupportedChannels.OfType<IDigitalChannelInfo>().FirstOrDefault();
            if (chan == null)
            {
                throw new Exception("Unable to create an output port on the pin, because it doesn't have a digital channel");
            }
            return new BiDirectionalPort(pin, ioController, chan, initialState, resistorMode, initialDirection, outputType);
        }

        ~BiDirectionalPort()
        {
            Dispose(false);
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
                    this.IOController.UnconfigureGpio(this.Pin);
                    bool success = this.IOController.DeviceChannelManager.ReleasePin(this.Pin);
                }
                disposed = true;
            }
        }

        public override bool State
        {
            get
            {
                Direction = PortDirectionType.Input;
                var value = IOController.GetDiscrete(this.Pin);
                return InverseLogic ? !value : value;
            }
            set
            {
                Direction = PortDirectionType.Output;
                IOController.SetDiscrete(this.Pin, InverseLogic ? !value : value);
            }
        }
    }
}
