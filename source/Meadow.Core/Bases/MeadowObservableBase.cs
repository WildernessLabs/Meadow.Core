using Meadow.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Bases
{
    public abstract class MeadowObservableBase<T> : IMeadowObservable<T>
    {
        public Dictionary<IMeadowObserver<T>, ObserverContext> _observers;

        public MeadowObservableBase()
        {
            _observers = new Dictionary<IMeadowObserver<T>, ObserverContext>();
        }

        public IDisposable Subscribe(IMeadowObserver<T> observer, Predicate<T> filter)
        {
            if (!_observers.ContainsKey(observer))
            {
                _observers.Add(observer, new ObserverContext(filter));
            }

            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private Dictionary<IMeadowObserver<T>, ObserverContext> _observers;
            private IMeadowObserver<T> _observer;

            public Unsubscriber(Dictionary<IMeadowObserver<T>, ObserverContext> observers, IMeadowObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

        // TODO: @BRYANC @BRIANK this is fine for AnalogInputPort; does it apply to DigitalInput? It might.
        public abstract void StartSampling(int sampleSize = 10, int sampleIntervalDuration = 40, int sampleSleepDuration = 0);

        public  class ObserverContext
        {
            public ObserverContext(Predicate<T> filter)
            {
                Filter = filter;
            }
            public Predicate<T> Filter { get; set; }
        }
    }
}
