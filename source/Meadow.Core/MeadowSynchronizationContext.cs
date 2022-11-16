using System.Threading;
using System;

namespace Meadow
{
    public class MeadowSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback action, object state)
        {

            SendOrPostCallback actionWrap = (object state2) =>
            {
                SynchronizationContext.SetSynchronizationContext(new MeadowSynchronizationContext());
                var async_result = action.BeginInvoke(state2, null, null);
                try {
                    action.EndInvoke(async_result);
                }
                catch (Exception e)
                {
                    Console.WriteLine ($"!!! {e.GetType()}: {e.Message}");
                }
            };
            ThreadPool.QueueUserWorkItem(new WaitCallback(action.Invoke));
        }

        public override SynchronizationContext CreateCopy()
        {
            return new MeadowSynchronizationContext();
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            base.Send(d, state);
        }

        public override void OperationStarted()
        {
            base.OperationStarted();
        }

        public override void OperationCompleted()
        {
            base.OperationCompleted();
        }
    }
}
