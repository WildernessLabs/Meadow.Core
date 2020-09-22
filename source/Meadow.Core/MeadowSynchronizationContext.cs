using System.Threading;

namespace Meadow
{
    public class MeadowSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback action, object state)
        {
            SendOrPostCallback actionWrap = (object state2) =>
            {
                SynchronizationContext.SetSynchronizationContext(new MeadowSynchronizationContext());
                action.Invoke(state2);
            };
            var callback = new WaitCallback(actionWrap.Invoke);
            ThreadPool.QueueUserWorkItem(callback, state);
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
