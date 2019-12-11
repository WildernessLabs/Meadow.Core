using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow
{
    public class FilterableObserver<C, T> : IObserver<C> where C : IChangeResult<T>
    {
        protected Predicate<C> _filter = null;
        protected Action<C> _handler = null;

        protected bool _isInitialized = false;
        protected T _lastNotifedValue;

        public FilterableObserver(Action<C> handler = null, Predicate<C> filter = null)
        {
            this._handler = handler;
            this._filter = filter;
        }

        public void OnNext(C result)
        {
            // first time through, save initial state
            if (!_isInitialized) {
                _lastNotifedValue = result.New;
                _isInitialized = true;
            }
            // inject the actual last notified value into the result
            // (each last notified is specific to the observer)
            result.Old = _lastNotifedValue;

            if (_filter == null || _filter(result))
            {
                // update our state
                _lastNotifedValue = result.New;
                // invoke
                _handler?.Invoke(result);
            }
        }

        public void OnCompleted()
        {
            Console.WriteLine("Filtered Observer completed");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("Filtered Observer error: "+ error.Message);
        }
    }

}
