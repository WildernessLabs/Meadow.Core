namespace Meadow.UI
{
    public class MauiMeadowApplication<T> : Application, IApp
        where T : class, IMeadowDevice
    {
        public CancellationToken CancellationToken => throw new NotImplementedException();

        public static T Device => Resolver.Services.Get<IMeadowDevice>() as T;

        /// <inheritdoc/>
        public Dictionary<string, string> Settings { get; internal set; } = new();

        protected MauiMeadowApplication()
        {
        }

        /// <inheritdoc/>
        public virtual void OnBootFromCrash(IEnumerable<string> crashReports) { }

        /// <inheritdoc/>
        public void InvokeOnMainThread(Action<object?> action, object? state = null)
        {
            MainThread.BeginInvokeOnMainThread(() => action(state));
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