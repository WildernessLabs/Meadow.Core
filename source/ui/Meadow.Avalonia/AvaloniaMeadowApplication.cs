using Avalonia;
using Avalonia.Threading;

namespace Meadow.UI
{
    /// <summary>
    /// A base class for Avalonia UI Applications using the Meadow stack
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AvaloniaMeadowApplication<T> : Application, IApp
        where T : class, IMeadowDevice
    {
        public CancellationToken CancellationToken => throw new NotImplementedException();

        /// <summary>
        /// The IMeadowDevice
        /// </summary>
        public static T Device => Resolver.Services.Get<IMeadowDevice>() as T;

        protected AvaloniaMeadowApplication()
        {
        }

        /// <summary>
        /// Invokes an action on the main (UI) thread
        /// </summary>
        /// <param name="action">The action to invoke</param>
        /// <param name="state">Optional state information to pass as a paramter to the Action</param>
        public void InvokeOnMainThread(Action<object?> action, object? state = null)
        {
            Dispatcher.UIThread.Post(() => action(state));
        }

        virtual public Task OnError(Exception e)
        {
            return Task.CompletedTask;
        }

        virtual public Task OnShutdown()
        {
            return Task.CompletedTask;
        }

        virtual public void OnUpdate(Version newVersion, out bool approveUpdate)
        {
            approveUpdate = true;
        }

        virtual public void OnUpdateComplete(Version oldVersion, out bool rollbackUpdate)
        {
            rollbackUpdate = false;
        }

        virtual public Task MeadowRun()
        {
            return Task.CompletedTask;
        }

        public virtual Task MeadowInitialize()
        {
            return Task.CompletedTask;
        }

        Task IApp.Run()
        {
            return MeadowRun();
        }

        Task IApp.Initialize()
        {
            return MeadowInitialize();
        }

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