using Meadow.Units;

namespace Meadow;

/// <summary>
/// Information about available storage devices
/// </summary>
public abstract class StorageInformation : IStorageInformation
{
    /// <inheritdoc/>
    public string Name { get; protected set; } = default!;

    /// <inheritdoc/>
    public DigitalStorage SpaceAvailable { get; protected set; } = default!;

    /// <inheritdoc/>
    public DigitalStorage Size { get; protected set; } = default!;
}
