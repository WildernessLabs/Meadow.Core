using Meadow.Cloud;

namespace Meadow;

internal class MeadowCloudSettings : IMeadowCloudSettings
{
    public string Hostname { get; set; } = "https://www.meadowcloud.co";
    public string DataHostname { get; set; } = "https://collector.meadowcloud.co";
    public bool EnableHealthMetrics { get; set; } = false;
    public int HealthMetricsInterval { get; set; } = 60;
}