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
    /// <summary>
    /// Initializes a new instance of the <see cref="MeadowDaemonUpdateService"/> class.
    /// </summary>
    /// <param name="fileSystemRoot">The root directory of the file system where updates will be managed.</param>
    /// <param name="meadowCloudService">An instance of <see cref="IMeadowCloudService"/> used to interact with the Meadow cloud services.</param>
    public MeadowDaemonUpdateService(string fileSystemRoot, IMeadowCloudService meadowCloudService)
        : base(GetDaemonMeadowRootFolder(fileSystemRoot), meadowCloudService)
    {
    }

    private static string GetDaemonMeadowRootFolder(string defaultValue)
    {
        // if the Meadow.Daemon is installed, we want to use its configured meadow root

        Resolver.Log.Info("Querying Meadow Daemon for configured Meadow root folder...");

        // TODO: query this from the daemon
        return "/opt/meadow";

        //        return defaultValue;
    }

    /// <inheritdoc/>
    protected override string GetUpdateTempFolder()
    {
        // TODO: query this from the daemon
        return "/tmp/meadow";
    }

    /// <inheritdoc/>
    protected override string UpdateStoreDirectoryName
    {
        get
        {
            // TODO: query this from the daemon
            return "/tmp/meadow/update-store";
        }
    }

    /// <inheritdoc/>
    protected override string UpdateBinaryDirectoryName
    {
        get
        {
            // TODO: query this from the daemon
            return "/tmp/meadow/update";
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
