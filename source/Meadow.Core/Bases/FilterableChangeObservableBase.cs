using Meadow.Units;
using System;
using System.Collections.Generic;

namespace Meadow.Bases
{
    public class FilterableChangeObservableBase<U1> : IObserver<CompositeChangeResult<U1>> 
        where U1 : IUnitType
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(CompositeChangeResult<U1> value)
        {
            throw new NotImplementedException();
        }

        protected void NotifyObservers(U1 changeResult) { }
    }

    public class FilterableChangeObservableBase<T, U1, U2> : IObserver<T> 
        where T : CompositeChangeResult<U1, U2>
        where U1 : IUnitType
        where U2 : IUnitType
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T value)
        {
            throw new NotImplementedException();
        }
    }

    public class FilterableChangeObservableBase<T, U1, U2, U3> : IObserver<T>
        where T : CompositeChangeResult<U1, U2, U3>
        where U1 : IUnitType
        where U2 : IUnitType
        where U3 : IUnitType
    {
        // collection of observers
        protected List<IObserver<T>> observers { get; set; } = new List<IObserver<T>>();

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T value)
        {
            throw new NotImplementedException();
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
                observers = observers;
                observer = observer;
            }

            public void Dispose()
            {
                if (!(observer == null)) { observers.Remove(observer); }
            }
        }
    }
}
