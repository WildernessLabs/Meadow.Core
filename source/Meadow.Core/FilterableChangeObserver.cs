using System;

namespace Meadow
{
    //public class FilterableChangeObserver<C, T> : IObserver<C> where C : IChangeResult<T>

    /// <summary>
    /// An `IObserver` that handles change notifications and has an optional
    /// predicate that automatically filters results so only results that match
    /// the predicate will reach the subscriber.
    /// </summary>
    /// <typeparam name="RESULT">The `IChangeResult` notification data.</typeparam>
    /// <typeparam name="UNIT">The datatype that contains the notification data.
    /// I.e. `Temperature` or `decimal`. Must be a `struct`.</typeparam>
    public class FilterableChangeObserver<RESULT, UNIT> : IObserver<RESULT>
        where RESULT : struct, IChangeResult<UNIT>
        where UNIT : struct
    {
        protected Action<RESULT> _handler;// = null;
        protected Predicate<RESULT>? _filter = null;

        protected bool _isInitialized = false;
        protected UNIT? _lastNotifedValue;

        /// <summary>
        /// Creates a new `FilterableChangeObserver` that will execute the handler
        /// when a change occurs. If the `filter` parameter is supplied with a
        /// `Predicate`, only changes that satisfy that predicate (return `true`)
        /// will cause the handler to be invoked.
        /// </summary>
        /// <param name="handler">An `Action` that will be invoked when a
        /// change occurs.</param>
        /// <param name="filter">An optional `Predicate` that filters out any
        /// notifications that don't satisfy (return `true`) the predicate condition.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public FilterableChangeObserver(Action<RESULT> handler/* = null*/, Predicate<RESULT>? filter = null)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            this._handler = handler;
            this._filter = filter;
        }

        /// <summary>
        /// Called by an Observable when a change occurs.
        /// </summary>
        /// <param name="result"></param>
        public void OnNext(RESULT result)
        {
            // first time through, save initial state
            if (!_isInitialized)
            {
                _lastNotifedValue = result.New;
                _isInitialized = true;
            }
            // inject the actual last notified value into the result
            // (each last notified is specific to the observer)
            if (_lastNotifedValue is { } last) {
                result.Old = last;
            }
            //result.Old = _lastNotifedValue;

            // if there is no filter, or if the filter satisfies the result,
            if (_filter == null || _filter(result))
            {
                // update our state
                _lastNotifedValue = result.New;
                // invoke (execute) the handler
                _handler?.Invoke(result);
            }
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("Filtered Observer error: "+ error.Message);
        }
    }
}