using Meadow.Units;
using System;
using System.Collections.Generic;

namespace Meadow
{
    public class FilterableChangeObservableBase<T> : IObservable<T>
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
    }

    public class FilterableChangeObservable<U1> : FilterableChangeObservableBase<U1>
        where U1 : IUnitType
    {
    }

    public class FilterableChangeObservable<T, U1, U2> : FilterableChangeObservableBase<T> 
        where T : CompositeChangeResult<U1, U2>
        where U1 : IUnitType
        where U2 : IUnitType
    {
    }

    public class FilterableChangeObservable<T, U1, U2, U3> : FilterableChangeObservableBase<T>
        where T : CompositeChangeResult<U1, U2, U3>
        where U1 : IUnitType
        where U2 : IUnitType
        where U3 : IUnitType
    {
    }

    public class FilterableUnitChangeObservable<T, U1, U2, U3, U4> : FilterableChangeObservableBase<T>
        where T : CompositeChangeResult<U1, U2, U3, U4>
        where U1 : IUnitType
        where U2 : IUnitType
        where U3 : IUnitType
        where U4 : IUnitType
    {
    }
}