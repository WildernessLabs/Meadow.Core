using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow
{
    public class FilterableObserver<T> : IObserver<T>
    {
        protected Predicate<T> _filter = null;
        protected Action<T> _handler = null;

        public FilterableObserver(Action<T> handler = null, Predicate<T> filter = null)
        {
            this._handler = handler;
            this._filter = filter;
        }

        public void OnNext(T value)
        {
            if (_filter == null || _filter(value))
            {
                _handler?.Invoke(value);
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
