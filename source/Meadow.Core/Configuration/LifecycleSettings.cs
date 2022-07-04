namespace Meadow
{
    internal class LifecycleSettings : ConfigurableObject
    {
        public bool ResetOnAppFailure => GetConfiguredBool(nameof(ResetOnAppFailure), true);
        public int AppFailureRestartDelaySeconds => GetConfiguredInt(nameof(AppFailureRestartDelaySeconds), 60);
    }
}
