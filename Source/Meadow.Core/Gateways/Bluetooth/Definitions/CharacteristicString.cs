using System.Text;

namespace Meadow.Gateways.Bluetooth;

/// <summary>
/// Represents a Bluetooth characteristic with a string value.
/// </summary>
public class CharacteristicString : Characteristic<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicString"/> class with the specified parameters.
    /// </summary>
    /// <param name="name">The name of the characteristic.</param>
    /// <param name="uuid">The UUID of the characteristic.</param>
    /// <param name="permissions">The permissions of the characteristic.</param>
    /// <param name="properties">The properties of the characteristic.</param>
    /// <param name="maxLength">The maximum length of the string value.</param>
    /// <param name="descriptors">The descriptors associated with the characteristic.</param>
    public CharacteristicString(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, int maxLength, params Descriptor[] descriptors)
        : base(name, uuid, permissions, properties, maxLength, descriptors)
    {
    }

    /// <summary>
    /// Handles the data write for the characteristic.
    /// </summary>
    /// <param name="data">The data to be written.</param>
    public override void HandleDataWrite(byte[] data)
    {
        Resolver.Log.Debug($"HandleDataWrite in {this.GetType().Name}");

        RaiseValueSet(Encoding.UTF8.GetString(data));
    }

    /// <summary>
    /// Serializes the string value to byte array.
    /// </summary>
    /// <param name="value">The string value to be serialized.</param>
    /// <returns>The byte array representing the serialized value.</returns>
    protected override byte[] SerializeValue(string value)
    {
        return Encoding.UTF8.GetBytes(value);
    }
}
