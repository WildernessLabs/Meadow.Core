using System;
using System.IO;
using System.Threading;
using Meadow.Devices;

namespace Meadow
{
    public static partial class MeadowOS
    {
        private static SynchronizationContext? synchronizationContext;

        public static IMeadowDevice CurrentDevice { get; set; } = null!;


        public static void Sleep(DateTime until)
        {
            throw new NotImplementedException();
        }

        public static void Sleep(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public static void Sleep(WakeUpOptions wakeUp)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resets the Meadow hardware immediately
        /// </summary>
        [Obsolete("Use CurrentDevice.Reset", false)]
        public static void Reset()
        {
            CurrentDevice.Reset();
        }

        /// <summary>
        /// Enables a watchdog timer with the specified timeout in milliseconds.
        /// If Watchdog.Reset is not called before the timeout period, the Meadow
        /// will reset.
        /// </summary>
        /// <param name="timeoutMs">Watchdog timeout period, in milliseconds.
        /// Maximum allowed timeout of 32,768ms</param>
        [Obsolete("Use CurrentDevice.Reset", false)]
        public static void WatchdogEnable(int timeoutMs)
        {
            CurrentDevice.WatchdogEnable(TimeSpan.FromMilliseconds(timeoutMs));
        }

        public static void WatchdogReset()
        {
            CurrentDevice.WatchdogReset();
        }

        /// <summary>
        /// Creates the named OS directories if they don't exist, and makes sure
        /// the `/Temp` directory is emptied out.
        /// </summary>
        internal static void InitializeFileSystem()
        {
            Console.WriteLine("Initializing file system...");

            CreateFolderIfNeeded(FileSystem.CacheDirectory);
            CreateFolderIfNeeded(FileSystem.DataDirectory);
            CreateFolderIfNeeded(FileSystem.DocumentsDirectory);
            EmptyDirectory(FileSystem.TempDirectory);
            CreateFolderIfNeeded(FileSystem.TempDirectory);

            Console.WriteLine("File system initialized.");
        }

        private static void EmptyDirectory(string path)
        {
            if (Directory.Exists(path)) {
                foreach (var file in Directory.GetFiles(path)) {
                    File.Delete(file);
                }
            }
        }

        private static void CreateFolderIfNeeded(string path)
        {
            if (!Directory.Exists(path)) {
                Console.WriteLine($"Directory doesn't exist, creating '{path}'");
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"  Failed: {ex.Message}");
                }
            }
        }

        //==== Synchronization context

        internal static void SetSynchronizationContext(SynchronizationContext context)
        {
            synchronizationContext = context;
        }

        /// <summary>
        /// Runs the specified action on the main thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void BeginInvokeOnMainThread(Action action)
        {
            if (synchronizationContext == null) {
                action();
            } else {
                synchronizationContext.Send(delegate { action(); }, null);
            }
        }
    }
}
