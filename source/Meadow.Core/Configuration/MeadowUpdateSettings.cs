using Meadow.Update;

namespace Meadow;

internal class MeadowUpdateSettings : IUpdateSettings
{
    public bool Enabled { get; set; } = false;
    public string UpdateServer { get; set; } = "mqtt.meadowcloud.co";
    public int UpdatePort { get; set; } = 8883;
    public string Organization { get; set; } = "Default organization";
    public string RootTopic { get; set; } = "{OID}/ota/{ID};{OID}/commands/{ID}";
    public int CloudConnectRetrySeconds { get; set; } = 15;
    public bool UseAuthentication { get; set; } = true;
}
