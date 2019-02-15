using Meadow.Bases;
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
        private IDisposable unsubscriber;
        private Action<T> Handler;

        /// <summary>
        /// Subscribe to changes and get notified based on filters.
        /// </summary>
        /// <param name="provider">The MeadowObservable to subscribe to.</param>
        /// <param name="subscriptionMode">Mode to toggle which value to filter on.</param>
        /// <param name="filter">Predicate to filter notifications.</param>
        /// <param name="handler">Handler to process notifications.</param>
        public virtual void Subscribe(IMeadowObservable<T> provider, SubscriptionMode subscriptionMode, Predicate<T> filter, Action<T> handler)
        {
            unsubscriber = provider.Subscribe(this, subscriptionMode, filter);
            Handler = handler;
        }
        
        /// <summary>
        /// Unsubscribe from the MeadowObservable
        /// </summary>
        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }

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
