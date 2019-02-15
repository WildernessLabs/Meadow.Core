using Meadow.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Bases
{
    public abstract class MeadowObservableBase<T> : IMeadowObservable<T>
    {
        public Dictionary<IMeadowObserver<T>, ObserverContext> observers;

        public MeadowObservableBase()
        {
            observers = new Dictionary<IMeadowObserver<T>, ObserverContext>();
        }

        public IDisposable Subscribe(IMeadowObserver<T> observer, SubscriptionMode subscriptionMode, Predicate<T> filter)
        {
            if (!observers.ContainsKey(observer))
            {
                observers.Add(observer, new ObserverContext(subscriptionMode, filter));
            }

            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private Dictionary<IMeadowObserver<T>, ObserverContext> _observers;
            private IMeadowObserver<T> _observer;

            public Unsubscriber(Dictionary<IMeadowObserver<T>, ObserverContext> observers, IMeadowObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

        public abstract void StartSampling(int sampleSize = 10, int sampleIntervalDuration = 40, int sampleSleepDuration = 0);

        public  class ObserverContext
        {
            public ObserverContext(SubscriptionMode subscriptionMode, Predicate<T> filter)
            {
                SubscriptionMode = subscriptionMode;
                Filter = filter;
            }
            public SubscriptionMode SubscriptionMode { get; set; }
            public Predicate<T> Filter { get; set; }
        }
    }

    public enum SubscriptionMode
    {
        Absolute,
        Relative,
        Percentage
    }
}
