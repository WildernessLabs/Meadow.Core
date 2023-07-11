
namespace Meadow;

internal class MeadowLifecycleSettings : ILifecycleSettings
{
    public bool RestartOnAppFailure { get; set; } = false;
    public int AppFailureRestartDelaySeconds { get; set; } = 5;
}
