namespace Meadow
{
    using Meadow.Devices;
    using Meadow.Logging;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The entry point of the .NET part of Meadow OS.
    /// </summary>
    public static partial class MeadowOS
    {
        //==== internals
        private static SynchronizationContext? synchronizationContext;

        //==== properties
        internal static IMeadowDevice CurrentDevice { get; private set; } = null!;

        private static IApp App { get; set; }
        private static ILifecycleSettings LifecycleSettings { get; set; }

        static bool appRunning = false;

        internal static CancellationTokenSource AppAbort = new();

        public async static Task Main(string[] _)
        {
            Initialize();
            try
            {
                Resolver.Log.Trace("Initializing App");
                await App.Initialize();
            }
            catch (Exception e)
            {
                // exception on bring-up; not good
                SystemFailure(e, "App initialization failed");
            }

            while (!appRunning)
            {
                try
                {
                    Resolver.Log.Trace("Running App");
                    await App.Run();
                    appRunning = true;
                }
                catch (Exception e)
                {
                    Resolver.Log.Error($"App Error: {e.Message}");

                    App.OnError(e, out bool recovered);
                    if (recovered)
                    {
                        App.OnRecovery(e);
                    }
                    else
                    {
                        AppAbort.Cancel();
                        throw SystemFailure(e);
                    }
                }
                finally
                {
                    await Task.Delay(Timeout.Infinite, AppAbort.Token);
                }
            }

            try
            {
                Resolver.Log.Trace($"App shutting down");

                AppAbort.CancelAfter(millisecondsDelay: LifecycleSettings.AppFailureRestartDelaySeconds * 1000);
                App.OnShutdown();
            }
            catch (Exception e)
            {
                // we can't recover while shutting down
                throw SystemFailure(e, "App shutdown error");
            }
            finally
            {
                if (App is IAsyncDisposable { } da)
                {
                    await da.DisposeAsync();
                }
            }
            Shutdown();
        }

        private static void LoadSettings()
        {
            ILoggingSettings s;

            try
            {
                s = new LoggingSettings();
            }
            catch (Exception ex)
            {
                Resolver.Log.Warn(ex.Message);
                Resolver.Log.Warn("Using Default App Config");

                s = AppSettings.DefaultLoggingSettings;

            }

            if (Enum.TryParse<LogLevel>(s.LogLevel.Default, true, out LogLevel level))
            {
                Resolver.Log.Loglevel = level;
                Resolver.Log.Trace($"Setting log level to: {level}");
            }
            else
            {
                Resolver.Log.Info($"Log level: {level}");
            }

            Resolver.Log.Trace("LifecycleSettings:");

            try
            {
                LifecycleSettings = new LifecycleSettings();
            }
            catch (Exception ex)
            {
                LifecycleSettings = AppSettings.DefaultLifecycleSettings;

                Resolver.Log.Warn(ex.Message);
                Resolver.Log.Warn("Using Default App Config");
            }
            Resolver.Log.Trace($"  {nameof(LifecycleSettings.RestartOnAppFailure)}: {LifecycleSettings.RestartOnAppFailure}");
            Resolver.Log.Trace($"  {nameof(LifecycleSettings.AppFailureRestartDelaySeconds)}: {LifecycleSettings.AppFailureRestartDelaySeconds}");
        }


