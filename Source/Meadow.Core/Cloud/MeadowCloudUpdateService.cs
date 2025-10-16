using Meadow.Update;
using MQTTnet;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

using static Resolver;

internal class MeadowCloudUpdateService : IUpdateService
{
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

    private readonly byte[] read_buffer = new byte[1024 * 384]; // TODO: make this configurable/platform dependent
    private readonly byte[] write_buffer = new byte[1024 * 384]; // TODO: make this configurable/platform dependent

    private const string DefaultUpdateStoreDirectoryName = "update-store";
    private const string DefaultUpdateDirectoryName = "update";

    private readonly MeadowCloudConnectionService _connectionService;
    private UpdateState _state = UpdateState.Disconnected;

    private UpdateStore Store { get; }
    private string UpdateDirectory { get; }
    private string UpdateStoreDirectory { get; }

    private CancellationTokenSource _service_cancellation = new();
    private CancellationTokenSource _update_cancellation = new();
    public Task UpdateServiceTask = Task.CompletedTask;

    public MeadowCloudUpdateService(string fsRoot, MeadowCloudConnectionService connectionService)
    {
        UpdateStoreDirectory = Path.Combine(fsRoot, DefaultUpdateStoreDirectoryName);
        UpdateDirectory = Path.Combine(fsRoot, DefaultUpdateDirectoryName);
        Store = new UpdateStore(UpdateStoreDirectory);

        _connectionService = connectionService;
        _connectionService.MqttMessageReceived += OnMqttMessageReceived;
        _connectionService.AddSubscription("{OID}/ota/{ID}");
    }

