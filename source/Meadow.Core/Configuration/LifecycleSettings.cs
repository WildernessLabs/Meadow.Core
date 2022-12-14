namespace Meadow
{
    internal class LifecycleSettings : ConfigurableObject, ILifecycleSettings
    {
        public bool RestartOnAppFailure => GetConfiguredBool(nameof(RestartOnAppFailure), false);
        public int AppFailureRestartDelaySeconds => GetConfiguredInt(nameof(AppFailureRestartDelaySeconds), 5);
    }
}
