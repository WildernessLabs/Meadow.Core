using Meadow.Cloud;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Peripherals.Sensors;
using Meadow.Update;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static Meadow.Logging.Logger;
using RTI = System.Runtime.InteropServices.RuntimeInformation;
namespace Meadow;

/// <summary>
/// The entry point of the .NET part of Meadow OS.
/// </summary>
public static partial class MeadowOS
{
    //==== internals
    private static bool _startedDirectly = false;  // true when this assembly is the entry point

    //==== properties
    internal static IMeadowDevice CurrentDevice { get; private set; } = null!;

    private static IApp App { get; set; } = default!;
    private static ILifecycleSettings LifecycleSettings { get; set; } = default!;
    private static IMeadowCloudSettings MeadowCloudSettings { get; set; } = default!;

    /// <summary>
    /// The cancellation CancellationTokenSource used to signal the application to shut down
    /// </summary>
    /// <remarks>This is used by the UpdateService</remarks>
    public static CancellationTokenSource AppAbort = new();

    /// <summary>
    /// The value of the processor clock register when `Main` was entered
    /// </summary>
    public static int StartupTick { get; set; }

    /// <summary>
    /// The entry point for Meadow applications
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns></returns>
    public static async Task Main(string[] args)
    {
        StartupTick = Environment.TickCount;

        _startedDirectly = true;
        await Start(args);
    }

    /// <summary>
    /// Initializes and starts up the Meadow Core software stack
    /// </summary>
    public static Task Start(string[]? args)
    {
        return Start(args, null);
    }

    /// <summary>
    /// Initializes and starts up the Meadow Core software stack
    /// </summary>
    public static Task Start(IApp app, string[]? args = null)
    {
        return Start(args, app);
    }

    /// <summary>
    /// Initializes and starts up the Meadow Core software stack
    /// </summary>
    private static async Task Start(string[]? args, IApp? app)
    {
        bool systemInitialized = false;
        try
        {
            systemInitialized = Initialize(args, app);

            if (!systemInitialized)
            {
                // device initialization failed - don't try bring up the app
                Resolver.Log.Error("Device (system) Initialization Failure", MessageGroup.Core);
            }
        }
        catch (Exception e)
        {
            ReportAppException(e, "Device (system) Initialization Failure");
        }

        var reliabilityService = Resolver.Services.Get<IReliabilityService>();

        if (reliabilityService != null)
        {
            try
            {
                // check for any crash reports
                if (reliabilityService.IsCrashDataAvailable)
                {
                    reliabilityService.OnBootFromCrash();
                }
            }
            catch (Exception ex)
            {
                // if the app crashes in the crash report handler, we don't want to restart or we'll infinite loop!
                Resolver.Log.Error($"IReliabilityService.HandleBootFromError error: {ex.Message}", MessageGroup.Core);
            }
        }

        Resolver.Services.Add<ISensorService>(new SensorService());

        if (systemInitialized)
        {
            var stepName = "App Initialize";

            try
            {
                Resolver.Log.Trace("Initializing App", MessageGroup.Core);
                await App.Initialize();

                stepName = "App Run";

                Resolver.Log.Trace("Running App", MessageGroup.Core);

                await App.Run();
                AppAbort.Token.WaitHandle.WaitOne();
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
                    Resolver.Log.Error($"Exception in OnError handling: {ex.Message}", MessageGroup.Core);
                }

                // set the cancel token so any waiting Tasks, etc can abort cleanly
                AppAbort.Cancel();
            }

            try
            {
                // let the app handle shutdown
                Resolver.Log.Trace($"App shutting down", MessageGroup.Core);

                AppAbort.CancelAfter(millisecondsDelay: LifecycleSettings.AppFailureRestartDelaySeconds * 1000);
                await App.OnShutdown();
            }
            catch (Exception e)
            {
                // we can't recover while shutting down
                Resolver.Log.Error($"Exception in App.OnShutdown: {e.Message}", MessageGroup.Core);
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
                        Resolver.Log.Error($"Exception in App.Dispose: {ex.Message}", MessageGroup.Core);
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
        try
        {
            Resolver.Log.Error(message ?? " System Failure", MessageGroup.Core);
            Resolver.Log.Error($" {e.GetType()}: {e.Message}");
            Resolver.Log.Error(e.StackTrace);

            if (e is AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    Resolver.Log.Error($" Inner {ex.GetType()}: {ex.InnerException.Message}", MessageGroup.Core);
                    Resolver.Log.Error(ex.StackTrace);
                }
            }
            else if (e.InnerException != null)
            {
                Resolver.Log.Error($" Inner {e.InnerException.GetType()}: {e.InnerException.Message}", MessageGroup.Core);
                Resolver.Log.Error(e.InnerException.StackTrace);
            }
        }
        catch
        {
            // DEV NOTE (21 May, 2024):
            // I've seen the Console.Writeline call end in a `System.IO.IOException: Write fault on path / [Unknown]` in testing
            // this block is to protect against that crashing - but there's little we can do about it
            // This code path is part of a shutdown, so the app should be restarting anyway
        }

        try
        {
            var di = new DirectoryInfo(Path.GetDirectoryName(MeadowOS.FileSystem.AppCrashFile));
            if (!di.Exists) { di.Create(); }

            using var file = File.CreateText(MeadowOS.FileSystem.AppCrashFile);
            file.Write(e.ToString());
        }
        catch
        {
            // we're already in an error condition - nothing we can really do about this
        }
    }

