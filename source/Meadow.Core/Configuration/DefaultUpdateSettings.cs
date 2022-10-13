using Meadow.Update;

namespace Meadow
{
    public class DefaultUpdateSettings : IUpdateSettings
    {
        public bool Enabled { get; } = false;
        public string UpdateServer { get; } = "20.253.228.77";
        public int UpdatePort { get; } = 1883;
        public string RootTopic { get; } = "Meadow.OtA";
        public int CloudConnectRetrySeconds { get; } = 15;
    }
}
