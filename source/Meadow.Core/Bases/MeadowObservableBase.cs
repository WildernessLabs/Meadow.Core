using Meadow.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow
{
    public abstract class MeadowObservableBase<T, U> : IMeadowObservable<T> where U: IMeadowObserver<T>
    {
        protected Dictionary<IMeadowObserver<T>, ObserverContext> _observers;

        public MeadowObservableBase()
        {
            _observers = new Dictionary<IMeadowObserver<T>, ObserverContext>();
        }

        public IDisposable Subscribe(Predicate<T> filter, Action<T> handler)
        {
            U observer = Activator.CreateInstance<U>();
            observer.Handler = handler;

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
