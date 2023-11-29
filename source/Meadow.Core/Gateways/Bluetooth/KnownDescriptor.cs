using System;

namespace Meadow.Gateways.Bluetooth;

/// <summary>
/// Represents a known Bluetooth GATT descriptor with a name and a GUID.
/// </summary>
public struct KnownDescriptor
{
    /// <summary>
    /// Gets the name of the descriptor.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the GUID of the descriptor.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KnownDescriptor"/> struct with the specified name and GUID.
    /// </summary>
    /// <param name="name">The name of the descriptor.</param>
    /// <param name="id">The GUID of the descriptor.</param>
    public KnownDescriptor(string name, Guid id)
    {
        Name = name;
        Id = id;
    }
}
