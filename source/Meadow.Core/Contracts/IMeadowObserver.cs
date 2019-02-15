using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Contracts
{
    public interface IMeadowObserver<T>
    {
        Action<T> Handler { get; set; }
        void OnNext(T value);
    }
}
