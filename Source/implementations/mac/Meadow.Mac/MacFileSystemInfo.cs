﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Meadow;

/// <summary>
/// Represents the file system information for the Mac platform
/// </summary>
public class MacFileSystemInfo : IPlatformOS.FileSystemInfo
{
    /// <inheritdoc/>
    public override IEnumerable<IStorageInformation> Drives => throw new NotImplementedException();

    /// <inheritdoc/>
    public override string FileSystemRoot { get; }

    internal MacFileSystemInfo()
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
