namespace Meadow;

/// <summary>
/// Represents the interface for logging settings.
/// </summary>
public interface ILoggingSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether to show ticks in logs.
    /// </summary>
    bool ShowTicks { get; set; }

    /// <summary>
    /// Gets the log level settings.
    /// </summary>
    ILogLevelSettings LogLevel { get; }
}
