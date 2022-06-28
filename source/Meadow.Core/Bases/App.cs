namespace Meadow
{
    using System;
    using Meadow.Devices;
    using System.Threading.Tasks;
    using System.Threading;

    /// <summary>
    /// Provides a base implementation for the Meadow App. Use this
    /// class for Meadow applications to get strongly-typed access to the current
    /// device information.
    /// </summary>
    public abstract class App<D> : IApp, IAsyncDisposable
            where D : class, IMeadowDevice
    {
        readonly CancellationTokenSource cts = new CancellationTokenSource();

        protected App() {
            Device = MeadowOS.CurrentDevice as D; // 'D' is guaranteed to be initialized and the same type
            Abort = MeadowOS.AppAbort.Token;
        }

        public virtual Task Run()
        {
            return Task.Delay(-1, cts.Token);

            /*
            var task = Task.Run(() =>
            {
                Thread.Sleep(Timeout.Infinite);
            });

            return task; 
            */
        }

        public virtual Task Initialize() {  return Task.CompletedTask; }

        public virtual void Shutdown(out bool complete, Exception? e = null) 
        { 
            cts.Cancel();
            complete = true; 
        }

        public virtual void OnError(Exception e, out bool recovered) { recovered = false; }

        public virtual void Resume() { }

        public virtual void Sleep() { }

        public virtual void Recovery(Exception e) { }

        public virtual void Update(Version newVersion, out bool approveUpdate) { approveUpdate = true; }

        public virtual void UpdateComplete(Version oldVersion, out bool rollbackUpdate) { rollbackUpdate = false; }

        public virtual void Reset() { }

        /// <summary>
        /// The root Device interface
        /// </summary>
        public static D Device {get; protected set;}

        /// <summary>
        /// The app cancellation token
        /// </summary>
        public static CancellationToken Abort {get; protected set;}

        public async virtual ValueTask DisposeAsync() { return; }
    }
}