using System.IO;

namespace Meadow;

public static partial class MeadowOS
{
    /// <summary>
    /// Contains Meadow.OS File System information.
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// Gets the root directory of the app file system partition.
        /// </summary>
        public static string UserFileSystemRoot
        {
            get => Resolver.Device?.PlatformOS?.FileSystem?.FileSystemRoot ?? "/meadow0/";
        }

        /// <summary>
        /// Gets the `/Data` directory. Use this directory to store files that
        /// require permanent persistence, such as SQL data files, even
        /// through OS deployments and Over-the-Air (OtA) updates.
        /// </summary>
        public static string DataDirectory => Path.GetFullPath("Data", UserFileSystemRoot);

        /// <summary>
        /// Gets the `/Documents` directory. Use this directory to store files that
        /// require permanent persistence, such as application document files, even
        /// through OS deployments and Over-the-Air (OtA) updates.
        /// </summary>
        public static string DocumentsDirectory => Path.GetFullPath("Documents", UserFileSystemRoot);

        /// <summary>
        /// Gets the `/Cache` directory. Use this directory to store
        /// semi-transient files. The contents of this folder will be erased
        /// during application updates.
        /// </summary>
        public static string CacheDirectory => Path.GetFullPath("Cache", UserFileSystemRoot);

        /// <summary>
        /// Gets the `/Temp` directory. Use this directory to store transient
        /// files. 
        /// </summary>
        /// <remarks>
        /// The contents of this folder will be erased on device restart.
        /// </remarks>
        public static string TempDirectory => Path.GetFullPath("Temp", UserFileSystemRoot);

        /// <summary>
        /// Gets the `/crash` directory. This directory is used by various Meadow systems to store crash report
        /// files. 
        /// </summary>
        public static string CrashReportDirectory => Path.GetFullPath("crash", UserFileSystemRoot);

        /// <summary>
        /// Gets the full path to the file used to store crash reports from unhandled app exceptions
        /// </summary>
        public static string AppCrashFile => Path.Combine(CrashReportDirectory, "meadow.error");
        /// <summary>
        /// Gets the full path to the file used to store crash reports from unhandled runtime exceptions
        /// </summary>
        public static string RuntimeCrashFile => Path.Combine(CrashReportDirectory, "mono_error.txt");
    }
}
