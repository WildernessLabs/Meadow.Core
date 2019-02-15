using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Contracts
{
    public interface IMeadowObservable<T>
    {
        IDisposable Subscribe(Predicate<T> filter, Action<T> handler);
    }
}
