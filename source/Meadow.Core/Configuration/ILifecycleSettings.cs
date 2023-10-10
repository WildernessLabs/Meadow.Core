namespace Meadow;

/// <summary>
/// Represents the interface for lifecycle settings.
/// </summary>
public interface ILifecycleSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether to restart on application failure.
    /// </summary>
    bool RestartOnAppFailure { get; set; }

    /// <summary>
    /// Gets or sets the delay, in seconds, for restarting after application failure.
    /// </summary>
    int AppFailureRestartDelaySeconds { get; set; }
}
