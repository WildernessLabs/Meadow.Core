namespace Meadow
{
    using Meadow.Logging;
    using Meadow.Update;
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
        private static bool _startedDirectly = false;  // true when this assembly is the entry point

        //==== properties
        internal static IMeadowDevice CurrentDevice { get; private set; } = null!;

        private static IApp App { get; set; }
        private static ILifecycleSettings LifecycleSettings { get; set; }
        private static IUpdateSettings UpdateSettings { get; set; }

        internal static CancellationTokenSource AppAbort = new();

        /// <summary>
        /// The entry point for Meadow applications
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public async static Task Main(string[] _)
        {
            _startedDirectly = true;
            await Start();
        }

        /// <summary>
        /// Initializes and starts up the Meadow Core software stack
        /// </summary>
        /// <returns></returns>
        public async static Task Start()
        {
            bool systemInitialized = false;
            try
            {
                systemInitialized = Initialize();

                if (!systemInitialized)
                {
                    // device initialization failed - don't try bring up the app
                    Resolver.Log.Error("Device (system) Initialization Failure");
                }
            }
            catch (Exception e)
            {
                ReportAppException(e, "Device (system) Initialization Failure");
            }

            if (systemInitialized)
            {
                var stepName = "App Initialize";

                try
                {
                    Resolver.Log.Trace("Initializing App");
                    await App.Initialize();

                    stepName = "App Run";

                    Resolver.Log.Trace("Running App");
                    await App.Run();

                    while (!AppAbort.IsCancellationRequested)
                    {
                        await Task.Delay(500);
                    }

                    // the user's app has exited, which is almost certainly not intended
                    Resolver.Log.Warn("AppAbort cancellation has been requested");
                }
                catch (Exception e)
                {
                    ReportAppException(e, $"App Error in {stepName}: {e.Message}");

                    try
                    {
                        // let the app handle error conditions
                        await App.OnError(e);
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Error($"Exception in OnError handling: {ex.Message}");
                    }

                    // set the cancel token so any waiting Tasks, etc can abort cleanly
                    AppAbort.Cancel();
                }

                try
                {
                    // let the app handle shutdown
                    Resolver.Log.Trace($"App shutting down");

                    AppAbort.CancelAfter(millisecondsDelay: LifecycleSettings.AppFailureRestartDelaySeconds * 1000);
                    await App.OnShutdown();
                }
                catch (Exception e)
                {
                    // we can't recover while shutting down
                    Resolver.Log.Error($"Exception in App.OnShutdown: {e.Message}");
                }
                finally
                {
                    if (App is IAsyncDisposable { } da)
                    {
                        try
                        {
                            await da.DisposeAsync();
                        }
                        catch (Exception ex)
                        {
                            Resolver.Log.Error($"Exception in App.Dispose: {ex.Message}");
                        }
                    }
                }
            }

            // final shutdown process
            Shutdown();

            // we should never get to this point
        }

        private static void ReportAppException(Exception e, string? message = null)
        {
            Resolver.Log.Error(message ?? " System Failure");
            Resolver.Log.Error($" {e.GetType()}: {e.Message}");
            Resolver.Log.Error(e.StackTrace);

            if (e is AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    Resolver.Log.Error($" Inner {ex.GetType()}: {ex.InnerException.Message}");
                    Resolver.Log.Error(ex.StackTrace);
                }
            }
            else if (e.InnerException != null)
            {
                Resolver.Log.Error($" Inner {e.InnerException.GetType()}: {e.InnerException.Message}");
                Resolver.Log.Error(e.InnerException.StackTrace);
            }
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

            Resolver.Log.ShowTicks = s.ShowTicks;

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

            try
            {
                UpdateSettings = new UpdateSettings();
                Resolver.Log.Trace($"Using Update Server {UpdateSettings.UpdateServer}:{UpdateSettings.UpdatePort}");
            }
            catch (Exception ex)
            {
                UpdateSettings = AppSettings.DefaultUpdateSettings;

                Resolver.Log.Warn(ex.Message);
                Resolver.Log.Warn("Using Default Update Config");
            }
        }


        private static Type FindAppType()
        {
            Resolver.Log.Trace($"Looking for app assembly...");

            Assembly appAssembly;

            if (_startedDirectly)
            {
                // support app.exe or app.dll
                appAssembly = FindByPath(new string[] { "App.dll", "App.exe", "app.dll", "app.exe" });

                if (appAssembly == null) throw new Exception("No 'App' assembly found.  Expected either App.exe or App.dll");
            }
            else
            {
                appAssembly = Assembly.GetEntryAssembly();
            }

            // === LOCAL METHOD ===
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
                            Resolver.Log.Warn($"Unable to load assembly '{name}': {ex.Message}");
                        }
                    }
                }

                return null;
            }

            Resolver.Log.Trace($"Looking for IApp...");
            var searchType = typeof(IApp);
            Type? appType = null;
            foreach (var type in appAssembly.GetTypes())
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

        private static bool Initialize()
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
                Resolver.Log.Error($"Failed to create Logger: {ex.Message}");
                return false;
            }

            try
            {
                //capture unhandled exceptions
                AppDomain.CurrentDomain.UnhandledException += StaticOnUnhandledException;

                LoadSettings();

                // Initialize strongly-typed hardware access - setup the interface module specified in the App signature
                var b4 = Environment.TickCount;
                var et = Environment.TickCount - b4;
                var appType = FindAppType();
                Resolver.Log.Trace($"App is type {appType.Name}");
                Resolver.Log.Trace($"Finding '{appType.Name}' took {et}ms");

                var deviceType = appType.BaseType.GetGenericArguments()[0];

                try
                {
                    if (Activator.CreateInstance(deviceType) is not IMeadowDevice device)
                    {
                        throw new Exception($"Failed to create instance of '{deviceType.Name}'");
                    }
                    et = Environment.TickCount - b4;
                    Resolver.Log.Trace($"Creating '{deviceType.Name}' instance took {et}ms");

                    CurrentDevice = device;
                    Resolver.Services.Add<IMeadowDevice>(CurrentDevice);
                }
                catch (Exception)
                {
                    return false;
                }

                Resolver.Log.Trace($"Device Initialize starting...");
                CurrentDevice.Initialize();
                Resolver.Log.Trace($"PlatformOS Initialize starting...");
                CurrentDevice.PlatformOS.Initialize(CurrentDevice.Capabilities); // initialize the devices' platform OS

                // initialize file system folders and such
                // TODO: move this to platformOS
                Resolver.Log.Trace($"File system Initialize starting...");
                InitializeFileSystem();

                // Create the app object, bound immediately to the <IMeadowDevice>
                b4 = Environment.TickCount;
                Resolver.Log.Trace($"Creating instance of {appType.Name}...");

                if (Activator.CreateInstance(appType, nonPublic: true) is not IApp app)
                {
                    throw new Exception($"Failed to create instance of '{appType.Name}'");
                }
                et = Environment.TickCount - b4;
                Resolver.Log.Trace($"Creating '{appType.Name}' instance took {et}ms");

                // feels off, but not seeing a super clean way without the generics, etc.
                if (app.GetType().GetProperty(nameof(App.CancellationToken)) is PropertyInfo pi)
                {
                    pi.SetValue(app, AppAbort.Token);
                }

                App = app;

                var updateService = new UpdateService(UpdateSettings);
                Resolver.Services.Add<IUpdateService>(updateService);

                Resolver.Log.Info($"Update Service is {(UpdateSettings.Enabled ? "enabled" : "disabled")}.");
                if (UpdateSettings.Enabled)
                {
                    updateService.Start();
                }

                return true;
            }
            catch (Exception e)
            {
                Resolver.Log.Error(e.ToString());
                return false;
            }
        }

        private static void Shutdown()
        {
            // schedule a device restart if possible and if the user hasn't disabled it
            ScheduleRestart();

            // Do a best-attempt at freeing memory and resources
            GC.Collect(GC.MaxGeneration);
            Resolver.Log.Debug("Shutdown");

            // just put the Main thread to sleep (don't exit) because we want the Update service to still be able to work
            Thread.Sleep(Timeout.Infinite);
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

        private static void ScheduleRestart()
        {
            if (LifecycleSettings.RestartOnAppFailure)
            {
                int restart = 5;
                if (LifecycleSettings.AppFailureRestartDelaySeconds >= 0)
                {
                    restart = LifecycleSettings.AppFailureRestartDelaySeconds;
                }

                if (CurrentDevice != null && CurrentDevice.PlatformOS != null)
                {
                    Resolver.Log.Info($"CRASH: Meadow will restart in {restart} seconds.");
                    Thread.Sleep(restart * 1000);

                    CurrentDevice.PlatformOS.Reset();
                }
                else
                {
                    Resolver.Log.Info($"Initialization failure prevents automatic restart.");
                }
            }
        }

        private static void StaticOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // unsure how this would ever happen, but trap it anyway
            if (e.ExceptionObject is Exception ex)
            {
                ReportAppException(ex, "UnhandledException");
            }

            // final shutdown - which really is just an infinite Sleep()
            Shutdown();
        }

    }
}
