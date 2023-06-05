using System;
using System.Collections.Generic;
using System.IO;

namespace Meadow
{
    public class WindowsFileSystemInfo : IPlatformOS.FileSystemInfo
    {
        public override IEnumerable<IExternalStorage> ExternalStorage => throw new NotImplementedException();

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
}
