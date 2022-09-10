using Meadow.Update;

namespace Meadow
{
    internal class LifecycleSettings : ConfigurableObject, ILifecycleSettings
    {
        public bool RestartOnAppFailure => GetConfiguredBool(nameof(RestartOnAppFailure), true);
        public int AppFailureRestartDelaySeconds => GetConfiguredInt(nameof(AppFailureRestartDelaySeconds), 5);
    }

    public class UpdateSettings : ConfigurableObject, IUpdateSettings
    {
        public string UpdateServer => GetConfiguredString(nameof(UpdateServer), "20.253.228.77");
        public int UpdatePort => GetConfiguredInt(nameof(UpdatePort), 1883);
        public string ClientID => GetConfiguredString(nameof(ClientID), "simple_client");
        public string RootTopic => GetConfiguredString(nameof(RootTopic), "Meadow.OtA");
        public int CloudConnectRetrySeconds => GetConfiguredInt(nameof(CloudConnectRetrySeconds), 15);
    }

    public class DefaultUpdateSettings : IUpdateSettings
    {
        public string UpdateServer { get; } = "20.253.228.77";
        public int UpdatePort { get; } = 1883;
        public string ClientID { get; } = "simple_client";
        public string RootTopic { get; } = "Meadow.OtA";
        public int CloudConnectRetrySeconds { get; } = 15;
    }
}