        private static Type FindAppType()
        {
            Resolver.Log.Trace($"Looking for app assembly...");

            // support app.exe or app.dll
            var assembly = FindByPath(new string[] { "App.exe", "App.dll", "app.exe", "app.dll" });

            if (assembly == null) throw new Exception("No 'App' assembly found.  Expected either App.exe or App.dll");

            Assembly? FindByPath(string[] namesToCheck)
            {
                var root = AppDomain.CurrentDomain.BaseDirectory;

                foreach (var name in namesToCheck)
                {
                    var path = Path.Combine(root, name);
                    if (File.Exists(path))
                    {
                        Resolver.Log.Trace($"Found '{path}'");
                        try
                        {
                            return Assembly.LoadFrom(path);
                        }
                        catch (Exception ex)
                        {
                            Resolver.Log.Warn($"Unable to load assembly '{name}': { ex.Message}");
                        }
                    }
                }

                return null;
            }

            Resolver.Log.Trace($"Looking for IApp...");
            var searchType = typeof(IApp);
            Type? appType = null;
            foreach (var type in assembly.GetTypes())
            {
                if (searchType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    appType = type;
                    break;
                }
            }

            if (appType is null)
            {
                throw new Exception("App not found. Looking for 'MeadowApp.MeadowApp' type");
            }
            return appType;
        }

        private static void Initialize()
        {
            try
            {
                Resolver.Services.Create<Logger>();
                Resolver.Log.AddProvider(new ConsoleLogProvider());

                Resolver.Log.Trace($"Initializing OS... ");
            }
            catch (Exception ex)
            {
                // must use Console because the logger failed
                Console.WriteLine($"Failed to create Logger: {ex.Message}");
            }

            try
            {
                //capture unhandled exceptions
                AppDomain.CurrentDomain.UnhandledException += StaticOnUnhandledException;

                LoadSettings();

                // Initialize strongly-typed hardware access - setup the interface module specified in the App signature
                var appType = FindAppType();

                Resolver.Log.Trace($"App is type {appType.Name}");

                var deviceType = appType.BaseType.GetGenericArguments()[0];

                if (Activator.CreateInstance(deviceType) is not IMeadowDevice device)
                {
                    throw new Exception($"Failed to create instance of '{deviceType.Name}'");
                }

                CurrentDevice = device;
                CurrentDevice.Initialize();
                CurrentDevice.PlatformOS.Initialize(); // initialize the devices' platform OS

                // initialize file system folders and such
                // TODO: move this to platformOS
                InitializeFileSystem();

                // Create the app object, bound immediately to the <IMeadowDevice>
                if (Activator.CreateInstance(appType, nonPublic: true) is not IApp app)
                {
                    throw new Exception($"Failed to create instance of '{appType.Name}'");
                }

                App = app;

                CurrentDevice.BeforeSleep += () => { app.OnSleep(); };
                CurrentDevice.AfterWake += () => { app.OnResume(); };
                CurrentDevice.BeforeReset += () => { app.OnReset(); };

                Resolver.Log.Info($"Meadow OS v.{MeadowOS.CurrentDevice.PlatformOS.OSVersion}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Shutdown()
        {
            // Do a best-attempt at freeing memory and resources
            GC.Collect(GC.MaxGeneration);
            Resolver.Log.Debug("Shutdown");
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
            if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }
            }
        }

        private static void CreateFolderIfNeeded(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Resolver.Log.Error($"Failed: {ex.Message}");
                }
            }
        }

        private static Exception SystemFailure(Exception e, string? message = null)
        {
            if (App is null)
            {
                Resolver.Log.Error("OS startup failure:");
            }
            else
            {
                Resolver.Log.Error("App failure:");
            }

            Resolver.Log.Error(message ?? " System Failure");
            Resolver.Log.Error($" {e.GetType()}: {e.Message}");
            Resolver.Log.Debug(e.StackTrace);
            if (LifecycleSettings.RestartOnAppFailure)
            {
                int restart = 5;
                if (LifecycleSettings.AppFailureRestartDelaySeconds >= 0)
                {
                    restart = LifecycleSettings.AppFailureRestartDelaySeconds;
                }

                Resolver.Log.Info($"CRASH: Meadow will restart in {restart} seconds.");
                Thread.Sleep(restart * 1000);
                CurrentDevice.Reset();
            }
            throw e; // no return from this function
        }

        private static void StaticOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            SystemFailure(e.ExceptionObject as Exception, "Unhandled exception");
        }

    }
}
