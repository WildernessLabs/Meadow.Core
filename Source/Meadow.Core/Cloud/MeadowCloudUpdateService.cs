using Meadow.Cloud;
using Meadow.Update;
using MQTTnet;
using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

using static Resolver;

/// <summary>
/// Provides functionality to manage and apply updates from the Meadow Cloud service.
/// </summary>
/// <remarks>This service handles the retrieval, validation, and application of updates for devices connected to
/// the Meadow Cloud. It listens for update notifications, downloads update packages, validates their integrity, and
/// applies them to the device. The service also manages its internal state and raises events to notify subscribers
/// about update progress and results.</remarks>
public class MeadowCloudUpdateService : IUpdateService
{
    private const string LogMessageGroup = "update service";

    /// <inheritdoc/>
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

    private const int RetryDelayMilliseconds = 1000;
    private const int UpdateServicePulseMilliseconds = 10000;

    private const int DownloadBufferSize = 1024 * 384;

    private readonly MeadowCloudConnectionService _connectionService;
    private UpdateState _state = UpdateState.Disconnected;

    private UpdateStore Store { get; }
    private string UpdateDirectory { get; }
    private string UpdateStoreDirectory { get; }

    private CancellationTokenSource _service_cancellation = new();
    private CancellationTokenSource _update_cancellation = new();
    internal Task UpdateServiceTask = Task.CompletedTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeadowCloudUpdateService"/> class,  setting up directories for
    /// update storage and subscribing to OTA-related messages.
    /// </summary>
    /// <remarks>This constructor configures the update service by initializing the update store and
    /// directories  required for managing updates. It also subscribes to relevant MQTT topics for receiving OTA
    /// messages.</remarks>
    /// <param name="fsRoot">The root directory path where update-related files will be stored.</param>
    /// <param name="meadowCloudService">An instance of <see cref="IMeadowCloudService"/> used to manage cloud connectivity and message subscriptions.</param>
    public MeadowCloudUpdateService(string fsRoot, IMeadowCloudService meadowCloudService)
    {
        var connectionService = meadowCloudService as MeadowCloudConnectionService;

        if (connectionService == null)
        {
            throw new ArgumentException("meadowCloudService must be of type MeadowCloudConnectionService", nameof(meadowCloudService));
        }

        UpdateStoreDirectory = Path.Combine(fsRoot, UpdateStoreDirectoryName);
        UpdateDirectory = Path.Combine(fsRoot, UpdateBinaryDirectoryName);

        Resolver.Log.Info($"Update Store Directory: {UpdateStoreDirectory}", LogMessageGroup);
        Resolver.Log.Info($"Update Directory: {UpdateDirectory}", LogMessageGroup);

        // TODO: if the store and binaries folder overlap, bad things will ensue.
        // need to guard against that.

        Store = new UpdateStore(UpdateStoreDirectory);

        _connectionService = connectionService;
        _connectionService.MqttMessageReceived += OnMqttMessageReceived;
        _connectionService.AddSubscription("{OID}/ota/{ID}");
    }

    /// <summary>
    /// Gets the name of the directory used to store update-related files.
    /// </summary>
    /// <remarks>This property can be overridden in a derived class to customize the directory name.</remarks>
    protected virtual string UpdateStoreDirectoryName => "update-store";
    /// <summary>
    /// Gets the name of the directory where update binaries are stored after extraction from an MPAK download
    /// </summary>
    protected virtual string UpdateBinaryDirectoryName => "update";

    private async Task UpdateService()
    {
        while (!Resolver.App.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(UpdateServicePulseMilliseconds);
            Log.Trace($"store state: {Store.State} cloud connection state: {_connectionService.ConnectionState}", LogMessageGroup);

            _service_cancellation.Token.ThrowIfCancellationRequested();
            if (_connectionService.ConnectionState != CloudConnectionState.Connected && Store.State != UpdateStore.States.Mpak)
            {
                continue;
            }

            using var service_or_update_cancellation = CancellationTokenSource.CreateLinkedTokenSource(_service_cancellation.Token,
                                                                                                     _update_cancellation.Token);
            try
            {
                switch (Store.State)
                {

                    case UpdateStore.States.Empty:
                        //nothing - wait for OnMqttMessageReceived
                        break;
                    case UpdateStore.States.Manifest:
                        Log.Debug("update available", LogMessageGroup);
                        UpdateAvailable?.Invoke(this, Store.Manifest!, _update_cancellation);
                        _update_cancellation.Token.ThrowIfCancellationRequested();
                        await DownloadProc(Store.Manifest!, service_or_update_cancellation);
                        Store.CompleteMpak();

                        break;
                    case UpdateStore.States.Mpak:
                        Log.Debug("update retrieved", LogMessageGroup);
                        UpdateRetrieved?.Invoke(this, Store.Manifest!, _update_cancellation);
                        _update_cancellation.Token.ThrowIfCancellationRequested();
                        var expandedPath = ApplyUpdate(Store.Manifest!);
                        Store.State = UpdateStore.States.Empty;
                        UpdateSuccess?.Invoke(this, Store.Manifest!, _update_cancellation); // not called - device has reset by now
                        AfterUpdatePackageExpanded(expandedPath);
                        break;
                }
            }
            catch (OperationCanceledException e)
            {
                if (e.CancellationToken == _update_cancellation.Token)
                {
                    Log.Info("Update cancellation detected, clearing update store", LogMessageGroup);
                    Store.State = UpdateStore.States.Empty;
                    _update_cancellation.Dispose();
                    _update_cancellation = new();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, LogMessageGroup);
                UpdateFailure?.Invoke(this, Store.Manifest!, _update_cancellation);
            }
        }
    }

