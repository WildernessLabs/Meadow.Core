using Meadow.Logging;

namespace Meadow
{
    /// <summary>
    /// Encapsulates a default set of ILogLevelSettings
    /// </summary>
    public class DefaultLogLevelSettings : ILogLevelSettings
    {
        /// <summary>
        /// Creates an instance of the DefaultLogLevelSettings
        /// </summary>
        public DefaultLogLevelSettings()
        {
        }

        /// <summary>
        /// Gets the default log level used in the system ILogger
        /// </summary>
        public string Default => LogLevel.Information.ToString();
    }
}
