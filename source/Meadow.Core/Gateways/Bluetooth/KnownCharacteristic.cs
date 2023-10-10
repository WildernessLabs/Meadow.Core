using System;

namespace Meadow.Gateways.Bluetooth;

/// <summary>
/// Represents a known Bluetooth GATT characteristic with a name and a GUID.
/// </summary>
public struct KnownCharacteristic
{
    /// <summary>
    /// Gets the name of the characteristic.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the GUID of the characteristic.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KnownCharacteristic"/> struct with the specified name and GUID.
    /// </summary>
    /// <param name="name">The name of the characteristic.</param>
    /// <param name="id">The GUID of the characteristic.</param>
    public KnownCharacteristic(string name, Guid id)
    {
        Name = name;
        Id = id;
    }
}