    /// <summary>
    /// Performs actions after the update package has been expanded.
    /// </summary>
    /// <param name="expandedPackagePath">The file system path where the update package has been expanded.</param>
    /// <remarks>The default implementation requests a full device reset to apply the update.  Platforms with
    /// specific requirements, such as embedded Linux, may override this  method to implement alternative behaviors,
    /// such as restarting only the application process.</remarks>
    protected virtual void AfterUpdatePackageExpanded(string expandedPackagePath)
    {
        // default behavior (i.e. F7) is to fully reset the device.
        // some platforms (e.g. embedded linux?) might decide to only restart the app process?
        Log.Debug($"Requesting a device reset to apply the Update");
        Device.PlatformOS.Reset();
    }

    /// <summary>
    /// Generates a unique temporary folder path for storing update-related files.
    /// </summary>
    /// <remarks>The folder path is created by combining the system's temporary directory with a unique
    /// identifier. This method is virtual and can be overridden to customize the location or naming convention of the
    /// temporary folder.</remarks>
    /// <returns>A string representing the full path to the temporary folder.</returns>
    protected virtual string GetUpdateTempFolder()
    {
        // TODO: Replace with Path.GetTempPath() when https://github.com/WildernessLabs/Meadow/pull/734 lands
        return MeadowOS.FileSystem.TempDirectory;
    }

