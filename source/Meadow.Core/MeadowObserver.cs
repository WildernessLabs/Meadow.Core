using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow
{
    public class MeadowObserver<T> : IObserver<T>
    {
        /// <summary>
        /// Notification method which invokes the Handler.
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(T value)
        {
            Console.WriteLine("MeadowObserver Action called with value: " + value.ToString());
        }

        public void OnError(Exception ex)
        {
            Console.WriteLine("MeadowObserver error: " + ex.Message);
        }

        public void OnCompleted()
        {
            Console.WriteLine("MeadowObserver completed");
        }
    }
}
