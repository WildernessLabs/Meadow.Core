namespace Meadow
{
    internal class LifecycleSettings : ConfigurableObject
    {
        public bool ResetOnAppFailure => GetConfiguredBool(nameof(ResetOnAppFailure), true);
    }
}
