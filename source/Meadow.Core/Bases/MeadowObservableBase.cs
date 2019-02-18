using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow
{
    public abstract class MeadowObservableBase<T> : IObservable<T>
    {
        protected Dictionary<IObserver<T>, ObserverContext> _observers;

        public MeadowObservableBase()
        {
            _observers = new Dictionary<IObserver<T>, ObserverContext>();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return AddObserver(observer, null, null);
        }

        public IDisposable Subscribe(IObserver<T> observer, Predicate<T> filter = null, Action<T> handler = null)
        {
            return AddObserver(observer, filter, handler);
        }

        public IDisposable Subscribe<U>(Predicate<T> filter = null, Action<T> handler = null) where U: IObserver<T>
        {
            U observer = Activator.CreateInstance<U>();
            return AddObserver(observer, filter, handler);
        }

        protected void NotifyObservers(T result)
        {
            foreach (var observer in _observers)
            {
                if (observer.Value.Filter == null)
                {
                    InvokeHandler(observer, result);
                }
                else if(observer.Value.Filter(result))
                {
                    InvokeHandler(observer, result);
                }
            }
        }

        private void InvokeHandler(KeyValuePair<IObserver<T>, ObserverContext> observer, T result)
        {
            if (observer.Value.Handler != null)
            {
                observer.Value.Handler.Invoke(result);
            }
            observer.Key.OnNext(result);
        }

        private IDisposable AddObserver(IObserver<T> observer, Predicate<T> filter, Action<T> handler)
        {
            if (!_observers.ContainsKey(observer))
            {
                _observers.Add(observer, new ObserverContext(filter, handler));
            }

            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private Dictionary<IObserver<T>, ObserverContext> _observers;
            private IObserver<T> _observer;

            public Unsubscriber(Dictionary<IObserver<T>, ObserverContext> observers, IObserver<T> observer)
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
            public ObserverContext()
            {
            }

            public ObserverContext(Predicate<T> filter, Action<T> handler)
            {
                Filter = filter;
                Handler = handler;
            }

            public Predicate<T> Filter { get; set; } = null;
            public Action<T> Handler { get; set; } = null;
        }
    }
}
