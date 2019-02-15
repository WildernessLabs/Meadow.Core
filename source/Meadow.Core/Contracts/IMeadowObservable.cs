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
        IDisposable Subscribe(IMeadowObserver<T> observer, SubscriptionMode subscriptionMode, Predicate<T> filter);

        void StartSampling(int sampleSize = 10, int sampleIntervalDuration = 40, int sampleSleepDuration = 0);
    }
}
