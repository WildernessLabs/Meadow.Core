using Meadow.Units;
using System;
using System.Collections.Generic;

namespace Meadow
{
    public class FilterableChangeObservable<T, U1> : FilterableChangeObservableBase<T>
        where T : struct, IChangeResult<U1>
        where U1 : struct //struct
    {
        public static FilterableChangeObserver<T, U1> CreateObserver(Action<T> handler, Predicate<T>? filter = null)
        {
            return new FilterableChangeObserver<T, U1>(
                handler, filter);
        }
    }
}