    private static Dictionary<string, string> LoadSettings()
    {
        IAppSettings settings;

        var configPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "app.config.yaml");

        if (File.Exists(configPath))
        {
            Console.WriteLine($"Parsing app.config.yaml...");

            // testing shows this takes ~375ms (7/20/23) on F7
            var yml = File.ReadAllText(configPath);

            // testing shows this takes ~1100ms (7/20/23)
            settings = AppSettingsParser.Parse(yml);
        }
        else
        {
            Console.WriteLine($"Using default app.config.yaml...");
            settings = new MeadowAppSettings(); // defaults
        }

        Resolver.Log.ShowTicks = settings.LoggingSettings.ShowTicks;

        Resolver.Log.LogLevel = settings.LoggingSettings.LogLevel.Default;
        Resolver.Log.Info($"Log level: {settings.LoggingSettings.LogLevel.Default}", MessageGroup.Core);

        LifecycleSettings = settings.LifecycleSettings;
        MeadowCloudSettings = settings.MeadowCloudSettings;

        // cascade cloud enable
        if (MeadowCloudSettings.EnableUpdates)
        {
            MeadowCloudSettings.Enabled = true;
        }
        if (MeadowCloudSettings.EnableHealthMetrics)
        {
            MeadowCloudSettings.Enabled = true;
        }

