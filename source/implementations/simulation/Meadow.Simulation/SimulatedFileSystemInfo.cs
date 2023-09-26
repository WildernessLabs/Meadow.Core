using System;
using System.Collections.Generic;
using System.IO;

namespace Meadow.Simulation;

public class SimulatedFileSystemInfo : IPlatformOS.FileSystemInfo
{
    public override IEnumerable<IExternalStorage> ExternalStorage => throw new NotImplementedException();

    public override string FileSystemRoot { get; }

    internal SimulatedFileSystemInfo()
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
