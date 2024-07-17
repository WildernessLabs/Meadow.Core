using Meadow.Units;

namespace Meadow;

/// <summary>
/// Provides storage information specific to Linux systems.
/// </summary>
public class LinuxStorageInformation : StorageInformation
{
    internal LinuxStorageInformation(string name, string filesystem, DigitalStorage size, DigitalStorage spaceAvailable)
        : base(name, size, spaceAvailable)
    {
        Filesystem = filesystem;
    }

    /// <inheritdoc/>
    public string Filesystem { get; private set; }
}
