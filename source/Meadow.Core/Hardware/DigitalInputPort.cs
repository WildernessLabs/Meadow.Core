using System;
using System.Linq;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of reading digital input.
    /// </summary>
    public class DigitalInputPort : DigitalInputPortBase
    {
        private ResistorMode _resistorMode;
        private TimeSpan _debounceDuration;
        private TimeSpan _glitchDuration;

        protected IMeadowIOController IOController { get; set; }

        private DateTime LastEventTime { get; set; } = DateTime.MinValue;

        protected DigitalInputPort(
            IPin pin,
            IMeadowIOController ioController,
            IDigitalChannelInfo channel,
            InterruptMode interruptMode,
            ResistorMode resistorMode,
            TimeSpan debounceDuration,
            TimeSpan glitchDuration
            ) : base(pin, channel, interruptMode)
        {
            // DEVELOPER NOTE:
            // Debounce recognizes the first state transition and then ignores anything after that for a period of time.
            // Glitch filtering ignores the first state transition and waits a period of time and then looks at state to make sure the result is stable

            if (interruptMode != InterruptMode.None && (!channel.InterruptCapable))
            {
                throw new Exception("Unable to create port; channel is not capable of interrupts");
            }

            this.IOController = ioController;
            this.IOController.Interrupt += OnInterrupt;
            this._resistorMode = resistorMode;
            _debounceDuration = debounceDuration;
            _glitchDuration = glitchDuration;

            // attempt to reserve
            var success = this.IOController.DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.DigitalInput);
            if (success.Item1)
            {
                // make sure the pin is configured as a digital input with the proper state
                ioController.ConfigureInput(pin, resistorMode, interruptMode, debounceDuration, glitchDuration);
            }
            else
            {
                throw new PortInUseException();
            }
        }

        public static DigitalInputPort From(
            IPin pin,
            IMeadowIOController ioController,
            InterruptMode interruptMode,
            ResistorMode resistorMode,
            TimeSpan debounceDuration,
            TimeSpan glitchDuration
            )
        {
            // convert to microseconds
            var debounce = debounceDuration.TotalMilliseconds * 10;
            var glitch = glitchDuration.TotalMilliseconds * 10;

            var chan = pin.SupportedChannels.OfType<IDigitalChannelInfo>().FirstOrDefault();
            //TODO: may need other checks here.
            if (chan == null)
            {
                throw new Exception("Unable to create an input port on the pin, because it doesn't have a digital channel");
            }
            if (interruptMode != InterruptMode.None && (!chan.InterruptCapable))
            {
                throw new Exception("Unable to create input; channel is not capable of interrupts");
            }
            if (debounce < 0.0 || debounce > 1000.0)
            {
                throw new ArgumentOutOfRangeException(nameof(debounceDuration), "Unable to create an input port, because debounceDuration is out of range (0.1-1000.0)");
            }
            if (glitch < 0.0 || glitch > 1000.0)
            {
                throw new ArgumentOutOfRangeException(nameof(glitchDuration), "Unable to create an input port, because glitchDuration is out of range (0.1-1000.0)");
            }

            var port = new DigitalInputPort(pin, ioController, chan, interruptMode, resistorMode,
                            debounceDuration, glitchDuration);
            return port;
        }

        /// <summary>
        /// Gets or Sets the internal resistor mode for the input
        /// </summary>
        public override ResistorMode Resistor
        {
            get => _resistorMode;
            set
            {
                IOController.SetResistorMode(this.Pin, value);
                _resistorMode = value;
            }
        }

        void OnInterrupt(IPin pin, bool state)
        {
            if (pin == this.Pin)
            {
                var capturedLastTime = LastEventTime; // note: doing this for latency reasons. kind of. sort of. bad time good time. all time.
                this.LastEventTime = DateTime.Now;
                //Console.WriteLine($"event on pin: {pin.Name}, state: {state}");
                // BC 2021.05.21 b5.0: Changed this to the new result type.
                // assuming that old state is just an inversion of the new state if date time isn't min, yeah?
                DigitalState? old = (capturedLastTime == DateTime.MinValue) ? null : new DigitalState(!state, capturedLastTime);
                RaiseChangedAndNotify(new DigitalPortResult(new DigitalState(state, this.LastEventTime), old));
                //RaiseChangedAndNotify(new DigitalInputPortChangeResult(state, this.LastEventTime, capturedLastTime));
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // TODO: we should consider moving this logic to the finalizer
            // but the problem with that is that we don't know when it'll be called
            // but if we do it in here, we may need to check the _disposed field
            // elsewhere
            if (!disposed)
            {
                if (disposing)
                {
                    this.IOController.Interrupt -= OnInterrupt;
                    this.IOController.DeviceChannelManager.ReleasePin(Pin);
                    IOController.UnconfigureGpio(Pin);
                }
                disposed = true;
            }
        }

        // Finalizer
        ~DigitalInputPort()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the current State of the input (True == high, False == low)
        /// </summary>
        public override bool State
        {
            get
            {
                var state = this.IOController.GetDiscrete(this.Pin);
                return InverseLogic ? !state : state;
            }
        }

        /// <summary>
        /// Gets or Sets the interrupt debounce duration
        /// </summary>
        public override TimeSpan DebounceDuration
        {
            get => _debounceDuration;
            set
            {
                if (value.TotalMilliseconds < 0.0 || value.TotalMilliseconds > 1000.0) throw new ArgumentOutOfRangeException("DebounceDuration");
                if (value == _debounceDuration) return;

                _debounceDuration = value;

                // Update in F7
                // we have to disconnect the interrupt and reconnect, otherwise we'll get an error for an already-wired interupt
                this.IOController.WireInterrupt(Pin, InterruptMode.None, _resistorMode, TimeSpan.Zero, TimeSpan.Zero);
                this.IOController.WireInterrupt(Pin, InterruptMode, _resistorMode, _debounceDuration, _glitchDuration);
            }
        }

        /// <summary>
        /// Gets or Sets the interrupt glitch filter duration
        /// </summary>
        public override TimeSpan GlitchDuration
        {
            get => _glitchDuration;
            set
            {
                if (value.TotalMilliseconds < 0.0 || value.TotalMilliseconds > 1000.0) throw new ArgumentOutOfRangeException("GlitchDuration");
                if (value == _glitchDuration) return;

                _glitchDuration = value;

                // Update in F7
                // we have to disconnect the interrupt and reconnect, otherwise we'll get an error for an already-wired interupt
                this.IOController.WireInterrupt(Pin, InterruptMode.None, _resistorMode, TimeSpan.Zero, TimeSpan.Zero);
                this.IOController.WireInterrupt(Pin, InterruptMode, _resistorMode, _debounceDuration, _glitchDuration);
            }
        }
    }
}