        return settings.Settings;
    }

    private static Type[] FindAppType(string? root)
    {
        Resolver.Log.Trace($"Looking for app assembly...", MessageGroup.Core);

        Assembly? appAssembly;

        if (_startedDirectly)
        {
            // support app.exe or app.dll
            appAssembly = FindByPath(new string[] { "App.dll", "App.exe", "app.dll", "app.exe" }, root);

            if (appAssembly == null) throw new Exception("No 'App' assembly found.  Expected either App.exe or App.dll");
        }
        else
        {
            appAssembly = Assembly.GetEntryAssembly();
        }

        // === LOCAL METHOD ===
        static Assembly? FindByPath(string[] namesToCheck, string? root = null)
        {
            root ??= AppDomain.CurrentDomain.BaseDirectory;

            foreach (var name in namesToCheck)
            {
                var path = Path.Combine(root, name);
                if (File.Exists(path))
                {
                    Resolver.Log.Trace($"Found '{path}'", MessageGroup.Core);
                    try
                    {
                        return Assembly.LoadFrom(path);
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Warn($"Unable to load assembly '{name}': {ex.Message}", MessageGroup.Core);
                    }
                }
            }

            return null;
        }

        Resolver.Log.Trace($"Looking for IApp...", MessageGroup.Core);
        var searchType = typeof(IApp);
        var resultList = new List<Type>();
        foreach (var type in appAssembly.GetTypes())
        {
            if (searchType.IsAssignableFrom(type) && !type.IsAbstract)
            {
                resultList.Add(type);
            }
        }

        return resultList.ToArray();
    }

    private static MeadowPlatform DetectPlatform()
    {
        if (RTI.IsOSPlatform(OSPlatform.Windows))
        {
            return MeadowPlatform.Windows;
        }
        else if (RTI.IsOSPlatform(OSPlatform.OSX))
        {
            return MeadowPlatform.OSX;
        }
        else if (RTI.IsOSPlatform(OSPlatform.Linux))
        {
            // could be an embedded linux, could be desktop
            // how do we determine?  ARM v Other?
            return RTI.ProcessArchitecture switch
            {
                Architecture.Arm => MeadowPlatform.EmbeddedLinux,
                Architecture.Arm64 => MeadowPlatform.EmbeddedLinux,
                _ => MeadowPlatform.DesktopLinux
            };
        }
        else if (Directory.Exists("/meadow0"))
        {
            // we're an F7 - but with the current OS we can't tell exctly what type of F7
        }

        return MeadowPlatform.Unknown;
    }

    private static Type FindDeviceTypeParameter_old(Type type)
    {
        if (type.IsGenericType)
        {
            var dt = type.GetGenericArguments().FirstOrDefault(a => typeof(IMeadowDevice).IsAssignableFrom(a));
            if (dt != null) return dt;
        }

        return FindDeviceTypeParameter_old(type.BaseType);
    }

    private static (Type DeviceType, Type? HardwareProviderType) FindDeviceTypeParameter(Type type)
    {
        Type? deviceType = null;
        Type? hardwareProviderType = null;

        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();

            foreach (var arg in genericArgs)
            {
                if (typeof(IMeadowDevice).IsAssignableFrom(arg))
                {
                    deviceType = arg;
                }
                // not a fan of this magic string, but the generic param type is unknown to us, so we can't look for it
                else if (arg.GetInterfaces().Any(i => i.Name.StartsWith("IMeadowAppEmbeddedHardwareProvider")))
                {
                    hardwareProviderType = arg;
                }
            }
        }

        if (deviceType != null)
        {
            return (deviceType, hardwareProviderType);
        }

        return FindDeviceTypeParameter(type.BaseType);
    }

    private static (Type appType, Type deviceType, Type? hardwareProviderType)? FindAppForPlatform(MeadowPlatform platform)
    {
        var allApps = FindAppType(null);

        if (allApps.Length == 0)
        {
            throw new Exception("Cannot find an IApp implementation");
        }

        foreach (var app in allApps)
        {
            Resolver.Log.Info(app.Name, MessageGroup.Core);
        }

        // find an IApp that matches our target platform
        switch (platform)
        {
            case MeadowPlatform.Windows:
                // look for Desktop or Windows
                // (wish C# supported static properties on an interface, they type could then tell what platforms it supports)
                (Type, Type, Type?)? windowsTypeTuple = null;

                foreach (var app in allApps)
                {
                    var devicetype = FindDeviceTypeParameter(app);

                    if (devicetype.DeviceType.FullName == "Meadow.Desktop")
                    {
                        return (app, devicetype.DeviceType, devicetype.HardwareProviderType);
                    }
                    else if (devicetype.DeviceType.FullName == "Meadow.Windows")
                    {
                        // keep a ref in case Desktop isn't found
                        windowsTypeTuple = (app, devicetype.DeviceType, devicetype.HardwareProviderType);
                    }
                }

                if (windowsTypeTuple != null)
                {
                    return windowsTypeTuple;
                }

                throw new Exception("Cannot find an IApp that targets Desktop or Windows");
            case MeadowPlatform.OSX:
                (Type, Type, Type?)? macTypeTuple = null;

                foreach (var app in allApps)
                {
                    var devicetype = FindDeviceTypeParameter(app);

                    if (devicetype.DeviceType.FullName == "Meadow.Desktop")
                    {
                        return (app, devicetype.DeviceType, devicetype.HardwareProviderType);
                    }
                    else if (devicetype.DeviceType.FullName == "Meadow.Mac")
                    {
                        // keep a ref in case Desktop isn't found
                        macTypeTuple = (app, devicetype.DeviceType, devicetype.HardwareProviderType);
                    }
                }

                if (macTypeTuple != null)
                {
                    return macTypeTuple;
                }

                throw new Exception("Cannot find an IApp that targets Desktop or Mac");
            case MeadowPlatform.DesktopLinux:
                (Type, Type, Type?)? linuxTypeTuple = null;

                foreach (var app in allApps)
                {
                    var devicetype = FindDeviceTypeParameter(app);

                    if (devicetype.DeviceType.FullName == "Meadow.Desktop")
                    {
                        return (app, devicetype.DeviceType, devicetype.HardwareProviderType);
                    }
                    else if (devicetype.DeviceType.FullName == "Meadow.Linux")
                    {
                        // keep a ref in case Desktop isn't found
                        linuxTypeTuple = (app, devicetype.DeviceType, devicetype.HardwareProviderType);
                    }
                }

                if (linuxTypeTuple != null)
                {
                    return linuxTypeTuple;
                }

                throw new Exception("Cannot find an IApp that targets Desktop or Linux");
            case MeadowPlatform.EmbeddedLinux:
                // TODO: improve this by finding a way to specifically differentiate Linux ARM
                foreach (var app in allApps)
                {
                    var devicetype = FindDeviceTypeParameter(app);

                    switch (devicetype.DeviceType.FullName)
                    {
                        case "Meadow.BeagleBoneBlack":
                        case "Meadow.OrangePi":
                        case "Meadow.RaspberryPi":
                        case "Meadow.JetsonNano":
                        case "Meadow.JetsonXavierAgx":
                        case "Meadow.SnickerdoodleBlack":
                            return (app, devicetype.DeviceType, devicetype.HardwareProviderType);
                    }
                }

                throw new Exception("Cannot find an IApp that targets a supported ARM Linux");
            case MeadowPlatform.Unknown:
                Interop.HardwareVersion hw = Interop.HardwareVersion.Unknown;
                try
                {
                    hw = Interop.Nuttx.meadow_os_hardware_version();
                }
                catch (Exception)
                {
                    // OS is probably too old to provide this, so just go with the first F7* implementation
                }

                // look for the first F7 app type (no way to determine it before creating the device implementation yet)
                foreach (var app in allApps)
                {
                    var devicetype = FindDeviceTypeParameter(app);

                    switch (hw)
                    {
                        case Interop.HardwareVersion.Unknown:
                            if (allApps.Length > 1)
                            {
                                Resolver.Log.Warn("Multi-targeting of F7 devices is only supported on OS 1.9 and later", MessageGroup.Core);
                            }
                            return (app, devicetype.DeviceType, devicetype.HardwareProviderType);
                        case Interop.HardwareVersion.F7FeatherV1:
                            if (devicetype.DeviceType.FullName == "Meadow.Devices.F7FeatherV1")
                            {
                                return (app, devicetype.DeviceType, devicetype.HardwareProviderType);
                            }
                            break;
                        case Interop.HardwareVersion.F7FeatherV2:
                            if (devicetype.DeviceType.FullName == "Meadow.Devices.F7FeatherV2")
                            {
                                return (app, devicetype.DeviceType, devicetype.HardwareProviderType);
                            }
                            break;
                        case Interop.HardwareVersion.F7CoreComputeV2:
                            if (devicetype.DeviceType.FullName == "Meadow.Devices.F7CoreComputeV2")
                            {
                                return (app, devicetype.DeviceType, devicetype.HardwareProviderType);
                            }
                            break;
                    }
                }

                throw new Exception($"Cannot find an IApp that targets a {(hw == Interop.HardwareVersion.Unknown ? "Meadow F7" : hw)} device");
            default:
                throw new Exception($"Cannot find an IApp that targets platform '{platform}'");
        }
    }

    private static bool Initialize(string[]? args, IApp? app)
    {
        try
        {
            var now = Environment.TickCount;

            now = Environment.TickCount;
            Resolver.Services.Add(new Logger());
            Resolver.Log.AddProvider(new ConsoleLogProvider());

            Console.WriteLine($"Initializing OS... ");
        }
        catch (Exception ex)
        {
            // must use Console because the logger failed
            Console.WriteLine($"Failed to create Logger: {ex.Message}");
            return false;
        }

        //capture unhandled exceptions
        AppDomain.CurrentDomain.UnhandledException += StaticOnUnhandledException;

        // TODO: allow user injection of this
        Resolver.Services.Add<IJsonSerializer>(new MicroJsonSerializer());

        var settings = LoadSettings();
        var platform = DetectPlatform();
        var appTypes = FindAppForPlatform(platform);

        Type appType = appTypes!.Value.appType;
        var deviceType = appTypes!.Value.deviceType;
        var hardwareProviderType = appTypes!.Value.hardwareProviderType;

        try
        {
            // Initialize strongly-typed hardware access - setup the interface module specified in the App signature
            var b4 = Environment.TickCount;
            var et = Environment.TickCount - b4;

            // is there an override arg for "root"?
            string? root = null;
            if (args != null)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--root" && args.Length > i)
                    {
                        root = args[i + 1];
                        _startedDirectly = true;
                        break;
                    }
                }
            }

            try
            {
                if (Activator.CreateInstance(deviceType) is not IMeadowDevice device)
                {
                    throw new Exception($"Failed to create instance of '{deviceType.Name}'");
                }
                et = Environment.TickCount - b4;
                Resolver.Log.Trace($"Creating '{deviceType.Name}' instance took {et}ms", MessageGroup.Core);

                CurrentDevice = device;
                Resolver.Services.Add<IMeadowDevice>(CurrentDevice);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    Resolver.Log.Error($"Creating IMeadowDevice instance failure : {ex.Message}{Environment.NewLine}{ex.InnerException}", MessageGroup.Core);
                }
                else
                {
                    Resolver.Log.Error($"Creating IMeadowDevice instance failure : {ex.Message}", MessageGroup.Core);
                }
                return false;
            }

            try
            {
                var reliabilityService = CurrentDevice.ReliabilityService;
                if (reliabilityService != null)
                {
                    Resolver.Services.Add<IReliabilityService>(reliabilityService);
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Warn($"Failed to create IReliabilityService: {ex.Message}", MessageGroup.Core);
            }

            Resolver.Log.Trace($"Device Initialize starting...", MessageGroup.Core);
            CurrentDevice.Initialize(platform);
            Resolver.Log.Trace($"PlatformOS Initialize starting...", MessageGroup.Core);
            CurrentDevice.PlatformOS.Initialize(CurrentDevice.Capabilities, args); // initialize the devices' platform OS

            // initialize file system folders and such
            // TODO: move this to platformOS
            Resolver.Log.Trace($"File system Initialize starting...", MessageGroup.Core);
            InitializeFileSystem();

            IMeadowAppEmbeddedHardware? appHardwareInstance = null;

            try
            {
                if (hardwareProviderType != null)
                {
                    // because reflection doesn't seem to traverse the type constraints, manually do this trash
                    var createMethod = hardwareProviderType
                        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                        .Where(m => m.Name == nameof(IMeadowAppEmbeddedHardwareProvider<IMeadowAppEmbeddedHardware>.Create)
                            && m.GetParameters().Length == 1
                            && m.GetParameters().FirstOrDefault(
                                p => typeof(IMeadowDevice).IsAssignableFrom(p.ParameterType))
                            != null)
                        .FirstOrDefault();

                    if (createMethod != null)
                    {
                        Resolver.Log.Trace($"Creating hardware provider '{hardwareProviderType.Name}'", MessageGroup.Core);
                        // allow using non-public constructors
                        var provider = Activator.CreateInstance(
                            hardwareProviderType, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                            null, null, null);

                        Resolver.Log.Trace($"Using hardware provider to create hardware...", MessageGroup.Core);
                        var hardware = createMethod.Invoke(provider, new object[] { CurrentDevice });

                        Resolver.Log.Trace($"Hardware is a {hardware.GetType().Name}", MessageGroup.Core);
                        appHardwareInstance = hardware as IMeadowAppEmbeddedHardware;
                    }
                    else
                    {
                        Resolver.Log.Warn($"No appropriate Create method found in {hardwareProviderType.Name}", MessageGroup.Core);
                    }

                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    Resolver.Log.Error($"Creating hardware provider instance failure : {ex.Message}{Environment.NewLine}{ex.InnerException}", MessageGroup.Core);
                }
                else
                {
                    Resolver.Log.Error($"Creating hardware provider instance failure : {ex.Message}", MessageGroup.Core);
                }
                return false;
            }

            if (app == null)
            {
                // Create the app object, bound immediately to the <IMeadowDevice>
                b4 = Environment.TickCount;
                Resolver.Log.Trace($"Creating instance of {appType.Name}...", MessageGroup.Core);

                if (Activator.CreateInstance(appType, nonPublic: true) is not IApp capp)
                {
                    throw new Exception($"Failed to create instance of '{appType.Name}'");
                }
                app = capp;
                et = Environment.TickCount - b4;
                Resolver.Log.Trace($"Creating '{appType.Name}' instance took {et}ms", MessageGroup.Core);
            }

            // feels off, but not seeing a super clean way without the generics, etc.
            if (appType.GetProperty(nameof(IApp.CancellationToken)) is PropertyInfo pi)
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(app, AppAbort.Token);
                }
            }
            if (appType.GetProperty(nameof(IApp.Settings)) is PropertyInfo spi)
            {
                if (spi.CanWrite)
                {
                    spi.SetValue(app, settings);
                }
            }

            Resolver.Log.Trace($"Checking for Hardware property", MessageGroup.Core);
            if (appType.GetProperty("Hardware",
                BindingFlags.Static
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.FlattenHierarchy) is PropertyInfo hpi)
            {
                if (hpi.CanWrite)
                {
                    Resolver.Log.Trace($"Setting Hardware property", MessageGroup.Core);
                    hpi.SetValue(app, appHardwareInstance);
                }
                else
                {
                    Resolver.Log.Trace($"!! Hardware property not writable", MessageGroup.Core);
                }
            }
            else
            {
                Resolver.Log.Trace($"No Hardware property found in App.", MessageGroup.Core);
            }
            App = app;

            var cloudConnectionService = new MeadowCloudConnectionService(MeadowCloudSettings);
            Resolver.Services.Add<IMeadowCloudService>(cloudConnectionService);
            var commandService = new MeadowCloudCommandService(cloudConnectionService);
            Resolver.Services.Add<ICommandService>(commandService);

            var updateService = new MeadowCloudUpdateService(
                CurrentDevice.PlatformOS.FileSystem.FileSystemRoot,
                cloudConnectionService);
            Resolver.Services.Add<IUpdateService>(updateService);

            if (MeadowCloudSettings.EnableUpdates)
            {
                updateService.Start();
            }

            var healthReporter = new HealthReporter();
            Resolver.Services.Add<IHealthReporter>(healthReporter);
            if (MeadowCloudSettings.EnableHealthMetrics)
            {
                if (MeadowCloudSettings.HealthMetricsIntervalMinutes > 0)
                {
                    healthReporter.Start(MeadowCloudSettings.HealthMetricsIntervalMinutes).RethrowUnhandledExceptions();
                }
                else
                {
                    Resolver.Log.Warn($"Health metrics interval of {MeadowCloudSettings.HealthMetricsIntervalMinutes} is invalid.", MessageGroup.Core);
                }
            }

            if (MeadowCloudSettings.Enabled
                || MeadowCloudSettings.EnableUpdates
                || MeadowCloudSettings.EnableHealthMetrics)
            {
                Resolver.Log.Info($"Meadow cloud base features: {(MeadowCloudSettings.Enabled ? "enabled" : "disabled")}", MessageGroup.Core);
                Resolver.Log.Info($"Meadow cloud updates: {(MeadowCloudSettings.EnableUpdates ? "enabled" : "disabled")}", MessageGroup.Core);
                Resolver.Log.Info($"Meadow cloud health metrics: {(MeadowCloudSettings.EnableHealthMetrics ? "enabled" : "disabled")}", MessageGroup.Core);
                cloudConnectionService.Start();
            }
            else
            {
                Resolver.Log.Info("All cloud features are disabled.", MessageGroup.Core);
            }

            return true;
        }
        catch (Exception e)
        {
            Resolver.Log.Error(e.ToString(), MessageGroup.Core);
            return false;
        }
    }

    private static ManualResetEvent _forceTerminate = new ManualResetEvent(false);

    /// <summary>
    /// Cancel the meadow OS application Run call and allow the process to exit
    /// </summary>
    public static void TerminateRun()
    {
        AppAbort.Cancel();
        _forceTerminate.Set();
    }

    private static void Shutdown()
    {
        AppAbort.Cancel(true);

        // stop the update service
        if (Resolver.Services.Get<IMeadowCloudService>() is { } cloudService)
        {
            cloudService.Stop();
        }

        // schedule a device restart if possible and if the user hasn't disabled it
        ScheduleRestart();

        // Do a best-attempt at freeing memory and resources
        GC.Collect(GC.MaxGeneration);
        Resolver.Log.Debug("Shutdown", MessageGroup.Core);

        // just put the Main thread to sleep (don't exit) because we want the Update service to still be able to work
        _forceTerminate.WaitOne();
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
                Resolver.Log.Error($"Failed: {ex.Message}", MessageGroup.Core);
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
                Resolver.Log.Info($"CRASH: Meadow will restart in {restart} seconds.", MessageGroup.Core);
                Thread.Sleep(restart * 1000);

                CurrentDevice.PlatformOS.Reset();

                AppAbort.Cancel();
            }
            else
            {
                Resolver.Log.Info($"Initialization failure prevents automatic restart.", MessageGroup.Core);
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

internal static partial class Interop
{
    public enum HardwareVersion
    {
        //#define MEADOW_F7_HW_VERSION_NUMB_F7V1 (1)
        //#define MEADOW_F7_HW_VERSION_NUMB_F7V2 (2)
        //#define MEADOW_F7_HW_VERSION_NUMB_CCMV2 (3)
        Unknown = 0,
        F7FeatherV1 = 1,
        F7FeatherV2 = 2,
        F7CoreComputeV2 = 3,
    }

    public static partial class Nuttx
    {
        [DllImport("nuttx")]
        public static extern HardwareVersion meadow_os_hardware_version();
    }
}