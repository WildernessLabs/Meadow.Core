using System;

namespace Meadow
{
    //public class FilterableChangeObserver<C, T> : IObserver<C> where C : IChangeResult<T>

    /// <summary>
    /// An `IObserver` that handles change notifications and has an optional
    /// predicate that automatically filters results so only results that match
    /// the predicate will reach the subscriber.
    /// </summary>
    /// <typeparam name="C">The `IChangeResult` notification data.</typeparam>
    /// <typeparam name="T">The datatype that contains the notification data.
    /// I.e. `AtmosphericConditions` or `decimal`.</typeparam>
    public class FilterableChangeObserver<C, T> : IObserver<C>
        where C : struct, IChangeResult<T>
        //where T : notnull //struct
        where T : struct//, IComparable
    {
        protected Action<C> _handler;// = null;
        protected Predicate<C>? _filter = null;

        protected bool _isInitialized = false;
        protected T? _lastNotifedValue;

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
        public FilterableChangeObserver(Action<C> handler/* = null*/, Predicate<C>? filter = null)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            this._handler = handler;
            this._filter = filter;
        }

        /// <summary>
        /// Called by an Observable when a change occurs.
        /// </summary>
        /// <param name="result"></param>
        public void OnNext(C result)
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