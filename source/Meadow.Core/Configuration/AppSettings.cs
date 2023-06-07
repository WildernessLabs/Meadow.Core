using Meadow.Update;

namespace Meadow
{
    /// <summary>
    /// A collection of application settings parsed from settings files at application startup
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Gets a default set of ILoggingSettings
        /// </summary>
        public static ILoggingSettings DefaultLoggingSettings => new DefaultLoggingSettings();
        /// <summary>
        /// Gets a default set of ILifecycleSettings
        /// </summary>
        public static ILifecycleSettings DefaultLifecycleSettings => new DefaultLifecycleSettings();
        /// <summary>
        /// Gets a default set of IUpdateSettings
        /// </summary>
        public static IUpdateSettings DefaultUpdateSettings => new DefaultUpdateSettings();
    }
}
