using Meadow.Hardware;
using Meadow.Update;
using MQTTnet;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

internal class MeadowCloudUpdateService : IUpdateService
{
    public event EventHandler<UpdateState>? StateChanged;
    /// <inheritdoc/>
    public event UpdateEventHandler? UpdateAvailable;
    /// <inheritdoc/>
    public event UpdateEventHandler? UpdateProgress;
    /// <inheritdoc/>
    public event UpdateEventHandler? UpdateRetrieved;
    /// <inheritdoc/>
    public event UpdateEventHandler? UpdateSuccess;
    /// <inheritdoc/>
    public event UpdateEventHandler? UpdateFailure;

    /// <summary>
    /// Specifies the maximum number of download attempts before giving up.
    /// </summary>
    public const int MaxDownloadRetries = 10;
    /// <summary>
    /// Retry period the service will use to attempt network reconnection
    /// </summary>
    private const int NetworkRetryTimeoutSeconds = 15;
    /// <summary>
    /// Period the service will wait between download attempts in 
    /// case of failure.
    /// </summary>
    public const int RetryDelayMilliseconds = 1000;

    private const string DefaultUpdateStoreDirectoryName = "update-store";
    private const string DefaultUpdateDirectoryName = "update";

    private bool _useAuthentication = true; // TODO: add config for this

    private readonly MeadowCloudConnectionService _connectionService;
    private bool _stopService = false;
    private UpdateState _state;
    private bool _downloadInProgress;

    private UpdateStore Store { get; }
    private string UpdateDirectory { get; }
    private string UpdateStoreDirectory { get; }

    public MeadowCloudUpdateService(string fsRoot, MeadowCloudConnectionService connectionService)
    {
        UpdateStoreDirectory = Path.Combine(fsRoot, DefaultUpdateStoreDirectoryName);
        UpdateDirectory = Path.Combine(fsRoot, DefaultUpdateDirectoryName);

        Store = new UpdateStore(UpdateStoreDirectory);

        _connectionService = connectionService;
        _connectionService.MqttMessageReceived += OnMqttMessageReceived;
        _connectionService.AddSubscription("{OID}/ota/{ID}");
    }

    private void OnMqttMessageReceived(object sender, MqttApplicationMessage e)
    {
        if (e.Topic.EndsWith($"/ota/{Resolver.Device.Information.UniqueID}", StringComparison.OrdinalIgnoreCase))
        {
            Resolver.Log.Info("Meadow update message received", "cloud");

            var json = Encoding.UTF8.GetString(e.Payload);
            var info = JsonSerializer.Deserialize<UpdateMessage>(json);

            if (info == null)
            {
                Resolver.Log.Warn($"Unable to deserialize Updater Message");
            }
            else
            {
                Resolver.Log.Trace($"Updater Message Received: {info.ID}");

                // do we already know about this update?
                if (Store.TryGetMessage(info.ID, out UpdateMessage? msg))
                {
                    if (!(msg?.Retrieved ?? false))
                    {
                        Resolver.Log.Trace($"Update {info.ID} is known but not retrieved");

                        UpdateAvailable?.Invoke(this, info);
                    }
                    else
                    {
                        Resolver.Log.Trace($"Update {info.ID} has already been retrieved");
                    }
                }
                else
                {
                    Store.Add(info);
                    UpdateAvailable?.Invoke(this, info);
                }

            }
        }
    }

    public UpdateState State
    {
        get => _state;
        private set
        {
            if (value == State) return;

            _state = value;
            StateChanged?.Invoke(this, State);
            Resolver.Log.Trace($"Updater State -> {State}");
        }
    }

    /// <summary>
    /// Starts the service if it is not already running
    /// </summary>
    public void Start()
    {
        if (State == UpdateState.Dead)
        {
            // using Thread instead of a Task because of a bug (in the runtime or BCL)
            new Thread(() => UpdateStateMachine())
                .Start();
        }
    }

    /// <inheritdoc/>
    public void Stop()
    {
        _stopService = true;
    }

    private void UpdateStateMachine()
    {
        _stopService = false;

        // we need to wait for the network stack to come up on the F7 platforms
        switch (Resolver.Device.Information.Platform)
        {
            case MeadowPlatform.F7FeatherV1:
            case MeadowPlatform.F7FeatherV2:
            case MeadowPlatform.F7CoreComputeV2:
                Thread.Sleep(TimeSpan.FromSeconds(NetworkRetryTimeoutSeconds));
                break;
        }

        State = UpdateState.Disconnected;

        var nic = Resolver.Device?.NetworkAdapters?.Primary<INetworkAdapter>();

        // update state machine
        while (!_stopService)
        {
            switch (State)
            {

                case UpdateState.Connected:
                    if (_downloadInProgress)
                    {
                        Resolver.Log.Debug($"Resuming download after reconnection...");
                        State = UpdateState.DownloadingFile;
                    }
                    break;
                case UpdateState.Idle:
                case UpdateState.DownloadingFile:
                case UpdateState.UpdateInProgress:
                default:
                    Thread.Sleep(1000);
                    break;
            }
        }

        State = UpdateState.Dead;
    }

