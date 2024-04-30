using Meadow.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Meadow;

public class LinuxStorageInformation : StorageInformation
{
    internal LinuxStorageInformation(string name, string filesystem, DigitalStorage size, DigitalStorage spaceAvailable)
        : base(name, size, spaceAvailable)
    {
        Filesystem = filesystem;
    }

    public string Filesystem { get; private set; }
}

public class LinuxFileSystemInfo : IPlatformOS.FileSystemInfo
{
    /// <inheritdoc/>
    public override IEnumerable<IStorageInformation> Drives => GetDrives();

    /// <inheritdoc/>
    public override string FileSystemRoot => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".meadow");

    internal LinuxFileSystemInfo()
    {

    }

    public static IEnumerable<IStorageInformation> GetDrives()
    {
        // any reported drive with one of the Filesystems will be ignored
        var ignoreTypes = new string[]
            {
                "none",
                "rootfs",
                "devtmpfs",
                "tmpfs",
                "snapfuse"
            };

        /*
        $ df /
        Filesystem     1K-blocks    Used Available Use% Mounted on
        /dev/root       30431968 4444388  24692740  16% /
        */
        var result = Linux.ExecuteCommandLine("df", "-k /");
        var lines = result.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var list = new List<IStorageInformation>();

        for (var i = 1; i < lines.Length; i++)
        {
            var elements = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (elements != null && elements.Length == 6)
            {
                if (ignoreTypes.Contains(elements[0])) continue;
                var size = int.Parse(elements[1]);
                var available = int.Parse(elements[3]);
                list.Add(new LinuxStorageInformation(
                    elements[5],
                    elements[0],
                    new DigitalStorage(size, DigitalStorage.UnitType.KibiBytes),
                    new DigitalStorage(available, DigitalStorage.UnitType.KibiBytes)
                ));
            }
        }
        return list;
    }
}
