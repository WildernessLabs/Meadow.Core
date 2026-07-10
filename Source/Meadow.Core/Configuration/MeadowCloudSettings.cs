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
    public int ConnectRetrySeconds { get; set; } = 5;
    // 90s covers the cold-start cost on the .NET 10 / Meadow port: the first HTTPS
    // request after boot triggers the one-time mono JIT of the SslStream/HTTP stack
    // plus a full DNS + TCP + TLS handshake, which together can take ~90s on the F7.
    // Subsequent requests are fast. Each request also forces a fresh connection
    // (PooledConnectionLifetime=Zero) to avoid a TLS connection-reuse issue; once
    // that's resolved this can drop back toward ~20s.
    public int AuthTimeoutSeconds { get; set; } = 90;
    //    public int MaxQueueDepth { get; set; } = CloudDataQueue.DefaultQueueDepth;

    public int MaximumDisconnectTimeMinutes { get; set; } = 180;
    public string TelemetryStore { get; set; } = "memory";
}