using Meadow.Units;

namespace Meadow;

/// <summary>
/// Information about available storage devices
/// </summary>
public abstract class StorageInformation : IStorageInformation
{
    /// <summary>
    /// Crreates a StorageInformation instance
    /// </summary>
    /// <param name="name">The store name</param>
    /// <param name="size">The total size of the store</param>
    /// <param name="spaceAvailable">The available space in the store</param>
    protected StorageInformation(string name, DigitalStorage size, DigitalStorage spaceAvailable)
    {
        Name = name;
        Size = size;
        SpaceAvailable = spaceAvailable;
    }

    /// <inheritdoc/>
    public string Name { get; protected set; } = default!;

    /// <inheritdoc/>
    public DigitalStorage SpaceAvailable { get; protected set; } = default!;

    /// <inheritdoc/>
    public DigitalStorage Size { get; protected set; } = default!;
}
