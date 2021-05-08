using System;
using System.Collections.Generic;

namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for digital input ports.
    /// </summary>
    public abstract class DigitalInputPortBase : DigitalPortBase, IDigitalInputPort, IDigitalInterruptPort
    {
        /// <summary>
        /// Occurs when the state is changed. To enable this, set the InterruptMode at construction
        /// </summary>
        public event EventHandler<DigitalInputPortChangeResult> Changed = delegate { };

        /// <summary>
        /// Gets or sets a value indicating the type of interrupt monitoring this input.
        /// </summary>
        /// <value><c>true</c> if interrupt enabled; otherwise, <c>false</c>.</value>
        public InterruptMode InterruptMode { get; protected set; }

        public abstract bool State { get; }
        public abstract ResistorMode Resistor { get; set; }
        public abstract double DebounceDuration { get; set; }
        public abstract double GlitchDuration { get; set; }

        protected List<IObserver<DigitalInputPortChangeResult>> _observers { get; set; } = new List<IObserver<DigitalInputPortChangeResult>>();

        protected DigitalInputPortBase(
            IPin pin,
            IDigitalChannelInfo channel,
            InterruptMode interruptMode = InterruptMode.None
            )
            : base(pin, channel)
        {
            this.InterruptMode = interruptMode;
        }

        protected void RaiseChangedAndNotify(DigitalInputPortChangeResult changeResult)
        {
            Changed?.Invoke(this, changeResult);
            _observers.ForEach(x => x.OnNext(changeResult));
        }

        public IDisposable Subscribe(IObserver<DigitalInputPortChangeResult> observer)
        {
            if (!_observers.Contains(observer)) _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<DigitalInputPortChangeResult>> _observers;
            private IObserver<DigitalInputPortChangeResult> _observer;

            public Unsubscriber(List<IObserver<DigitalInputPortChangeResult>> observers, IObserver<DigitalInputPortChangeResult> observer)
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