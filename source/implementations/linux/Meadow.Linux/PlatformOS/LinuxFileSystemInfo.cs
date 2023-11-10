using System;
using System.Collections.Generic;
using System.IO;

namespace Meadow;

public class LinuxFileSystemInfo : IPlatformOS.FileSystemInfo
{
    /// <inheritdoc/>
    public override IEnumerable<IStorageInformation> Drives => throw new NotImplementedException();

    /// <inheritdoc/>
    public override string FileSystemRoot => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".meadow");

    internal LinuxFileSystemInfo()
    {
    }
}
