using Meadow.Units;
using System;

namespace Meadow.Bases
{
    public class FilterableChangeObservableBase<U1> : IObserver<CompositeChangeResult<U1>> 
        where U1 : UnitType
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
        where U1 : UnitType
        where U2 : UnitType
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
        where U1 : UnitType
        where U2 : UnitType
        where U3 : UnitType
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
}
