using Meadow.Cloud;
using System.Collections.Generic;

namespace Meadow
{
    /// <summary>
    /// Represents the IApp settings
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Gets the settings for Logging
        /// </summary>
        public ILoggingSettings LoggingSettings { get; }
        /// <summary>
        /// Gets the settings for the application lifecycle
        /// </summary>
        public ILifecycleSettings LifecycleSettings { get; }
        /// <summary>
        /// Gets the settings for Meadow.Cloud
        /// </summary>
        public IMeadowCloudSettings MeadowCloudSettings { get; }
        /// <summary>
        /// Gets a dictionary of user supplied settings
        /// </summary>
        public Dictionary<string, string> Settings { get; }
    }
}
