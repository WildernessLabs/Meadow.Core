using System;

namespace Meadow.Gateways.Bluetooth;

/// <summary>
/// Represents a known Bluetooth GATT service with a name and a GUID.
/// </summary>
public struct KnownService
{
    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the GUID of the service.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KnownService"/> struct with the specified name and GUID.
    /// </summary>
    /// <param name="name">The name of the service.</param>
    /// <param name="id">The GUID of the service.</param>
    public KnownService(string name, Guid id)
    {
        Name = name;
        Id = id;
    }
}
