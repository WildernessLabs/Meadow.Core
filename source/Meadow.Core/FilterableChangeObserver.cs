using System;

namespace Meadow
{
    /// <summary>
    /// An `IObserver` that handles change notifications and has an optional
    /// predicate that automatically filters results so only results that match
    /// the predicate will reach the subscriber.
    /// </summary>
    /// <typeparam name="UNIT">The datatype that contains the notification data.
    /// I.e. `Temperature` or `decimal`. Must be a `struct`.</typeparam>
    public class FilterableChangeObserver<UNIT> : IObserver<IChangeResult<UNIT>>
        where UNIT : struct
    {
        /// <summary>
        /// Than handler that is called in `OnNext` if the filter is satisfied.
        /// </summary>
        protected Action<IChangeResult<UNIT>> Handler { get; } = delegate { };
        /// <summary>
        /// A filter that specifies whether or not the observer should get notified.
        /// </summary>
        protected Predicate<IChangeResult<UNIT>>? Filter { get; } = null;

        /// <summary>
        /// The last notified value. Note that this may differ from the `Old`
        /// property on the result, because this only gets updated if the filter
        /// is satisfied and the result is sent to the observer.
        /// </summary>
        protected UNIT? lastNotifedValue;

        /// <summary>
        /// Creates a new `FilterableChangeObserver` that will execute the handler
        /// when a change occurs. If the `filter` parameter is supplied with a
        /// `Predicate`, only changes that satisfy that predicate (return `true`)
        /// will cause the handler to be invoked.
        /// </summary>
        /// <param name="handler">An `Action` that will be invoked when a
        /// change occurs.</param>
        /// <param name="filter">An optional `Predicate` that filters out any
        /// notifications that don't satisfy (return `true`) the predicate condition.
        /// Note that the first reading will always call the handler.</param>
        public FilterableChangeObserver(Action<IChangeResult<UNIT>> handler, Predicate<IChangeResult<UNIT>>? filter = null)
        {
            this.Handler = handler;
            this.Filter = filter;
        }

        /// <summary>
        /// Called by an Observable when a change occurs.
        /// </summary>
        /// <param name="result"></param>
        public void OnNext(IChangeResult<UNIT> result)
        {
            // if the last notified value isn't null, inject it into the result.
            // (each last notified is specific to the observer)
            if (lastNotifedValue is { } last) {
                result.Old = last;
            }

            // if there is no filter,
            //  OR
            // if the filter satisfies the result,
            //  OR
            // if it's the first time (result.Old == null)
            if (Filter == null || Filter(result) || result.Old is null)
            {
                // save the last notified value as this new value
                lastNotifedValue = result.New;
                // invoke (execute) the handler
                Handler?.Invoke(result);
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