using System;
using System.Threading;

namespace Meadow;

/// <summary>
/// A SynchronizationContext for Meadow devices
/// </summary>
public class MeadowSynchronizationContext : SynchronizationContext
{
    /// <inheritdoc/>
    public override void Post(SendOrPostCallback action, object state)
    {

        SendOrPostCallback actionWrap = (object state2) =>
        {
            SynchronizationContext.SetSynchronizationContext(new MeadowSynchronizationContext());
            var async_result = action.BeginInvoke(state2, null, null);
            try
            {
                action.EndInvoke(async_result);
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"!!! {e.GetType()}: {e.Message}");
            }
        };
        ThreadPool.QueueUserWorkItem(new WaitCallback(action.Invoke));
    }

    /// <inheritdoc/>
    public override SynchronizationContext CreateCopy()
    {
        return new MeadowSynchronizationContext();
    }

    /// <inheritdoc/>
    public override void Send(SendOrPostCallback d, object state)
    {
        base.Send(d, state);
    }

    /// <inheritdoc/>
    public override void OperationStarted()
    {
        base.OperationStarted();
    }

    /// <inheritdoc/>
    public override void OperationCompleted()
    {
        base.OperationCompleted();
    }
}
