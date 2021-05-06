using Meadow.Units;
using System;
using System.Collections.Generic;

namespace Meadow
{
    public abstract class FilterableChangeObservableBase<T, U1> : IObservable<T>
        where T : struct, IChangeResult<U1>
        where U1 : struct
    {
        // collection of observers
        protected List<IObserver<T>> observers { get; set; } = new List<IObserver<T>>();

        protected void NotifyObservers(T changeResult)
        {
            observers.ForEach(x => x.OnNext(changeResult));
        }

        /// <summary>
        /// Subscribes an `IObserver` to get notified when a change occurs.
        /// </summary>
        /// <param name="observer">The `IObserver` that will receive the
        /// change notifications.</param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }

            return new Unsubscriber(observers, observer);
        }

        /// <summary>
        /// class to handle the collection of subscribers.
        /// </summary>
        private class Unsubscriber : IDisposable
        {
            private List<IObserver<T>> observers;
            private IObserver<T> observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }

            public void Dispose()
            {
                if (!(observer == null)) { observers.Remove(observer); }
            }
        }

        public static FilterableChangeObserver<T, U1> CreateObserver(Action<T> handler, Predicate<T>? filter = null)
        {
            return new FilterableChangeObserver<T, U1>(
                handler, filter);
        }

    }
}