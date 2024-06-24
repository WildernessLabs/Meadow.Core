namespace Meadow.Gateways.Bluetooth;

/// <summary>
/// Provides access to a device Bluetooth capabilities
/// </summary>
public class Service : IService
{
    /// <summary>
    /// Gets or sets the handle for the service.
    /// </summary>
    public ushort Handle { get; set; }

    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the UUID of the service.
    /// </summary>
    public ushort Uuid { get; }

    /// <summary>
    /// Gets the collection of characteristics associated with this service.
    /// </summary>
    public CharacteristicCollection Characteristics { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="Service"/> class with the specified name, UUID, and characteristics.
    /// </summary>
    /// <param name="name">The name of the service.</param>
    /// <param name="uuid">The UUID of the service.</param>
    /// <param name="characteristics">The characteristics associated with the service.</param>
    public Service(string name, ushort uuid, params ICharacteristic[] characteristics)
    {
        Name = name;
        Uuid = uuid;
        Characteristics = new CharacteristicCollection();
        Characteristics.AddRange(characteristics);
    }


    /// <summary>
    /// Converts the service to a JSON string.
    /// </summary>
    /// <returns>A JSON representation of the service.</returns>
    public string ToJson()
    {
        // serialize to JSON, but without pulling in a JSON lib dependency
        var json = $@"{{
                        ""name"":""{Name}"",
                        ""uuid"":{Uuid}
                    ";

        if (Characteristics != null && Characteristics.Count > 0)
        {
            json += ", \"characteristics\": [";
            for (int i = 0; i < Characteristics.Count; i++)
            {
                json += (Characteristics[i] as IJsonSerializable)?.ToJson();
                if (i < (Characteristics.Count - 1))
                {
                    json += ",";
                }
            }
            json += "]";
        }

        json += "}";
        return json;
    }
}
