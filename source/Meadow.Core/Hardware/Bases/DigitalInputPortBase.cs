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
        public event EventHandler<DigitalPortResult> Changed = delegate { };

        /// <summary>
        /// Gets or sets a value indicating the type of interrupt monitoring this input.
        /// </summary>
        /// <value><c>true</c> if interrupt enabled; otherwise, <c>false</c>.</value>
        public InterruptMode InterruptMode { get; protected set; }

        public abstract bool State { get; }
        public abstract ResistorMode Resistor { get; set; }
        public abstract double DebounceDuration { get; set; }
        public abstract double GlitchDuration { get; set; }

        protected List<IObserver<IChangeResult<DigitalState>>> _observers { get; set; } = new List<IObserver<IChangeResult<DigitalState>>>();

        protected DigitalInputPortBase(
            IPin pin,
            IDigitalChannelInfo channel,
            InterruptMode interruptMode = InterruptMode.None
            )
            : base(pin, channel)
        {
            // TODO: check interrupt mode (i.e. if != none, make sure channel info agrees)
            this.InterruptMode = interruptMode;
        }

        protected void RaiseChangedAndNotify(DigitalPortResult changeResult)
        {
            Changed?.Invoke(this, changeResult);
            _observers.ForEach(x => x.OnNext(changeResult));
        }

        public IDisposable Subscribe(IObserver<IChangeResult<DigitalState>> observer)
        {
            if (!_observers.Contains(observer)) _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<IChangeResult<DigitalState>>> _observers;
            private IObserver<IChangeResult<DigitalState>> _observer;

            public Unsubscriber(List<IObserver<IChangeResult<DigitalState>>> observers, IObserver<IChangeResult<DigitalState>> observer)
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