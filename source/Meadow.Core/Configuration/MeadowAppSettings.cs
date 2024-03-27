using Meadow.Cloud;
using System.Collections.Generic;

namespace Meadow;

internal class MeadowAppSettings : IAppSettings
{
    public ILoggingSettings LoggingSettings { get; set; } = new MeadowLoggingSettings();
    public ILifecycleSettings LifecycleSettings { get; set; } = new MeadowLifecycleSettings();
    public IMeadowCloudSettings MeadowCloudSettings { get; set; } = new MeadowCloudSettings();
    public Dictionary<string, string> Settings { get; set; } = new();
}
