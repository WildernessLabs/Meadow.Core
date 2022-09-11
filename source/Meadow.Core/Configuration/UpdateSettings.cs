using Meadow.Update;

namespace Meadow
{
    public class UpdateSettings : ConfigurableObject, IUpdateSettings
    {
        public bool Enabled => GetConfiguredBool(nameof(Enabled), false);
        public string UpdateServer => GetConfiguredString(nameof(UpdateServer), "20.253.228.77");
        public int UpdatePort => GetConfiguredInt(nameof(UpdatePort), 1883);
        public string RootTopic => GetConfiguredString(nameof(RootTopic), "Meadow.OtA");
        public int CloudConnectRetrySeconds => GetConfiguredInt(nameof(CloudConnectRetrySeconds), 15);
    }
}
