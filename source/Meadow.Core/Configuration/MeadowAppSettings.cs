using Meadow.Update;
using System.Collections.Generic;

namespace Meadow;

internal class MeadowAppSettings : IAppSettings
{
    public ILoggingSettings LoggingSettings { get; set; } = new MeadowLoggingSettings();
    public ILifecycleSettings LifecycleSettings { get; set; } = new MeadowLifecycleSettings();
    public IUpdateSettings UpdateSettings { get; set; } = new MeadowUpdateSettings();
    public Dictionary<string, string> Settings { get; set; } = new();
}
