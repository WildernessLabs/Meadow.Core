namespace Meadow.Gateways.Bluetooth;

/// <summary>
/// Represents the definition of a Bluetooth device.
/// </summary>
public class Definition : IDefinition
{
    /// <summary>
    /// Gets the device name.
    /// </summary>
    public string DeviceName { get; }

    /// <summary>
    /// Gets the collection of services associated with this device definition.
    /// </summary>
    public ServiceCollection Services { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Definition"/> class with the specified device name and services.
    /// </summary>
    /// <param name="deviceName">The name of the device.</param>
    /// <param name="services">The services associated with the device.</param>
    public Definition(string deviceName, params IService[] services)
    {
        DeviceName = deviceName;
        Services = new ServiceCollection();
        Services.AddRange(services);
    }

    /// <summary>
    /// Converts the device definition to a JSON string.
    /// </summary>
    /// <returns>A JSON representation of the device definition.</returns>
    public string ToJson()
    {
        // serialize to JSON, but without pulling in a JSON lib dependency
        var json = $@"{{
                        ""deviceName"":""{DeviceName}""
                    ";

        if (Services != null && Services.Count > 0)
        {
            json += ", \"services\": [";
            for (int i = 0; i < Services.Count; i++)
            {
                if (Services[i] != null)
                {
                    json += Services[i]?.ToJson();
                    if (i < (Services.Count - 1))
                    {
                        json += ",";
                    }
                }
            }
            json += "]";
        }

        json += "}";
        return json;
    }
}
