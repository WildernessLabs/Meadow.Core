using System;
using System.Collections.Generic;
using System.Linq;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of reading digital input.
    /// </summary>
    public class DigitalInputPort : DigitalInputPortBase, IObservable<FloatChangeResult>
    {
        protected IIOController IOController { get; set; }

        public override int DebounceDuration { get; set; }
        public override int GlitchFilterCycleCount { get; set; }
        public ResistorMode Resistor { get; set; }

        protected DateTime LastEventTime { get; set; } = DateTime.MinValue;

        private List<IObserver<FloatChangeResult>> _observers;

        protected DigitalInputPort(
            IPin pin,
            IIOController ioController,
            IDigitalChannelInfo channel,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            int debounceDuration = 0,
            int glitchFilterCycleCount = 0
            ) : base(pin, channel, interruptMode)
        {
            this.IOController = ioController;
            this.IOController.Interrupt += OnInterrupt;

            // attempt to reserve
            var success = DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.DigitalInput);
            if (success.Item1)
            {
                // make sure the pin is configured as a digital output with the proper state
                ioController.ConfigureInput(pin, resistorMode, interruptMode, debounceDuration, glitchFilterCycleCount);
                DebounceDuration = debounceDuration;
                GlitchFilterCycleCount = glitchFilterCycleCount;
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
            int debounceDuration = 0,
            int glitchFilterCycleCount = 0
            )
        {
            var chan = pin.SupportedChannels.OfType<IDigitalChannelInfo>().FirstOrDefault();
            if (chan != null) {
                //TODO: need other checks here.
                if (interruptMode != InterruptMode.None && (!chan.InterrruptCapable)) {
                    throw new Exception("Unable to create input; channel is not capable of interrupts");
                }
                return new DigitalInputPort(pin, ioController, chan, interruptMode, resistorMode, debounceDuration, glitchFilterCycleCount);
            } else {
                throw new Exception("Unable to create an output port on the pin, because it doesn't have a digital channel");
            }
        }

        void OnInterrupt(IPin pin)
        {
            if(pin == this.Pin)
            {
                var time = DateTime.Now;

                // debounce timing checks
                if (DebounceDuration > 0) {
                    if ((time - this.LastEventTime).TotalMilliseconds < DebounceDuration) {
                        //Console.WriteLine("Debounced.");
                        return;
                    }
                }

                var state = false;

                switch(InterruptMode)
                {
                    case InterruptMode.EdgeRising:
                    case InterruptMode.LevelHigh:
                        state = true;
                        break;
                    case InterruptMode.EdgeFalling:
                    case InterruptMode.LevelLow:
                        state = false;
                        break;
                    case InterruptMode.EdgeBoth:
                        // we could probably move this query lower to reduce latency risk
                        state = State;
                        break;
                }
                this.LastEventTime = time;
                RaiseChanged(state, time);
            }
        }

        public override void Dispose()
        {
            //TODO: implement full pattern
        }

        public IDisposable Subscribe(IObserver<FloatChangeResult> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);

            return new Unsubscriber(_observers, observer);
        }

        public override bool State
        {
            get => this.IOController.GetDiscrete(this.Pin);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<FloatChangeResult>> _observers;
            private IObserver<FloatChangeResult> _observer;

            public Unsubscriber(List<IObserver<FloatChangeResult>> observers, IObserver<FloatChangeResult> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }
    }
}