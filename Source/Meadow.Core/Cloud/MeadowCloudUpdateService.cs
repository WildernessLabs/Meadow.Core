using Meadow.Update;
using MQTTnet;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Meadow;

internal class MeadowCloudUpdateService : IUpdateService
{
    private const string PackageDirectoryOs = "os";
    private const string PackageDirectoryApp = "app";
    private const string PackageDirectoryFirmware = "firmware";

    public event EventHandler<UpdateState>? StateChanged;
    /// <inheritdoc/>
    public event UpdateEventHandler? UpdateAvailable;
    /// <inheritdoc/>
    public event UpdateEventHandler? RetrieveProgress;
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

    private readonly MeadowCloudConnectionService _connectionService;
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
        _connectionService.ConnectionStateChanged += OnConnectionStateChanged;
    }

    private void OnConnectionStateChanged(object sender, CloudConnectionState e)
    {
        switch (e)
        {
            case CloudConnectionState.Unknown:
            case CloudConnectionState.Disconnected:
            case CloudConnectionState.Authenticating:
            case CloudConnectionState.Connecting:
            case CloudConnectionState.Subscribing:
                if (State != UpdateState.UpdateInProgress)
                {
                    State = UpdateState.Disconnected;
                }
                break;
            case CloudConnectionState.Connected:
                if (_downloadInProgress)
                {
                    State = UpdateState.DownloadingFile;
                }
                else if (State != UpdateState.UpdateInProgress)
                {
                    State = UpdateState.Connected;
                }
                break;
        }
    }

    private void OnMqttMessageReceived(object sender, MqttApplicationMessage e)
    {
        if (e.Topic.EndsWith($"/ota/{Resolver.Device.Information.UniqueID}", StringComparison.OrdinalIgnoreCase))
        {
            if (!_isEnabled)
            {
                Resolver.Log.Warn("Meadow update message received, but updates are currently disabled", "cloud");
                return;
            }

            Resolver.Log.Info("Meadow update message received", "cloud");

            var info = Resolver.JsonSerializer.Deserialize<UpdateMessage>(e.Payload);

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
        }
    }

    private bool _isEnabled = false;

    /// <summary>
    /// Starts the service if it is not already running
    /// </summary>
    public void Start()
    {
        _isEnabled = true;
    }

    /// <inheritdoc/>
    public void Stop()
    {
        _isEnabled = false;
    }

    /// <inheritdoc/>
    public void ClearUpdates()
    {
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
            Task.Run(() => DownloadProc(message))
                .RethrowUnhandledExceptions();
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
            if (_connectionService.Settings.UseAuthentication)
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
                    if (_connectionService.Settings.UseAuthentication)
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
                            byte[] buffer = new byte[1024 * 64]; // TODO: make this configurable/platform dependent
                            int bytesRead;

                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalBytesDownloaded += bytesRead;

                                message.DownloadProgress = totalBytesDownloaded;

                                RetrieveProgress?.Invoke(this, message);
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

                var hash = await Store.GetFileHash(fi);

                if (!string.IsNullOrWhiteSpace(message.Crc))
                {
                    if (hash != message.Crc)
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
            }
            catch (Exception ex)
            {
                sw.Stop();

                // TODO: raise some event?
                Resolver.Log.Error($"[ {ex.GetType().Name} ] Failed to download Update after {sw.Elapsed.TotalSeconds:0} seconds: {ex.Message} {ex.StackTrace}");

                Resolver.Log.Info($"Retrying attempt {retryCount + 1} of {MaxDownloadRetries} in {RetryDelayMilliseconds} milliseconds...");
                await Task.Delay(RetryDelayMilliseconds);
            }
        }

        State = _connectionService.ConnectionState == CloudConnectionState.Connected ? UpdateState.Connected : UpdateState.Disconnected;

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

            // TODO: should we delete the update or quarantine it?

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

        State = _connectionService.ConnectionState == CloudConnectionState.Connected ? UpdateState.Connected : UpdateState.Disconnected;
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
        var appDir = di.GetDirectories(PackageDirectoryApp).FirstOrDefault();
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
        var osDir = di.GetDirectories(PackageDirectoryOs).FirstOrDefault();
        var firmwareDir = di.GetDirectories(PackageDirectoryFirmware).FirstOrDefault();
        if (osDir == null && firmwareDir == null)
        {
            return false;
        }

        // make sure that some files exist
        return osDir?.GetFiles().Count() > 0 || firmwareDir?.GetFiles().Count() > 0;
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
