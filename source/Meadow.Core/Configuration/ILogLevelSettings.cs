using Meadow.Logging;

namespace Meadow;

/// <summary>
/// Represents the interface for log level settings.
/// </summary>
public interface ILogLevelSettings
{
    /// <summary>
    /// Gets or sets the default log level.
    /// </summary>
    LogLevel Default { get; set; }
}
