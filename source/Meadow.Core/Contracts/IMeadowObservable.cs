using Meadow.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Contracts
{
    public interface IMeadowObservable<T>
    {
        IDisposable Subscribe(IMeadowObserver<T> observer, Predicate<T> filter);
    }
}
