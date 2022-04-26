namespace Meadow
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Reflection;
    using Meadow.Devices;
    using System.Threading.Tasks;

    using static System.Console;

    /// <summary>
    /// The entry point of the .NET part of Meadow OS.
    /// </summary>
    public static partial class MeadowOS
    {
        //==== internals
        private static SynchronizationContext? synchronizationContext;

        //==== properties
        internal static IMeadowDevice CurrentDevice { get; private set; } = null!;

        static IApp app;

        static bool appSetup = false;
        static bool appRan = false;
        static bool appShutdown = false;

        internal static CancellationTokenSource AppAbort = new();

        public async static Task Main (string[] args)
        {
            Initialize();
            try {
                await app.Initialize();
                appSetup = true;
            }
            catch (Exception e) {
                // exception on bring-up; not good
                SystemFailure(e, "App initialization failed");
            }

            while (!appRan)
            {
                try {
                    await app.Run();
                    appRan = true;
                }
                catch (Exception e)
                {
                    bool recovered;
                    app.OnError (e, out recovered);
                    if (recovered)
                        app.Recovery(e);
                    else
                    {
                        AppAbort.Cancel();
                        throw SystemFailure(e);
                    }
                }
            }

            try {
                AppAbort.CancelAfter(millisecondsDelay: 60);
                app.Shutdown(out appShutdown);
            }
            catch (Exception e)
            {
                // we can't recover while shutting down
                throw SystemFailure(e, "App shutdown error");
            }
            finally {
                await (app as IAsyncDisposable).DisposeAsync();
            }
            Shutdown();
        }

        private static void Initialize()
        {
            Write($"Initializing... ");
            //capture unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += StaticOnUnhandledException;

            // Load 'App.dll'
            var loaded_app = Assembly.LoadFrom("/meadow0/App.exe");
            var app_type = loaded_app.GetType("MeadowApp.MeadowApp");
            if (app_type is null)
                throw new Exception ("App not found. Looking for 'MeadowApp.MeadowApp' type");

            // Initialize strongly-typed hardware access - setup the interface module specified in the App signature
            var device_type = app_type.BaseType.GetGenericArguments()[0];
            CurrentDevice = Activator.CreateInstance(device_type) as IMeadowDevice;
            CurrentDevice.Initialize();
            CurrentDevice.PlatformOS.Initialize(); // initialize the devices' platform OS

            // initialize file system folders and such
            // TODO: move this to platformOS
            InitializeFileSystem();

            // Create the app object, bound immediately to the <IMeadowDevice>
            app = Activator.CreateInstance(app_type, nonPublic: true) as IApp;

            CurrentDevice.BeforeSleep += () => { app.BeforeSleep(); };
            CurrentDevice.AfterWake += () => { app.AfterSleep(); };
            CurrentDevice.BeforeReset += () => { app.BeforeReset(); };

            WriteLine($"Meadow OS v.{MeadowOS.CurrentDevice.PlatformOS.OSVersion}");
        }

        private static void Shutdown()
        {
            app = null;
            // Do a best-attempt at freeing memory and resources
            GC.Collect(GC.MaxGeneration);
            WriteLine("Done.");
            Thread.Sleep(-1);
        }


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
        [Obsolete("Use CurrentDevice.Reset", true)]
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
        [Obsolete("Use CurrentDevice.WatchdogEnable", true)]
        public static void WatchdogReset(int timeoutMs)
        {
            CurrentDevice.WatchdogEnable(TimeSpan.FromMilliseconds(timeoutMs));
        }

        [Obsolete("Use CurrentDevice.WatchdogReset", true)]
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
            CreateFolderIfNeeded(FileSystem.CacheDirectory);
            CreateFolderIfNeeded(FileSystem.DataDirectory);
            CreateFolderIfNeeded(FileSystem.DocumentsDirectory);
            EmptyDirectory(FileSystem.TempDirectory);
            CreateFolderIfNeeded(FileSystem.TempDirectory);
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
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch(Exception ex)
                {
                    WriteLine($"Failed: {ex.Message}");
                }
            }
        }

        private static Exception SystemFailure (Exception e, string? message = null)
        {
            if (app is null)
                Write("OS startup failure:");
            else
                Write("App failure:");

            WriteLine(message);
            WriteLine($"{e.GetType()}: {e.Message}");
            WriteLine(e.StackTrace);
            WriteLine("App failure. Meadow will restart in 5 seconds.");
            Thread.Sleep(5000);
            CurrentDevice.Reset();
            throw e; // no return from this function
        }

        private static void StaticOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            SystemFailure(e.ExceptionObject as Exception, "Unhandled exception");
        }

    }
}
