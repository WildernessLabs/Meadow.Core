namespace Meadow.UI
{
    public class MauiMeadowApplication<T> : Application, IApp
        where T : class, IMeadowDevice
    {
        public CancellationToken CancellationToken => throw new NotImplementedException();

        public static T Device => Resolver.Services.Get<IMeadowDevice>() as T;

        protected MauiMeadowApplication()
        {
        }

        public void InvokeOnMainThread(Action<object?> action, object? state = null)
        {
            MainThread.BeginInvokeOnMainThread(() => action(state));
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