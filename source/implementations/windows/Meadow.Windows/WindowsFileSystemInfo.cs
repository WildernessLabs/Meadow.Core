using System;
using System.Collections.Generic;
using System.IO;

namespace Meadow;

public class WindowsFileSystemInfo : IPlatformOS.FileSystemInfo
{
    /// <inheritdoc/>
    public override IEnumerable<IStorageInformation> Drives => throw new NotImplementedException();

    /// <inheritdoc/>
    public override string FileSystemRoot { get; }

    internal WindowsFileSystemInfo()
    {
        // create the Meadow root folder
        var di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Meadow"));
        if (!di.Exists)
        {
            di.Create();
        }

        FileSystemRoot = di.FullName;
    }
}
