using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow
{
    public class FilteredObserver<T> : IObserver<T>
    {
        protected Predicate<T> _filter = null;
        protected Action<T> _handler = null;

        public FilteredObserver(Predicate<T> filter = null, Action<T> handler = null)
        {
            this._filter = filter;
            this._handler = handler;
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
