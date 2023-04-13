using Meadow.Update;

namespace Meadow;

/// <summary>
/// A default implementation of IUpdateSettings
/// </summary>
public class DefaultUpdateSettings : IUpdateSettings
{
    /// <summary>
    /// Gets the desired enabled state of the service
    /// </summary>
    public bool Enabled { get; } = false;
    /// <summary>
    /// Gets the address of the Update server to use
    /// </summary>
    public string UpdateServer => "mqtt.meadowcloud.co";
    /// <summary>
    /// Gets the port of the Update server to use
    /// </summary>
    public int UpdatePort => 1883;
    /// <summary>
    /// Gets the address of the authentication server to use
    /// </summary>
    public string AuthServer { get; } = "https://www.meadowcloud.co";
    /// <summary>
    /// Gets the port of the authentication server to use
    /// </summary>
    public int AuthPort { get; } = 443;
    /// <summary>
    /// Gets the root MQTT topic to subscribe to for updates
    /// </summary>
    public string RootTopic { get; } = "ota";
    /// <summary>
    /// Reconnect period used when a disconnection from the Update server occrs
    /// </summary>
    public int CloudConnectRetrySeconds { get; } = 15;
    /// <summary>
    /// Gets the preference for using authentication when connecting to the Update server
    /// </summary>
    public bool UseAuthentication { get; } = true;
    /// <summary>
    /// Gets the Organization the device is registered to
    /// </summary>
    public string Organization { get; } = "Default organization";
}
