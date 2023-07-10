using Meadow.Update;

namespace Meadow;

internal class MeadowAppSettings : IAppSettings
{
    public ILoggingSettings LoggingSettings { get; set; } = new MeadowLoggingSettings();
    public ILifecycleSettings LifecycleSettings { get; set; } = new MeadowLifecycleSettings();
    public IUpdateSettings UpdateSettings { get; set; } = new MeadowUpdateSettings();
}
