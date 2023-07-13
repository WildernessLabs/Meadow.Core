using Meadow.Update;
using System.Collections.Generic;

namespace Meadow
{
    public interface IAppSettings
    {
        public ILoggingSettings LoggingSettings { get; }
        public ILifecycleSettings LifecycleSettings { get; }
        public IUpdateSettings UpdateSettings { get; }
        public Dictionary<string, string> Settings { get; }
    }
}
