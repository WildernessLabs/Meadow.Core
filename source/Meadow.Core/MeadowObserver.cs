using Meadow.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow
{
    public class MeadowObserver<T> : IMeadowObserver<T>
    {
        public Action<T> Handler { get; set; }

        /// <summary>
        /// Notification method which invokes the Handler.
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(T value)
        {
            Handler.Invoke(value);
        }
    }
}
