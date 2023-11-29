using System;

namespace Meadow.Gateways.Bluetooth;

/// <summary>
/// Represents a Bluetooth characteristic with an integer (Int32) value.
/// </summary>
public class CharacteristicInt32 : Characteristic<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicInt32"/> class with the specified parameters.
    /// </summary>
    /// <param name="name">The name of the characteristic.</param>
    /// <param name="uuid">The UUID of the characteristic.</param>
    /// <param name="permissions">The permissions of the characteristic.</param>
    /// <param name="properties">The properties of the characteristic.</param>
    /// <param name="descriptors">The descriptors associated with the characteristic.</param>
    public CharacteristicInt32(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, params Descriptor[] descriptors)
        : base(name, uuid, permissions, properties, descriptors)
    {
    }

    /// <summary>
    /// Handles the data write for the characteristic.
    /// </summary>
    /// <param name="data">The data to be written.</param>
    public override void HandleDataWrite(byte[] data)
    {
        Resolver.Log.Info($"HandleDataWrite in {this.GetType().Name}");

        // TODO: if the written data isn't 4 bytes, then what??
        // for now I'll right-pad with zeros
        if (data.Length < 4)
        {
            Resolver.Log.Info($"HandleDataWrite only got {data.Length} bytes - padding");
            var temp = new byte[4];
            Array.Copy(data, temp, data.Length);
            RaiseValueSet(BitConverter.ToInt32(temp));
        }
        else
        {
            if (data.Length > 4)
            {
                Resolver.Log.Info($"HandleDataWrite got {data.Length} bytes - using only the first 4");
            }
            RaiseValueSet(BitConverter.ToInt32(data));
        }
    }

    /// <summary>
    /// Serializes the integer value to a byte array.
    /// </summary>
    /// <param name="value">The integer value to be serialized.</param>
    /// <returns>The byte array representing the serialized value.</returns>
    protected override byte[] SerializeValue(int value)
    {
        return BitConverter.GetBytes(value);
    }
}
