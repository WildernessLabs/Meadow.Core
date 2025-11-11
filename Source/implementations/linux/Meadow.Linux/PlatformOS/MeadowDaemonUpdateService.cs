using Meadow.Cloud;

namespace Meadow;

/// <summary>
/// Provides functionality to manage and apply updates for the Meadow Daemon,  including retrieving updates from the
/// Meadow cloud service and tracking update progress.
/// </summary>
/// <remarks>This service interacts with Meadow cloud services to check for, retrieve, and apply updates. It
/// provides events to notify subscribers about the update process, including state changes, update availability,
/// progress, and success or failure outcomes.</remarks>
public class MeadowDaemonUpdateService : MeadowCloudUpdateService
{
    private static MeadowDaemon.DaemonConfig? _daemonConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeadowDaemonUpdateService"/> class.
    /// </summary>
    /// <param name="fileSystemRoot">The root directory of the file system where updates will be managed.</param>
    /// <param name="meadowCloudService">An instance of <see cref="IMeadowCloudService"/> used to interact with the Meadow cloud services.</param>
    public MeadowDaemonUpdateService(string fileSystemRoot, IMeadowCloudService meadowCloudService)
        : base(GetDaemonMeadowRootFolder(fileSystemRoot), meadowCloudService)
    {
    }

    private static MeadowDaemon.DaemonConfig? GetConfig()
    {
        if (_daemonConfig == null)
        {
            Resolver.Log.Info("Loading Meadow Daemon configuration...");
            _daemonConfig = MeadowDaemon.GetConfiguration().GetAwaiter().GetResult();
        }
        return _daemonConfig;
    }

    private static string GetDaemonMeadowRootFolder(string defaultValue)
    {
        var config = GetConfig();
        return config?.MeadowRoot ?? "/opt/meadow";
    }

    /// <inheritdoc/>
    protected override string GetUpdateTempFolder()
    {
        var config = GetConfig();
        return config?.MeadowTemp ?? "/tmp/meadow";
    }

    /// <inheritdoc/>
    protected override string UpdateStoreDirectoryName
    {
        get
        {
            var config = GetConfig();
            return config?.UpdateStorePath ?? "/meadow/update-store";
        }
    }

    /// <inheritdoc/>
    protected override string UpdateBinaryDirectoryName
    {
        get
        {
            var config = GetConfig();
            return config?.TempExtractPath ?? "/tmp/meadow/update";
        }
    }

    /// <inheritdoc/>
    protected override void AfterUpdatePackageExpanded(string expandedPackagePath)
    {
        // now tell the daemon we need to restart, where the update is, and what our PID is
        Resolver.Log.Info("Notifying Meadow Daemon to apply update...");

        var updateTask = MeadowDaemon.ApplyUpdate();

        updateTask.Wait();

        Resolver.Log.Info("Shutting down the app");

        MeadowOS.TerminateRun();
    }
}
