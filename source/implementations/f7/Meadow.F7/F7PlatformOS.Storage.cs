namespace Meadow;

public partial class F7PlatformOS : IPlatformOS
{
    /// <summary>
    /// Gets the file system information for the platform.
    /// </summary>
    public Meadow.IPlatformOS.FileSystemInfo FileSystem { get; private set; } = default!;

}
