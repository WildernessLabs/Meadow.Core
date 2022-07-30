namespace Meadow
{
    internal class LifecycleSettings : ConfigurableObject
    {
        public bool RestartOnAppFailure => GetConfiguredBool(nameof(RestartOnAppFailure), true);
        public int AppFailureRestartDelaySeconds => GetConfiguredInt(nameof(AppFailureRestartDelaySeconds), 60);
    }
}
