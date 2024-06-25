using System.Text;

namespace Meadow.Gateways.Bluetooth;

/// <summary>
/// Represents a Bluetooth descriptor.
/// </summary>
public class Descriptor : IDescriptor
{
    /// <summary>
    /// Gets or sets the handle for the descriptor.
    /// </summary>
    public ushort Handle { get; set; }

    private CharacteristicPermission Permissions { get; }
    private CharacteristicProperty Properties { get; }
    private int Length { get; }
    private byte[] Data { get; }

    /// <summary>
    /// Gets the 128-bit unique ID for the descriptor
    /// </summary>
    public string Uuid { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Descriptor"/> class with an integer value.
    /// </summary>
    /// <param name="uuid">The UUID of the descriptor.</param>
    /// <param name="value">The integer value.</param>
    public Descriptor(string uuid, int value)
    {
        Uuid = uuid;

        // TODO: open these up
        Permissions = CharacteristicPermission.Read | CharacteristicPermission.Write;

        Length = sizeof(int);
        Data = new byte[Length];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Descriptor"/> class with a string value.
    /// </summary>
    /// <param name="uuid">The UUID of the descriptor.</param>
    /// <param name="value">The string value.</param>
    public Descriptor(string uuid, string value)
    {
        Uuid = uuid;

        // TODO: open these up
        Permissions = CharacteristicPermission.Read | CharacteristicPermission.Write;

        Data = Encoding.UTF8.GetBytes(value);
        Length = Data.Length;
    }

    /// <summary>
    /// Converts the descriptor to a JSON string.
    /// </summary>
    /// <returns>A JSON representation of the descriptor.</returns>
    public string ToJson()
    {
        // serialize to JSON, but without pulling in a JSON lib dependency
        var json = $@"{{
                        ""uuid"":10498,
                        ""permission"":{(int)Permissions},
                        ""len"":2,
                        ""value"":0
                        }}";

        return json;
    }
}
