namespace Meadow.Gateways.Bluetooth;

/// <summary>
/// Represents a Bluetooth characteristic with a boolean value.
/// </summary>
public class CharacteristicBool : Characteristic<bool>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicBool"/> class with the specified parameters.
    /// </summary>
    /// <param name="name">The name of the characteristic.</param>
    /// <param name="uuid">The UUID of the characteristic.</param>
    /// <param name="permissions">The permissions of the characteristic.</param>
    /// <param name="properties">The properties of the characteristic.</param>
    /// <param name="descriptors">The descriptors associated with the characteristic.</param>
    public CharacteristicBool(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, params Descriptor[] descriptors)
        : base(name, uuid, permissions, properties, descriptors)
    {
    }

    /// <summary>
    /// Handles the data write for the characteristic.
    /// </summary>
    /// <param name="data">The data to be written.</param>
    public override void HandleDataWrite(byte[] data)
    {
        Resolver.Log.Debug($"HandleDataWrite in {this.GetType().Name}");
        RaiseValueSet(data[0] != 0);
    }

    /// <summary>
    /// Serializes the boolean value to a byte array.
    /// </summary>
    /// <param name="value">The boolean value to be serialized.</param>
    /// <returns>The byte array representing the serialized value.</returns>
    protected override byte[] SerializeValue(bool value)
    {
        return new byte[] { (byte)(value ? 1 : 0) };
    }
}
