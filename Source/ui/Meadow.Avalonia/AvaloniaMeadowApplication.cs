using Avalonia;
using Avalonia.Threading;

namespace Meadow.Avalonia
{
    /// <summary>
    /// A base class for Avalonia UI Applications using the Meadow stack.
    /// </summary>
    /// <typeparam name="T">The type of the Meadow device that implements <see cref="IMeadowDevice"/>.</typeparam>
    public class AvaloniaMeadowApplication<T> : Application, IApp
        where T : class, IMeadowDevice
    {
        /// <inheritdoc/>
        public CancellationToken CancellationToken => throw new NotImplementedException();

        /// <summary>
        /// Gets the Meadow device instance.
        /// </summary>
        public static T Device => Resolver.Services.Get<IMeadowDevice>() as T;

        /// <inheritdoc/>
        public Dictionary<string, string> Settings { get; internal set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaMeadowApplication{T}"/> class.
        /// </summary>
        protected AvaloniaMeadowApplication()
        {
        }

        /// <inheritdoc/>
        public virtual void OnBootFromCrash(IEnumerable<string> crashReports) { }

        /// <summary>
        /// Invokes an action on the main (UI) thread.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="state">Optional state information to pass as a parameter to the action.</param>
        public void InvokeOnMainThread(Action<object?> action, object? state = null)
        {
            Dispatcher.UIThread.Post(() => action(state));
        }

        /// <inheritdoc/>
        public virtual Task OnError(Exception e)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task OnShutdown()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual void OnUpdate(Version newVersion, out bool approveUpdate)
        {
            approveUpdate = true;
        }

        /// <inheritdoc/>
        public virtual void OnUpdateComplete(Version oldVersion, out bool rollbackUpdate)
        {
            rollbackUpdate = false;
        }

        /// <inheritdoc/>
        public virtual Task MeadowRun()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task MeadowInitialize()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        Task IApp.Run()
        {
            return MeadowRun();
        }

        /// <inheritdoc/>
        Task IApp.Initialize()
        {
            return MeadowInitialize();
        }

        /// <summary>
        /// Loads MeadowOS in a background thread.
        /// </summary>
        protected void LoadMeadowOS()
        {
            new Thread((o) =>
            {
                _ = MeadowOS.Start(this, null);
            })
            {
                IsBackground = true
            }
            .Start();
        }
    }
}