    /// <inheritdoc/>
    public void ClearUpdates()
    {
        switch (State)
        {
            case UpdateState.Dead:
            case UpdateState.Idle:
                break;
            default:
                throw new Exception("Cannot clear until Idle");
        }

        Store.Clear();
    }

    /// <inheritdoc/>
    public void RetrieveUpdate(UpdateInfo updateInfo)
    {
        State = UpdateState.DownloadingFile;

        UpdateMessage? message;


        if (!Store.TryGetMessage(updateInfo.ID, out message))
        {
            throw new ArgumentException($"Cannot find update with ID {updateInfo.ID}");
        }

        if (message != null)
        {
            Task.Run(() => DownloadProc(message));
        }
    }

    private async Task DownloadProc(UpdateMessage message)
    {
        Resolver.Log.Trace($"Device OS Version: {Resolver.Device.PlatformOS.OSVersion}, Update OS Version: {message.OsVersion}");

        var destination = message.MpakDownloadUrl;

        if (!string.IsNullOrEmpty(message.OsVersion)
            && Resolver.Device.PlatformOS.OSVersion != message.OsVersion)
        {
            destination = message.MpakWithOsDownloadUrl;
        }

        if (!destination.StartsWith("http"))
        {
            if (_useAuthentication)
            {
                destination = $"https://{destination}";
            }
            else
            {
                destination = $"http://{destination}";
            }
        }

        Resolver.Log.Trace($"Attempting to retrieve {destination}");
        var sw = Stopwatch.StartNew();

        long totalBytesDownloaded = 0;
        _downloadInProgress = true;

        for (int retryCount = 0; retryCount < MaxDownloadRetries && _downloadInProgress; retryCount++)
        {
            try
            {
                // Note: this is infrequently called, so we don't want to follow the advice of "use one instance for all calls"
                using (var httpClient = new HttpClient())
                {
                    if (_useAuthentication)
                    {
                        httpClient.DefaultRequestHeaders.Authorization = _connectionService.CreateAuthenticationHeaderValue();
                    }

                    // Configure the HTTP range header to indicate resumption of partial download, starting from 
                    // the 'totalBytesDownloaded' byte position and extending to the end of the content.
                    httpClient.DefaultRequestHeaders.Range = new System.Net.Http.Headers.RangeHeaderValue(totalBytesDownloaded, null);

                    using (var stream = await httpClient.GetStreamAsync(destination))
                    {
                        using (var fileStream = Store.GetUpdateFileStream(message.ID))
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead;

                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalBytesDownloaded += bytesRead;

                                message.DownloadProgress = totalBytesDownloaded;

                                UpdateProgress?.Invoke(this, message);
                                Resolver.Log.Trace($"Download progress: {totalBytesDownloaded:N0} bytes downloaded");
                            }
                        }
                    }
                }

                sw.Stop();

                var path = Store.GetUpdateArchivePath(message.ID);
                var fi = new FileInfo(path);

                if (sw.Elapsed.Seconds > 0)
                {
                    Resolver.Log.Info($"Retrieved {fi.Length} bytes in {sw.Elapsed.TotalSeconds:0} sec ({fi.Length / sw.Elapsed.Seconds:0} bps)");
                }
                else
                {
                    // don't divide by 0
                    Resolver.Log.Info($"Retrieved {fi.Length} bytes in {sw.Elapsed.TotalSeconds:0} sec");
                }

                _downloadInProgress = false;

                var hash = Store.GetFileHash(fi);

                if (!string.IsNullOrWhiteSpace(message.DownloadHash))
                {
                    if (hash != message.DownloadHash)
                    {
                        Resolver.Log.Warn("Downloaded Hash does not match expected Hash");
                        // TODO: what do we do? Retry? Ignore?
                    }
                    else
                    {
                        Resolver.Log.Trace("Update package hash matched");
                    }
                }
                else
                {
                    Resolver.Log.Warn("Downloaded Updated was not Hashed by server!");
                    // TODO: what do we do?
                }

                UpdateRetrieved?.Invoke(this, message);
                Store.SetRetrieved(message);

                State = UpdateState.Idle;
            }
            catch (Exception ex)
            {
                sw.Stop();

                // TODO: raise some event?
                Resolver.Log.Error($"Failed to download Update after {sw.Elapsed.TotalSeconds:0} seconds: {ex.Message}");

                Resolver.Log.Info($"Retrying attempt {retryCount + 1} of {MaxDownloadRetries} in {RetryDelayMilliseconds} milliseconds...");
                await Task.Delay(RetryDelayMilliseconds);
            }
        }

        State = UpdateState.Idle;
        _downloadInProgress = false;
    }

    /// <inheritdoc/>
    public void ApplyUpdate(UpdateInfo updateInfo)
    {
        State = UpdateState.UpdateInProgress;

        var sourcePath = Store.GetUpdateArchivePath(updateInfo.ID);

        Resolver.Log.Debug($"Applying update from '{sourcePath}'");

        if (sourcePath == null)
        {
            UpdateFailure?.Invoke(this, updateInfo);
            throw new ArgumentException($"Cannot find update with ID {updateInfo.ID}");
        }

        // ensure the update directory is currently empty
        var di = new DirectoryInfo(UpdateDirectory);
        if (!di.Exists)
        {
            di.Create();
        }
        else
        {
            DeleteDirectoryContents(di);
        }

        try
        {
            // extract zip
            Resolver.Log.Debug($"Extracting update to '{UpdateDirectory}'...");
            var sw = Stopwatch.StartNew();
            ZipFile.ExtractToDirectory(sourcePath, UpdateDirectory);
            sw.Stop();
            Resolver.Log.Debug($"Extracting took {sw.Elapsed.TotalSeconds} seconds.");

            Resolver.Log.Debug($"Contents of Update");
            Resolver.Log.Debug($"------------------");
            DisplayTree(new DirectoryInfo(UpdateDirectory));
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Failed to extract update package: {ex.Message}");
            UpdateFailure?.Invoke(this, updateInfo);
            throw ex;
        }

        // do we actually contain an update?
        var updateValid = CurrentUpdateContainsAppUpdate() || CurrentUpdateContainsOSUpdate();

        // TODO: should we verify paths, etc?

        if (!updateValid)
        {
            Resolver.Log.Warn($"Update {updateInfo.ID} contains no valid Update data");
            UpdateFailure?.Invoke(this, updateInfo);
            return;
        }

        // shut down the app (or timeout)
        Resolver.Log.Debug($"Stopping application to allow update");
        var shutdownTimeoutTask = Task.Delay(TimeSpan.FromSeconds(5));
        var shutDownTask = Resolver.App?.OnShutdown();

        // cancel the app run cancellation token
        MeadowOS.AppAbort.Cancel();

        // wait for the app to return, or the timeout
        Task.WaitAny(shutDownTask, shutdownTimeoutTask);

        // restart the device
        Resolver.Log.Debug($"Requesting a device reset to apply the Update");
        Resolver.Device.PlatformOS.Reset();

        // the OS will handle updates on the next boot

        // TODO: these will never happen due to the above reset. need to be moved to "post update boot"
        Store.SetApplied(updateInfo);
        UpdateSuccess?.Invoke(this, updateInfo);

        State = UpdateState.Idle;
    }

    private void DeleteDirectoryContents(DirectoryInfo di, bool deleteDirectory = false)
    {
        foreach (var f in di.EnumerateFiles())
        {
            f.Delete();
        }

        foreach (var d in di.EnumerateDirectories())
        {
            DeleteDirectoryContents(d, true);
            if (deleteDirectory)
            {
                d.Delete();
            }
        }
    }

    private bool CurrentUpdateContainsAppUpdate()
    {
        var di = new DirectoryInfo(UpdateDirectory);
        var appDir = di.GetDirectories("app").FirstOrDefault();
        if (appDir == null)
        {
            return false;
        }

        // make sure that some files exist - no files would brick us.  A bad set still can
        return appDir.GetFiles().Count() > 0;
    }

    private bool CurrentUpdateContainsOSUpdate()
    {
        var di = new DirectoryInfo(UpdateDirectory);
        var osDir = di.GetDirectories("os").FirstOrDefault();
        if (osDir == null)
        {
            return false;
        }

        // make sure that some files exist
        return osDir.GetFiles().Count() > 0;
    }

    private void DisplayTree(DirectoryInfo di)
    {
        Resolver.Log.Debug($"+ {di.Name}");

        foreach (var f in di.GetFiles())
        {
            Resolver.Log.Debug($"  - {f.Name}");
        }

        foreach (var d in di.GetDirectories())
        {
            DisplayTree(d);
        }
    }
}
