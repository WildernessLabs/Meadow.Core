namespace Meadow
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Reflection;
    using Meadow.Devices;
    using System.Threading.Tasks;

    using static System.Console;
    using System.Linq;
    using Meadow.Logging;

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

        static bool appSetup = false;
        static bool appRan = false;
        static bool appShutdown = false;

        internal static CancellationTokenSource AppAbort = new();

        public async static Task Main(string[] args)
        {
            Resolver.Services.Create<Logger>();
            Resolver.Log.AddProvider(new ConsoleLogProvider());

            // TODO: pull from file/config
            Resolver.Log.Loglevel = LogLevel.Verbose;

            Initialize();
            try
            {
                Resolver.Log.Verbose("Initializing App");
                await App.Initialize();
                appSetup = true;
            }
            catch (Exception e)
            {
                // exception on bring-up; not good
                SystemFailure(e, "App initialization failed");
            }

            while (!appRan)
            {
                try
                {
                    Resolver.Log.Verbose("Running App");
                    await App.Run();
                    appRan = true;
                }
                catch (Exception e)
                {
                    Resolver.Log.Error($"App Error: {e.Message}");
                    
                    bool recovered;
                    App.OnError(e, out recovered);
                    if (recovered)
                    {
                        App.Recovery(e);
                    }
                    else
                    {
                        AppAbort.Cancel();
                        throw SystemFailure(e);
                    }
                }
            }

            try
            {
                Resolver.Log.Verbose($"App shutting down");

                AppAbort.CancelAfter(millisecondsDelay: 60);
                App.Shutdown(out appShutdown);
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
     
        private static Type FindAppType()
        {
            Resolver.Log.Verbose($"Looking for app assembly...");

            // support app.exe or app.dll
            var assembly = FindByPath(new string[] { "App.exe", "App.dll", "app.exe", "app.dll" });

            if(assembly == null) throw new Exception("No 'App' assembly found.  Expected either App.exe or App.dll");

            Assembly? FindByPath(string[] namesToCheck)
            {
                var root = AppDomain.CurrentDomain.BaseDirectory;

                foreach (var name in namesToCheck)
                {
                    var path = Path.Combine(root, name);
                    if (File.Exists(path))
                    {
                        Resolver.Log.Verbose($"Found '{path}'");
                        return Assembly.LoadFrom(path);
                    }
                }

                return null;
            }

            Resolver.Log.Verbose($"Looking for IApp...");
            var types = assembly.GetTypes();
            var app_type = types.FirstOrDefault(t => typeof(IApp).IsAssignableFrom(t));

            if (app_type is null)
            {
                throw new Exception("App not found. Looking for 'MeadowApp.MeadowApp' type");
            }
            return app_type;
        }

        private static void Initialize()
        {
            Resolver.Log.Verbose($"Initializing OS... ");

            try
            {
                //capture unhandled exceptions
                AppDomain.CurrentDomain.UnhandledException += StaticOnUnhandledException;


                // Initialize strongly-typed hardware access - setup the interface module specified in the App signature
                var app_type = FindAppType();

                var device_type = app_type.BaseType.GetGenericArguments()[0];
                var device = Activator.CreateInstance(device_type) as IMeadowDevice;
                if(device == null)
                {
                    throw new Exception($"Failed to create instance of '{device_type.Name}'");
                }
                CurrentDevice = device;
                CurrentDevice.Initialize();
                CurrentDevice.PlatformOS.Initialize(); // initialize the devices' platform OS

                // initialize file system folders and such
                // TODO: move this to platformOS
                InitializeFileSystem();

                // Create the app object, bound immediately to the <IMeadowDevice>
                var app = Activator.CreateInstance(app_type, nonPublic: true) as IApp;
                if(app == null)
                {
                    throw new Exception($"Failed to create instance of '{app_type.Name}'");
                }

                App = app;

                CurrentDevice.BeforeSleep += () => { app.BeforeSleep(); };
                CurrentDevice.AfterWake += () => { app.AfterSleep(); };
                CurrentDevice.BeforeReset += () => { app.BeforeReset(); };

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
            if (App is null)
            {
                Resolver.Log.Error("OS startup failure:");
            }
            else
            {
                Resolver.Log.Error("App failure:");
            }

            Resolver.Log.Error(message);
            Resolver.Log.Debug($"{e.GetType()}: {e.Message}");
            Resolver.Log.Debug(e.StackTrace);
            Resolver.Log.Info("App failure. Meadow will restart in 5 seconds.");
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
