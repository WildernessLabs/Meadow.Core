using System;
using System.Collections.Generic;
using System.Linq;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of reading digital input.
    /// </summary>
    public class DigitalInputPort : DigitalInputPortBase
    {
        private ResistorMode _resistorMode;
        private uint _debounceDuration;
        private uint _glitchDuration;

        protected IIOController IOController { get; set; }

        private DateTime LastEventTime { get; set; } = DateTime.MinValue;

        protected DigitalInputPort(
            IPin pin,
            IIOController ioController,
            IDigitalChannelInfo channel,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            uint debounceDuration = 0,
            uint glitchDuration = 0
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
            this._debounceDuration = debounceDuration;
            this._glitchDuration = glitchDuration;

            // attempt to reserve
            var success = DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.DigitalInput);
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
            IIOController ioController,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            uint debounceDuration = 0,
            uint glitchDuration = 0
            )
        {
            var chan = pin.SupportedChannels.OfType<IDigitalChannelInfo>().FirstOrDefault();
            if (chan != null) {
                //TODO: need other checks here.
                if (interruptMode != InterruptMode.None && (!chan.InterruptCapable)) {
                    throw new Exception("Unable to create input; channel is not capable of interrupts");
                }
                var port = new DigitalInputPort(pin, ioController, chan, interruptMode, resistorMode, debounceDuration, glitchDuration);                // set these here, not in a constructor because they are virtual
                return port;

            } else {
                throw new Exception("Unable to create an input port on the pin, because it doesn't have a digital channel");
            }
        }

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
            if(pin == this.Pin)
            {
                var capturedLastTime = LastEventTime; // note: doing this for latency reasons. kind of. sort of. bad time good time. all time.
                this.LastEventTime = DateTime.Now;
                RaiseChangedAndNotify(new DigitalInputPortEventArgs(state, this.LastEventTime, capturedLastTime));
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
                    DeviceChannelManager.ReleasePin(Pin);
                }
                disposed = true;
            }
        }

        // Finalizer
        ~DigitalInputPort()
        {
            Dispose(false);
        }

        public override bool State
        {
            get
            {
                var state = this.IOController.GetDiscrete(this.Pin);
                return InverseLogic ? !state : state;
            }
        }

        public override uint DebounceDuration
        {
            get => _debounceDuration;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException();
                _debounceDuration = value;
            }
        }

        public override uint GlitchDuration
        {
            get => _glitchDuration;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException();
                _glitchDuration = value;
            }
        }
    }
}