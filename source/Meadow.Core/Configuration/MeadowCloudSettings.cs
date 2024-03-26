using Meadow.Cloud;

namespace Meadow;

internal class MeadowCloudSettings : IMeadowCloudSettings
{
    public bool Enabled { get; set; } = false;
    public string MqttHostname { get; set; } = "mqtt.meadowcloud.co";
    public string AuthHostname { get; set; } = "https://www.meadowcloud.co";
    public string DataHostname { get; set; } = "https://collector.meadowcloud.co";
    public bool UseAuthentication { get; set; } = true;
    public bool EnableUpdates { get; set; } = false;
    public bool EnableHealthMetrics { get; set; } = false;
    public int HealthMetricsIntervalMinutes { get; set; } = 60;
    public int MqttPort { get; set; } = 8883;
    public int ConnectRetrySeconds { get; set; } = 15;
    public int AuthTimeoutSeconds { get; set; } = 120;
}