    private async Task UpdateService()
    {
        while (!Resolver.App.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(UpdateServicePulseMilliseconds);
            Log.Trace($"store state: {Store.State} cloud connection state: {_connectionService.ConnectionState}", "update service");

            _service_cancellation.Token.ThrowIfCancellationRequested();
            if (_connectionService.ConnectionState != CloudConnectionState.Connected && Store.State != UpdateStore.States.Mpak)
                continue;


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
                        Log.Debug("update available", "update service");
                        UpdateAvailable?.Invoke(this, Store.Manifest!, _update_cancellation);
                        _update_cancellation.Token.ThrowIfCancellationRequested();
                        await DownloadProc(Store.Manifest!, service_or_update_cancellation);
                        Store.CompleteMpak();

                        break;
                    case UpdateStore.States.Mpak:
                        Log.Debug("update retrieved", "update service");
                        UpdateRetrieved?.Invoke(this, Store.Manifest!, _update_cancellation);
                        _update_cancellation.Token.ThrowIfCancellationRequested();
                        ApplyUpdate(Store.Manifest!);
                        Store.State = UpdateStore.States.Empty;
                        Log.Debug($"Requesting a device reset to apply the Update");
                        Device.PlatformOS.Reset();
                        UpdateSuccess?.Invoke(this, Store.Manifest!, _update_cancellation); // not called - device has reset by now
                        break;
                }
            }
            catch (OperationCanceledException e)
            {
                if (e.CancellationToken == _update_cancellation.Token)
                {
                    Log.Info("Update cancellation detected, clearing update store", "update service");
                    Store.State = UpdateStore.States.Empty;
                    _update_cancellation.Dispose();
                    _update_cancellation = new();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "update service");
                UpdateFailure?.Invoke(this, Store.Manifest!, _update_cancellation);
            }
        }
    }

    private void OnMqttMessageReceived(object sender, MqttApplicationMessage e)
    {
        Log.Debug("MQTT message received", "update service");
        if (!e.Topic.EndsWith($"/ota/{Device.Information.UniqueID}", StringComparison.OrdinalIgnoreCase))
            return;

        Log.Debug("MQTT message is OTA message for this device", "update service");

        try
        {
            var temp_path = Path.Combine(
                MeadowOS.FileSystem.TempDirectory,
                $"manifest_{Guid.NewGuid():N}"); // TODO: Replace with Path.GetTempPath() when https://github.com/WildernessLabs/Meadow/pull/734 lands
            Log.Debug($"Using temp path {temp_path}");
            File.WriteAllBytes(temp_path, e.PayloadSegment.ToArray());

            if (Store.State != UpdateStore.States.Empty)
            {
                // cancel old update
                var current_update_cancellation = _update_cancellation;
                current_update_cancellation.Cancel();
                while (current_update_cancellation == _update_cancellation)
                {
                    Task.Delay(1000).Wait();
                }
                Store.State = UpdateStore.States.Empty; // should not be needed
            }

            Store.AddManifest(temp_path);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "update service");
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
                Log.Error(e, "updater service");
                Stop();
            }
        }, _service_cancellation.Token);
        State = UpdateState.Connected;
        Log.Info("Update Service started", "update service");
    }

    /// <inheritdoc/>
    public void Stop()
    {
        _service_cancellation.Cancel();
        _service_cancellation.Dispose();
        _service_cancellation = new();
        State = UpdateState.Disconnected;
        Log.Info("Update Service stopped", "update service");
    }

    private async Task SafeDownloadFile(string url, UpdateInfo message, CancellationTokenSource cancel)
    {
        Log.Debug($"Attempting to retrieve {url}");
        var sw = Stopwatch.StartNew();

        long totalBytesDownloaded = 0;

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
            Log.Debug($"Resuming from offset {fileStream.Length}", "update service");
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
                Log.Info("Server responded with 416 Range Not Satisfiable. Assuming file is fully downloaded.", "update service");

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
                Log.Debug($"Total file size from Content-Range: {message.FileSize:N0} bytes.", "update service");
            }
            else if (rangeContentLength.HasValue && fileStream.Length == 0)
            {
                // First download attempt - Content-Length is the total size
                message.FileSize = rangeContentLength.Value;
                Log.Debug($"Total file size from Content-Length: {message.FileSize:N0} bytes.", "update service");
            }
            else if (!rangeContentLength.HasValue)
            {
                Log.Warn("Server did not provide Content-Length header. Progress tracking may be inaccurate.", "update service");
                // FileSize remains at its previous value or 0 if not set
            }

            // Validate that we got some length information for resumed downloads
            if (fileStream.Length > 0 && !totalContentLength.HasValue && !rangeContentLength.HasValue)
            {
                Log.Warn("Resuming download but server provided no length headers. Download may be incomplete.", "update service");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            int bytesRead;

            // Create a cancellation token with timeout to prevent stalled downloads
            using var downloadCts = new CancellationTokenSource();
            var lastProgressTime = DateTime.UtcNow;
            using var progressTimer = new Timer(_ =>
            {
                Log.Trace($"{DateTime.UtcNow - lastProgressTime} since last chunk downloaded", "update service");
                if ((DateTime.UtcNow - lastProgressTime) > TimeSpan.FromMinutes(1))
                {
                    Log.Warn("Stall detected, cancelling download", "update service");
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

                Log.Trace($"Download progress: {totalBytesDownloaded:N0} bytes downloaded", "update service");
            }
            await writeTask; // wait for last write to disk task to complete
            sw.Stop();
            Log.Debug($"Download complete: {totalBytesDownloaded} bytes in {sw.Elapsed.TotalSeconds} secs.", "update service");

            // Validate download completion
            PerformMpakValidation(contentDigest, rangeContentLength, totalBytesDownloaded);
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            Log.Error($"Download timed out or cancelled after {sw.Elapsed.TotalSeconds:0} seconds", "update service");
            await Task.Delay(RetryDelayMilliseconds);
            throw;
        }
        catch (MpakValidationFailedException ex)
        {
            sw.Stop();
            Log.Error($"Download failed mpak validation: {ex.Message}", "update service");
            await Task.Delay(RetryDelayMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            Log.Error($"[ {ex.GetType().Name} ] Failed to download Update after {sw.Elapsed.TotalSeconds:0} seconds: {ex.Message} {ex.StackTrace}", "update service");
            await Task.Delay(RetryDelayMilliseconds);
            throw;
        }
    }

    private async Task DownloadProc(UpdateMessage message, CancellationTokenSource cancel)
    {
        Log.Debug($"Device OS Version: {Resolver.Device.PlatformOS.OSVersion}, Update OS Version: {message.OsVersion}");

        var destination = message.MpakDownloadUrl;

        if (!string.IsNullOrEmpty(message.OsVersion)
            && Device.PlatformOS.OSVersion != message.OsVersion)
        {
            Log.Debug($"This OTA requires an OS update.");
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
    private void ApplyUpdate(UpdateInfo updateInfo)
    {
        var sourcePath = Store.MpakPath;
        Log.Debug($"Applying update from '{sourcePath}'");

        try
        {
            // extract zip
            var sw = Stopwatch.StartNew();
            MeadowOS.SafelyExtractZIPFile(sourcePath, UpdateDirectory);
            sw.Stop();
            Log.Debug($"Extracting took {sw.Elapsed.TotalSeconds} seconds.");

            Log.Debug($"Contents of Update");
            Log.Debug($"------------------");
            DisplayTree(new DirectoryInfo(UpdateDirectory));
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to extract update package: {ex.Message}");
            throw ex;
        }
        finally
        {
            try
            {
                File.Delete(sourcePath!);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to delete source file: {ex.Message}");
            }
        }
    }

    private void DisplayTree(DirectoryInfo di)
    {
        Log.Debug($"+ {di.Name}");
        foreach (var f in di.GetFiles())
        {
            Log.Debug($"  - {f.Name}");
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
            Log.Debug($"Skipping CRC hash check. Hash was not provided by server.", "update service");
        }

        Log.Debug($"Download validation successful.", "update service");
    }
}