    private void OnMqttMessageReceived(object sender, MqttApplicationMessage e)
    {
        Log.Debug("MQTT message received", LogMessageGroup);
        if (!e.Topic.EndsWith($"/ota/{Device.Information.UniqueID}", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Log.Debug("MQTT message is OTA message for this device", LogMessageGroup);

        try
        {
            var temp_path = GetUpdateTempFolder();
            Log.Debug($"Using temp path {temp_path}", LogMessageGroup);

            if (!Directory.Exists(temp_path))
            {
                Log.Debug($"Creating temp directory {temp_path}", LogMessageGroup);
                Directory.CreateDirectory(temp_path);
            }

            var packageFileName = Path.Combine(temp_path, $"manifest_{Guid.NewGuid():N}");

            Log.Debug($"Writing package to {packageFileName}", LogMessageGroup);

            File.WriteAllBytes(
                packageFileName,
                e.PayloadSegment.Array);

            var fileInfo = new FileInfo(packageFileName);
            Log.Debug($"file size: {fileInfo.Length}", LogMessageGroup);

            if (Store.State != UpdateStore.States.Empty)
            {
                Log.Info("New update received, cancelling any in-progress update", LogMessageGroup);

                // cancel old update
                var current_update_cancellation = _update_cancellation;
                current_update_cancellation.Cancel();
                while (current_update_cancellation == _update_cancellation)
                {
                    Task.Delay(1000).Wait();
                }
                Store.State = UpdateStore.States.Empty; // should not be needed
            }

            Store.AddManifest(packageFileName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, LogMessageGroup);
        }
    }

    /// <inheritdoc/>
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

    /// <summary>
    /// Starts the service if it is not already running
    /// </summary>
    public void Start()
    {
        UpdateServiceTask = Task.Run(async () =>
        {
            try
            {
                await UpdateService();
            }
            catch (Exception e)
            {
                Log.Error(e, LogMessageGroup);
                Stop();
            }
        }, _service_cancellation.Token);
        State = UpdateState.Connected;
        Log.Info("Update Service started", LogMessageGroup);
    }

    /// <inheritdoc/>
    public void Stop()
    {
        _service_cancellation.Cancel();
        _service_cancellation.Dispose();
        _service_cancellation = new();
        State = UpdateState.Disconnected;
        Log.Info("Update Service stopped", LogMessageGroup);
    }

    private async Task SafeDownloadFile(string url, UpdateInfo message, CancellationTokenSource cancel)
    {
        Log.Debug($"Attempting to retrieve {url}", LogMessageGroup);
        var sw = Stopwatch.StartNew();

        long totalBytesDownloaded = 0;

        var read_buffer = ArrayPool<byte>.Shared.Rent(DownloadBufferSize);
        var write_buffer = ArrayPool<byte>.Shared.Rent(DownloadBufferSize);
        try
        {
            using var httpClient = new HttpClient();

            // Set timeout for the entire operation
            httpClient.Timeout = TimeSpan.FromMinutes(20); // 20 min should download 10MB at 10KB/sec - and we can always resume

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (_connectionService.Settings.UseAuthentication)
            {
                request.Headers.Authorization = _connectionService.CreateAuthenticationHeaderValue();
            }

            // Configure the HTTP range header to indicate resumption of partial download, starting from 
            // the 'EOF' byte position of the partial download and extending to the end of the content.
            using var fileStream = Store.StartMpak();
            Log.Debug($"Resuming from offset {fileStream.Length}", LogMessageGroup);
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(fileStream.Length, null);

            using var sendCts = new CancellationTokenSource(millisecondsDelay: 15000);
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, sendCts.Token);

            // Handle Content-Range header (for resumable downloads)
            var rangeContentLength = response.Content.Headers.ContentLength;
            var totalContentLength = response.Content.Headers.ContentRange?.Length;
            var contentDigest = response.Headers.GetContentDigest();

            // Handle RequestedRangeNotSatisfiable which means the either the file is fully downloaded, or
            // the file on disk is larger than the file on the server (indicating an incorrect file)
            if (response.StatusCode == System.Net.HttpStatusCode.RequestedRangeNotSatisfiable)
            {
                Log.Info("Server responded with 416 Range Not Satisfiable. Assuming file is fully downloaded.", LogMessageGroup);

                // Validate download completion
                PerformMpakValidation(contentDigest, totalContentLength, fileStream.Length);
                return;
            }

            response.EnsureSuccessStatusCode();

            // Determine the total file size
            if (totalContentLength.HasValue)
            {
                // Content-Range header provides the total file size (preferred for resumable downloads)
                message.FileSize = totalContentLength.Value;
                Log.Debug($"Total file size from Content-Range: {message.FileSize:N0} bytes.", LogMessageGroup);
            }
            else if (rangeContentLength.HasValue && fileStream.Length == 0)
            {
                // First download attempt - Content-Length is the total size
                message.FileSize = rangeContentLength.Value;
                Log.Debug($"Total file size from Content-Length: {message.FileSize:N0} bytes.", LogMessageGroup);
            }
            else if (!rangeContentLength.HasValue)
            {
                Log.Warn("Server did not provide Content-Length header. Progress tracking may be inaccurate.", LogMessageGroup);
                // FileSize remains at its previous value or 0 if not set
            }

            // Validate that we got some length information for resumed downloads
            if (fileStream.Length > 0 && !totalContentLength.HasValue && !rangeContentLength.HasValue)
            {
                Log.Warn("Resuming download but server provided no length headers. Download may be incomplete.", LogMessageGroup);
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            int bytesRead;

            // Create a cancellation token with timeout to prevent stalled downloads
            using var downloadCts = new CancellationTokenSource();
            var lastProgressTime = DateTime.UtcNow;
            using var progressTimer = new Timer(_ =>
            {
                Log.Trace($"{DateTime.UtcNow - lastProgressTime} since last chunk downloaded", LogMessageGroup);
                if ((DateTime.UtcNow - lastProgressTime) > TimeSpan.FromMinutes(1))
                {
                    Log.Warn("Stall detected, cancelling download", LogMessageGroup);
                    downloadCts.Cancel();
                }
            }, null, 30000, 30000);

            Task writeTask = Task.CompletedTask;
            while ((bytesRead = await stream.ReadAsync(read_buffer, 0, read_buffer.Length, downloadCts.Token)) > 0)
            {
                await writeTask; // wait for last write to disk task to complete
                Buffer.BlockCopy(read_buffer, 0, write_buffer, 0, bytesRead);

                writeTask = fileStream.WriteAsync(write_buffer, 0, bytesRead, downloadCts.Token);

                totalBytesDownloaded += bytesRead;

                // Update progress tracking timestamp
                lastProgressTime = DateTime.UtcNow;

                message.DownloadProgress = totalBytesDownloaded;

                RetrieveProgress?.Invoke(this, message, _update_cancellation);
                _update_cancellation.Token.ThrowIfCancellationRequested();
                cancel.Token.ThrowIfCancellationRequested();

                Log.Trace($"Download progress: {totalBytesDownloaded:N0} bytes downloaded", LogMessageGroup);
            }
            await writeTask; // wait for last write to disk task to complete
            sw.Stop();
            Log.Debug($"Download complete: {totalBytesDownloaded} bytes in {sw.Elapsed.TotalSeconds} secs.", LogMessageGroup);

            // Validate download completion
            PerformMpakValidation(contentDigest, rangeContentLength, totalBytesDownloaded);
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            Log.Error($"Download timed out or cancelled after {sw.Elapsed.TotalSeconds:0} seconds", LogMessageGroup);
            await Task.Delay(RetryDelayMilliseconds);
            throw;
        }
        catch (MpakValidationFailedException ex)
        {
            sw.Stop();
            Log.Error($"Download failed mpak validation: {ex.Message}", LogMessageGroup);
            await Task.Delay(RetryDelayMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            Log.Error($"[ {ex.GetType().Name} ] Failed to download Update after {sw.Elapsed.TotalSeconds:0} seconds: {ex.Message} {ex.StackTrace}", LogMessageGroup);
            await Task.Delay(RetryDelayMilliseconds);
            throw;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(read_buffer);
            ArrayPool<byte>.Shared.Return(write_buffer);
        }
    }

    private async Task DownloadProc(UpdateMessage message, CancellationTokenSource cancel)
    {
        Log.Debug($"Device OS Version: {Resolver.Device.PlatformOS.OSVersion}, Update OS Version: {message.OsVersion}", LogMessageGroup);

        var destination = message.MpakDownloadUrl;

        if (!string.IsNullOrEmpty(message.OsVersion)
            && Device.PlatformOS.OSVersion != message.OsVersion)
        {
            Log.Debug($"This OTA requires an OS update.", LogMessageGroup);
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

        await SafeDownloadFile(destination, message, cancel);

        State = _connectionService.ConnectionState == CloudConnectionState.Connected ? UpdateState.Connected : UpdateState.Disconnected;
    }

    /// <inheritdoc/>
    protected virtual string ApplyUpdate(UpdateInfo updateInfo)
    {
        var sourcePath = Store.MpakPath;
        Log.Debug($"Applying update from '{sourcePath}' to {UpdateDirectory} ", LogMessageGroup);

        try
        {
            // extract zip
            var sw = Stopwatch.StartNew();
            MeadowOS.SafelyExtractZIPFile(sourcePath, UpdateDirectory);
            sw.Stop();
            Log.Debug($"Extracting took {sw.Elapsed.TotalSeconds} seconds.", LogMessageGroup);

            Log.Debug($"Contents of Update", LogMessageGroup);
            Log.Debug($"------------------", LogMessageGroup);
            DisplayTree(new DirectoryInfo(UpdateDirectory));

            return UpdateDirectory;
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to extract update package: {ex.Message}", LogMessageGroup);
            throw;
        }
        finally
        {
            try
            {
                Log.Debug($"Deleting source file '{sourcePath}'", LogMessageGroup);
                File.Delete(sourcePath!);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to delete source file: {ex.Message}", LogMessageGroup);
            }
        }
    }

    private void DisplayTree(DirectoryInfo di)
    {
        Log.Debug($"+ {di.Name}", LogMessageGroup);
        foreach (var f in di.GetFiles())
        {
            Log.Debug($"  - {f.Name}", LogMessageGroup);
        }
        foreach (var d in di.GetDirectories())
        {
            DisplayTree(d);
        }
    }

    private void PerformMpakValidation(ContentDigestItem? contentDigest, long? expectedDownloadSize, long actualDownloadSize)
    {
        // Validate download size against expected size, if provided
        if (expectedDownloadSize.HasValue && expectedDownloadSize.Value != actualDownloadSize)
        {
            Store.DeleteMpak();
            throw new MpakValidationFailedException($"Download size mismatch. Expected {expectedDownloadSize.Value:N0} bytes but received {actualDownloadSize:N0} bytes.");
        }

        // Validate CRC, if provided
        if (contentDigest != null && !Store.ValidateMpak(contentDigest.Algorithm, contentDigest.Value, out var actualHash))
        {
            Store.DeleteMpak();
            throw new MpakValidationFailedException($"CRC hash mismatch. Expected {contentDigest.Value} but received {actualHash}.");
        }
        else if (contentDigest == null)
        {
            Log.Debug($"Skipping CRC hash check. Hash was not provided by server.", LogMessageGroup);
        }

        Log.Debug($"Download validation successful.", LogMessageGroup);
    }
}
