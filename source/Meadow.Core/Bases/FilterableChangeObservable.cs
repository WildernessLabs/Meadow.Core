using Meadow.Units;
using System;
using System.Collections.Generic;

namespace Meadow
{
    public class FilterableChangeObservable<T, U1> : FilterableChangeObservableBase<T>
        where T : CompositeChangeResult<U1>
        where U1 : IUnitType
    {
        public static FilterableChangeObserver<T, U1?> CreateObserver(Action<T> handler, Predicate<T>? filter = null)
        {
            return new FilterableChangeObserver<T, U1?>(
                handler, filter);
        }
    }

    public class FilterableChangeObservable<T, U1, U2> : FilterableChangeObservableBase<T>
        where T : CompositeChangeResult<U1, U2>
        where U1 : IUnitType
        where U2 : IUnitType
    {
        public static FilterableChangeObserver<T, (U1, U2)?> CreateObserver(Action<T> handler, Predicate<T>? filter = null)
        {
            return new FilterableChangeObserver<T, (U1, U2)?>(
                handler, filter);
        }
    }

    public class FilterableChangeObservable<T, U1, U2, U3> : FilterableChangeObservableBase<T>
        where T : CompositeChangeResult<U1, U2, U3>
        where U1 : IUnitType
        where U2 : IUnitType
        where U3 : IUnitType
    {
        public static FilterableChangeObserver<T, (U1, U2, U3)?> CreateObserver(Action<T> handler, Predicate<T>? filter = null)
        {
            return new FilterableChangeObserver<T, (U1, U2, U3)?>(
                handler, filter);
        }
    }

    public class FilterableUnitChangeObservable<T, U1, U2, U3, U4> : FilterableChangeObservableBase<T>
        where T : CompositeChangeResult<U1, U2, U3, U4>
        where U1 : IUnitType
        where U2 : IUnitType
        where U3 : IUnitType
        where U4 : IUnitType
    {
        public static FilterableChangeObserver<T, (U1, U2, U3, U4)?> CreateObserver(Action<T> handler, Predicate<T>? filter = null)
        {
            return new FilterableChangeObserver<T, (U1, U2, U3, U4)?>(
                handler, filter);
        }
    }

    public class FilterableUnitChangeObservable<T, U1, U2, U3, U4, U5> : FilterableChangeObservableBase<T>
        where T : CompositeChangeResult<U1, U2, U3, U4, U5>
        where U1 : IUnitType
        where U2 : IUnitType
        where U3 : IUnitType
        where U4 : IUnitType
        where U5 : IUnitType
    {
        public static FilterableChangeObserver<T, (U1, U2, U3, U4, U5)?> CreateObserver(Action<T> handler, Predicate<T>? filter = null)
        {
            return new FilterableChangeObserver<T, (U1, U2, U3, U4, U5)?>(
                handler, filter);
        }
    }

    public class FilterableUnitChangeObservable<T, U1, U2, U3, U4, U5, U6> : FilterableChangeObservableBase<T>
        where T : CompositeChangeResult<U1, U2, U3, U4, U5, U6>
        where U1 : IUnitType
        where U2 : IUnitType
        where U3 : IUnitType
        where U4 : IUnitType
        where U5 : IUnitType
        where U6 : IUnitType
    {
        public static FilterableChangeObserver<T, (U1, U2, U3, U4, U5, U6)?> CreateObserver(Action<T> handler, Predicate<T>? filter = null)
        {
            return new FilterableChangeObserver<T, (U1, U2, U3, U4, U5, U6)?>(
                handler, filter);
        }
    }
}
