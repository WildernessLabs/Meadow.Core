using Meadow.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Contracts
{
    public interface IMeadowObserver<T>
    {
        void Subscribe(IMeadowObservable<T> provider, Predicate<T> filter, Action<T> handler);
        void Unsubscribe();
        void OnNext(T value);
    }
}
