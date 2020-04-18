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
        protected DateTime LastEventTime { get; set; } = DateTime.MinValue;

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
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input)
            : base (pin, channel, initialState, glitchFilter, interruptMode, resistorMode, initialDirection)
        {
            if (interruptMode != InterruptMode.None && (!channel.InterruptCapable))
            {
                throw new Exception("Unable to create port; channel is not capable of interrupts");
            }
            if (interruptMode != InterruptMode.None && (!channel.InputCapable))
            {
                throw new Exception("Unable to create port; channel is not capable of inputs");
            }
            if (interruptMode != InterruptMode.None && (!channel.OutputCapable))
            {
                throw new Exception("Unable to create port; channel is not capable of outputs");
            }

            this.IOController = gpioController ?? throw new ArgumentNullException(nameof(gpioController));
            this.IOController.Interrupt += OnInterrupt;

            // attempt to reserve the pin - we'll reserve it as an input even though we use it for bi-directional
            var result = DeviceChannelManager.ReservePin(
                this.Pin, 
                ChannelConfigurationType.DigitalInput);

            if(result.Item1)
            {
                _currentDirection = initialDirection;

                // make sure the pin direction (and state for outputs) is configured as desired
                if (_currentDirection == PortDirectionType.Input)
                {
                    this.IOController.ConfigureInput(this.Pin, this.Resistor, interruptMode);
                }
                else
                {
                    this.IOController.ConfigureOutput(this.Pin, InverseLogic ? !this.InitialState : this.InitialState);
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
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input)
        {
            var chan = pin.SupportedChannels.OfType<IDigitalChannelInfo>().FirstOrDefault();
            if(chan == null) 
            {
                throw new Exception("Unable to create an output port on the pin, because it doesn't have a digital channel");
            }
            return new BiDirectionalPort(pin, ioController, chan, initialState, glitchFilter, interruptMode, resistorMode, initialDirection);
        }

        ~BiDirectionalPort()
        {
            Dispose(false);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
                var value = IOController.GetDiscrete(this.Pin);
                return InverseLogic ? !value : value;
            }
            set 
            {
                Direction = PortDirectionType.Output;
                IOController.SetDiscrete(this.Pin, InverseLogic ? !value : value);
            }
        }

        void OnInterrupt(IPin pin, bool state)
        {
            if (pin == this.Pin)
            {
                var time = DateTime.Now;
// p-m TEST BiDirectionalPort AND SEE IF THIS IS USED
                // debounce timing checks
                if (DebounceDuration > 0)
                {
                    if ((time - this.LastEventTime).TotalMilliseconds < DebounceDuration)
                    {
                        return;
                    }
                }

                var capturedLastTime = LastEventTime; // note: doing this for latency reasons. kind of. sort of. bad time good time. all time.
                this.LastEventTime = time;

                RaiseChangedAndNotify(new DigitalInputPortEventArgs(state, time, capturedLastTime));

            }
        }
    }
}
