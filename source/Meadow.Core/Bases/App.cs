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
        protected App() { 
            Device = MeadowOS.CurrentDevice as D; // 'D' is guaranteed to be initialized and the same type
            Abort = MeadowOS.AppAbort.Token;
        }

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