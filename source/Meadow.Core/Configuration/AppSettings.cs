using Meadow.Update;

namespace Meadow
{
    public interface IAppSettings
    {
        public ILoggingSettings LoggingSettings { get; }
        public ILifecycleSettings LifecycleSettings { get; }
        public IUpdateSettings UpdateSettings { get; }
    }
}
