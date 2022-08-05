namespace Meadow
{
    using Meadow.Devices;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a base implementation for the Meadow App. Use this
    /// class for Meadow applications to get strongly-typed access to the current
    /// device information.
    /// </summary>
    public abstract class App<D> : IApp, IAsyncDisposable
            where D : class, IMeadowDevice
    {
        private ExecutionContext executionContext;

        protected App()
        {
            executionContext = Thread.CurrentThread.ExecutionContext;

            Device = MeadowOS.CurrentDevice as D; // 'D' is guaranteed to be initialized and the same type
            Abort = MeadowOS.AppAbort.Token;

            Resolver.Services.Add<IMeadowDevice>(Device);
            Resolver.Services.Add<IApp>(this);
        }

        public void InvokeOnMainThread(Action<object> action, object? state = null)
        {
            ExecutionContext.Run(executionContext, new ContextCallback(action), state);
        }

        public virtual Task Run() { return Task.CompletedTask; }

        public virtual Task Initialize() { return Task.CompletedTask; }

        public virtual void OnShutdown(out bool complete, Exception? e = null) { complete = true; }

        public virtual void OnError(Exception e, out bool recovered) { recovered = false; }

        public virtual void OnResume() { }

        public virtual void OnSleep() { }

        public virtual void OnRecovery(Exception e) { }

        public virtual void OnUpdate(Version newVersion, out bool approveUpdate) { approveUpdate = true; }

        public virtual void OnUpdateComplete(Version oldVersion, out bool rollbackUpdate) { rollbackUpdate = false; }

        public virtual void OnReset() { }

        /// <summary>
        /// The root Device interface
        /// </summary>
        public static D Device { get; protected set; }

        /// <summary>
        /// The app cancellation token
        /// </summary>
        public static CancellationToken Abort { get; protected set; }

        public async virtual ValueTask DisposeAsync() { return; }
    }
}