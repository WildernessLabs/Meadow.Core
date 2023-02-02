namespace Meadow
{
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

        public CancellationToken CancellationToken { get; internal set; }

        protected App()
        {
            executionContext = Thread.CurrentThread.ExecutionContext;

            Device = MeadowOS.CurrentDevice as D; // 'D' is guaranteed to be initialized and the same type
            Abort = MeadowOS.AppAbort.Token;

            Resolver.Services.Add<IApp>(this);
        }

        public void InvokeOnMainThread(Action<object> action, object? state = null)
        {
            ExecutionContext.Run(executionContext, new ContextCallback(action), state);
        }

        public virtual Task Run()
        {
            return Task.CompletedTask;
        }

        public virtual Task Initialize() { return Task.CompletedTask; }

        public virtual Task OnShutdown() { return Task.CompletedTask; }

        public virtual Task OnError(Exception e) { return Task.CompletedTask; }

        /// <summary>
        /// Called when the application is about to update itself.
        /// </summary>
        public void OnUpdate(Version newVersion, out bool approveUpdate)
        {
            approveUpdate = true;
        }

        /// <summary>
        /// Called when the application has updated itself.
        /// </summary>
        public void OnUpdateComplete(Version oldVersion, out bool rollbackUpdate)
        {
            rollbackUpdate = false;
        }

        /// <summary>
        /// The root Device interface
        /// </summary>
        public static D Device { get; protected set; }

        /// <summary>
        /// The app cancellation token
        /// </summary>
        public static CancellationToken Abort { get; protected set; }

        public virtual ValueTask DisposeAsync() { return new ValueTask(Task.CompletedTask); }
    }
}