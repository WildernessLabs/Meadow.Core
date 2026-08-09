using Meadow.Cloud;
using Meadow.Hardware;
using Meadow.Update;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SRI = System.Runtime.InteropServices;

namespace Meadow;

/// <summary>
/// Provides functionality to manage and maintain a connection to the Meadow Cloud service.
/// </summary>
/// <remarks>This service handles authentication, message queuing, and communication with the Meadow Cloud. It
/// supports automatic reconnection, message retrying, and subscription management for MQTT topics. The service also
/// provides events for monitoring connection state, error occurrences, and message transmissions.</remarks>
public class MeadowCloudConnectionService : IMeadowCloudService
{
    /// <inheritdoc/>
    public event EventHandler<Exception>? ErrorOccurred;
    /// <inheritdoc/>
    public event EventHandler? MessageSent;
    /// <inheritdoc/>
    public event EventHandler<CloudConnectionState>? ConnectionStateChanged;

    internal event EventHandler<MqttApplicationMessage>? MqttMessageReceived;

    /// <summary>
    /// Retry period the service will use to attempt network reconnection
    /// </summary>
    public const int NetworkRetryTimeoutSeconds = 3;
    /// <summary>
    /// Auth token expiration period in minutes.
    /// TODO: Replace this hard-coded value with one retrieved from the Meadow Cloud.
    /// </summary>
    public const int TokenExpirationPeriod = 60;

    private const int HttpClientTimeoutSeconds = 60;

    internal IMeadowCloudSettings Settings { get; private set; }

    private readonly List<string> _subscriptionTopics = new();
    private bool _stopService = true;
    private bool _firstConection = true;
    private DateTime _lastAuthenticationTime = DateTime.MinValue;
    private string? _jwt = null;
    private int _consecutiveMqttConnectFailures = 0;
    private CloudConnectionState _connectionState = CloudConnectionState.Unknown;
    private Task? _stateMachineTask;
    private static readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private readonly SemaphoreSlim _forwarderSemaphore = new(1, 1);
    private readonly CloudDataQueue _dataQueue;
    private Timer? _dataForwarderTimer;

    private MqttClientOptions? ClientOptions { get; set; } = default!;
    private MqttClient MqttClient { get; set; } = default!;

    // Static HttpClient instances for connection reuse (prevent socket exhaustion).
    // PooledConnectionLifetime=Zero disables HTTPS connection reuse: a separate
    // usrsock/mbedTLS connection-reuse issue (a kept-alive TLS connection can
    // produce a record the server rejects as bad_record_mac) is still open, so a
    // fresh TCP+TLS connection per request keeps the cloud path reliable. Revisit
    // once that's fixed (see project_meadowcloud_tls_intermittent).
    private static HttpClient CreateCloudHttpClient(int timeoutSeconds) =>
        new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.Zero,
            PooledConnectionIdleTimeout = TimeSpan.Zero,
        })
        {
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };

    private static readonly HttpClient _dataHttpClient = CreateCloudHttpClient(HttpClientTimeoutSeconds);

    // Timeout is updated from Settings.AuthTimeoutSeconds in Initialize().
    private static readonly HttpClient _authHttpClient = CreateCloudHttpClient(HttpClientTimeoutSeconds);

    /// <inheritdoc/>
    public bool IsEnabled { get; private set; }

    /// <inheritdoc/>
    public DateTimeOffset? LastSuccessfulSend { get; private set; }

    // Track the last time we were connected
    private DateTime _lastConnectedTime = DateTime.UtcNow;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeadowCloudConnectionService"/> class with the specified cloud
    /// settings.
    /// </summary>
    /// <remarks>The constructor determines the telemetry store type based on the <see
    /// cref="IMeadowCloudSettings.TelemetryStore"/> value. If the value is "sqlite" (case-insensitive), a SQLite-based
    /// telemetry store is used,  with the database file stored in the device's file system. Otherwise, an in-memory
    /// telemetry store is used.</remarks>
    /// <param name="settings">The cloud settings used to configure the connection service.  This includes the telemetry store type and other
    /// related configuration options.</param>
    public MeadowCloudConnectionService(IMeadowCloudSettings settings)
    {
        Settings = settings;

        if (string.Compare(settings.TelemetryStore, "sqlite", true) == 0)
        {
            Resolver.Log.Info("Using SQLite telemetry store", "cloud");

            var path = Path.Combine(Resolver.Device.PlatformOS.FileSystem.FileSystemRoot, "cloud", "telemetry.sqlite");
            _dataQueue = new CloudDataQueue(
                new SqliteTelemetryStore(path, 50, 1000));
        }
        else
        {
            Resolver.Log.Info("Using in-memory telemetry store", "cloud");

            _dataQueue = new CloudDataQueue(
                new InMemoryTelemetryStore());
        }
    }

    /// <inheritdoc/>
    public CloudConnectionState ConnectionState
    {
        get => _connectionState;
        private set
        {
            if (value == ConnectionState) return;
            _connectionState = value;
            ConnectionStateChanged?.Invoke(this, ConnectionState);
        }
    }

    /// <inheritdoc/>
    public int QueueCount { get => _dataQueue.Count; }

    private async Task DataForwarderProc()
    {
        PetLivenessWatchdog();

        if (!await _forwarderSemaphore.WaitAsync(0))
        {
            Resolver.Log.Trace("DataForwarder already running", "cloud");
            return;
        }

        try
        {
            while (_dataQueue.Count > 0)
            {
                try
                {
                    if (ConnectionState == CloudConnectionState.Connected)
                    {
                        // optimization: if more than one item is queued, try bulk send
                        if (_dataQueue.Count > 1)
                        {
                            // gather a batch of items with same endpoint (up to a reasonable limit)
                            const int MaxBatchSize = 50; // arbitrary cap to avoid overly large payloads
                            var first = _dataQueue.Peek();
                            if (first == null)
                            {
                                break;
                            }

                            var endpoint = GetBulkEndpoint(first.EndPoint);
                            var batch = new List<object>();
                            var originalItems = new List<CloudTelemetryItem>();

                            // We only dequeue after successful send; so peek then dequeue into temp list
                            // copy items with same original endpoint (not bulk) to maintain grouping
                            while (_dataQueue.Count > 0 && batch.Count < MaxBatchSize)
                            {
                                var next = _dataQueue.Peek();
                                if (next == null || next.EndPoint != first.EndPoint)
                                {
                                    // stop grouping when endpoint differs to keep semantics
                                    break;
                                }
                                // remove from queue, but keep reference in case we need to re-add on failure
                                next = _dataQueue.Dequeue();
                                if (next == null) break;
                                batch.Add(next.Item);
                                originalItems.Add(next);
                            }

                            if (batch.Count == 1)
                            {
                                // fallback to single send semantics
                                if (await Send(batch[0], first.EndPoint))
                                {
                                    Resolver.Log.Trace("Data queue sent single item (fallback from bulk)", "cloud");
                                }
                                else
                                {
                                    // failed; re-enqueue original item preserving priority
                                    foreach (var oi in originalItems)
                                    {
                                        _dataQueue.Enqueue(oi);
                                    }
                                }
                            }
                            else if (batch.Count > 1)
                            {
                                // perform bulk send
                                if (await BulkSend(batch, endpoint))
                                {
                                    Resolver.Log.Trace($"Bulk send succeeded for {batch.Count} items", "cloud");
                                }
                                else
                                {
                                    Resolver.Log.Trace("Bulk send failed; restoring items to queue", "cloud");
                                    // restore in original order by priority (originalItems already ordered)
                                    foreach (var oi in originalItems)
                                    {
                                        _dataQueue.Enqueue(oi);
                                    }
                                    // small back-off to avoid tight failure loops
                                    await Task.Delay(TimeSpan.FromSeconds(3));
                                }
                            }
                        }
                        else // single item path
                        {
                            var info = _dataQueue.Peek();
                            if (info != null)
                            {
                                if (await Send(info.Item, info.EndPoint))
                                {
                                    Resolver.Log.Trace("Data queue sent an item", "cloud");
                                    _dataQueue.Dequeue();
                                }
                                else
                                {
                                    // if send fails, add a short delay to avoid busy loop
                                    await Task.Delay(TimeSpan.FromSeconds(1));
                                }
                            }
                        }
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3));
                    }
                }
                catch (Exception ex)
                {
                    Resolver.Log.Trace($"Data queue exception!", "cloud");
                    LogAndRaiseOnErrorOccurredEvent("Unable to forward Meadow Cloud record", ex);
                    break;
                }
            }
        }
        finally
        {
            _forwarderSemaphore.Release();
        }
    }

    private string GetBulkEndpoint(string endpoint)
    {
        // explicit mapping for known endpoints
        // /api/logs -> /api/bulklogs
        // /api/events -> /api/bulkevents
        if (string.IsNullOrEmpty(endpoint)) return endpoint;
        return endpoint switch
        {
            "/api/logs" => "/api/bulklogs",
            "/api/events" => "/api/bulkevents",
            _ => endpoint // leave other endpoints unchanged unless future mapping required
        };
    }

    private async Task<bool> BulkSend<T>(IEnumerable<T> items, string endpoint)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        if (!IsEnabled)
        {
            Resolver.Log.Warn("MeadowCloud is not enabled. BulkSend call will not deliver data");
        }

        if (ConnectionState != CloudConnectionState.Connected)
        {
            return false;
        }

        // Convert to list to avoid multiple enumeration and for count logging
        var itemList = items as IList<T> ?? items.ToList();
        if (itemList.Count == 0) return true; // nothing to send

        if (!await _semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(60)))
        {
            return false;
        }

        try
        {
            int attempt = 0;
            int maxRetries = 1;
            int authRetries = 0;
            int maxAuthRetries = 3;
            while (true)
            {
                if (Settings.UseAuthentication)
                {
                    if (_jwt == null)
                    {
                        if (await Authenticate() == false)
                        {
                            authRetries++;
                            if (authRetries >= maxAuthRetries)
                            {
                                return false;
                            }
                            Resolver.Log.Error($"Failed to authenticate with Meadow.Cloud. Retrying in {Settings.ConnectRetrySeconds} seconds... (attempt {authRetries}/{maxAuthRetries})");
                            await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                            continue; // retry auth
                        }
                    }
                    _dataHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
                }

                var json = Resolver.JsonSerializer.Serialize(itemList);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                Resolver.Log.Debug($"making bulk cloud request to {endpoint} with {itemList.Count} items", "cloud");

                // Wrap HTTP POST with timeout to prevent infinite hangs
                var postTask = _dataHttpClient.PostAsync(endpoint, content);
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(HttpClientTimeoutSeconds));
                var completedTask = await Task.WhenAny(postTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    Resolver.Log.Error($"BulkSend() HTTP POST to {endpoint} timed out after {HttpClientTimeoutSeconds} seconds - network may be hung", "cloud");
                    break; // Exit retry loop on timeout
                }

                HttpResponseMessage response = await postTask;
                Resolver.Log.Trace($">>> BulkSend() post complete", "cloud");
                try
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized && attempt < maxRetries)
                    {
                        attempt++;
                        Resolver.Log.Trace($">>> BulkSend() unauthorized, invalidating and retry", "cloud");
                        InvalidateAuthentication();
                        _dataHttpClient.DefaultRequestHeaders.Authorization = null; // Clear old auth header
                        continue; // redo with new auth
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        string errorMessage = response.StatusCode == HttpStatusCode.InternalServerError
                            ? $"bulk cloud request to {endpoint} failed with {response.StatusCode}: '{responseContent}'"
                            : $"bulk cloud request to {endpoint} failed with {response.StatusCode}";
                        LogAndRaiseOnErrorOccurredEvent(errorMessage);
                        return false;
                    }
                    Resolver.Log.Debug($"bulk cloud request to {endpoint} completed successfully", messageGroup: "cloud");
                    MessageSent?.Invoke(this, EventArgs.Empty);
                    LastSuccessfulSend = DateTime.UtcNow;
                    return true;
                }
                finally
                {
                    response.Dispose();
                }
            }
        }
        catch (TaskCanceledException ex)
        {
            LogAndRaiseOnErrorOccurredEvent($"Bulk cloud request to {endpoint} timed out after {HttpClientTimeoutSeconds} seconds", ex);
            return false;
        }
        catch (Exception ex)
        {
            LogAndRaiseOnErrorOccurredEvent("Exception sending bulk cloud message", ex);
            return false;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
        return true;
    }

    private void ReportFatalErrorToReliabilityService(MeadowCloudException exception)
    {
        Resolver
            .Services
            .Get<IReliabilityService>()
            ?.OnMeadowSystemError(new MeadowCloudSystemErrorInfo(exception));
    }

    private void LogAndRaiseOnErrorOccurredEvent(string message, Exception? ex = null)
    {
        if (ex != null)
        {
            message = $"{message}{Environment.NewLine}{ex.Message}";
            if (ex.HResult != 0)
            {
                message = $"{message}({ex.HResult})";
            }
            if (ex.InnerException != null)
            {
                message = $"{message}{Environment.NewLine}{ex.InnerException.Message}";
                if (ex.InnerException.HResult != 0)
                {
                    message = $"{message}({ex.InnerException.HResult})";
                }
            }
        }

        var raisedException = new MeadowCloudException(message, ex);
        // Was Debug; bumped to Error so users see why cloud features
        // aren't working at the default LogLevel=Information.
        Resolver.Log.Error(message, "cloud");

        ErrorOccurred?.Invoke(this, raisedException);
    }

    /// <inheritdoc/>
    public void AddSubscription(string topic)
    {
        // pause the state machine
        var previousState = ConnectionState;
        ConnectionState = CloudConnectionState.Paused;

        Task.Run(async () =>
        {
            if (previousState == CloudConnectionState.Connected)
            {
                foreach (var t in _subscriptionTopics)
                {
                    await MqttClient.UnsubscribeAsync(t);
                }
            }
            _subscriptionTopics.Add(topic);

            if (previousState == CloudConnectionState.Connected)
            {
                _consecutiveMqttConnectFailures = 0;
                ConnectionState = CloudConnectionState.Subscribing;
            }
            else
            {
                ConnectionState = previousState;
            }
        }).RethrowUnhandledExceptions();
    }

    /// <summary>
    /// Method to determine if the authentication is required based on whether the time since 
    /// the last authentication exceeds the token expiration period (in minutes).
    /// </summary>
    private bool ShouldAuthenticate()
    {
        return Settings.UseAuthentication && (ClientOptions == null || (DateTime.UtcNow - _lastAuthenticationTime).TotalMinutes >= TokenExpirationPeriod);
    }

    /// <summary>
    /// Starts the service if it is not already running
    /// </summary>
    public void Start()
    {
        _dataForwarderTimer = new Timer(
            (s) => DataForwarderProc().RethrowUnhandledExceptions(),
            null,
            TimeSpan.FromSeconds(30),
            TimeSpan.FromSeconds(30));

        if (_stateMachineTask == null)
        {
            _stateMachineTask = Task.Run(() => ConnectionStateMachine(), Resolver.App.CancellationToken);
            IsEnabled = true;
        }
    }

    /// <inheritdoc/>
    public void Stop()
    {
        _dataForwarderTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        _dataForwarderTimer?.Dispose();
        _dataForwarderTimer = null;
        _stopService = true;
        IsEnabled = false;
    }

    private void Initialize()
    {
        CreateMqttClient();

        // Configure auth client timeout and base address from settings
        _authHttpClient.Timeout = TimeSpan.FromSeconds(Settings.AuthTimeoutSeconds);
        _dataHttpClient.BaseAddress = new Uri(Settings.DataHostname);
    }

    private void CreateMqttClient()
    {
        var factory = new MqttClientFactory();
        MqttClient = (MqttClient)factory.CreateMqttClient();

        MqttClient.ConnectedAsync += (args) =>
        {
            Resolver.Log.Debug("MQTT connected", "cloud");
            _consecutiveMqttConnectFailures = 0;
            ConnectionState = CloudConnectionState.Subscribing;
            return Task.CompletedTask;
        };

        MqttClient.DisconnectedAsync += (args) =>
        {
            Resolver.Log.Debug("MQTT disconnected", "cloud");
            ConnectionState = CloudConnectionState.Disconnected;
            return Task.CompletedTask;
        };

        MqttClient.ApplicationMessageReceivedAsync += (args) =>
        {
            Resolver.Log.Debug($"MQTT message received at topic: {args.ApplicationMessage.Topic}", "cloud");
            MqttMessageReceived?.Invoke(this, args.ApplicationMessage);
            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// Replaces the MqttClient with a fresh instance. MQTTnet (5.x especially)
    /// permanently rejects ConnectAsync while a previous connect/disconnect is
    /// still pending on the same instance; after an abandoned or interrupted
    /// connect attempt, recreating the client is the reliable way back to a
    /// connectable state -- a device reset is not required.
    /// </summary>
    private void RecreateMqttClient()
    {
        var old = MqttClient;
        CreateMqttClient();

        // Dispose the old client in the background: its teardown may block on
        // adapter I/O timeouts and must not stall the state machine.
        _ = Task.Run(() =>
        {
            try { old?.Dispose(); }
            catch (Exception ex) { Resolver.Log.Trace($"Old MQTT client dispose: {ex.Message}", "cloud"); }
        });
    }

    private async Task ConnectionStateMachine()
    {
        _stopService = false;

        // we need to wait for the network stack to come up on the F7 platforms
        switch (Resolver.Device.Information.Platform)
        {
            case MeadowPlatform.F7FeatherV1:
            case MeadowPlatform.F7FeatherV2:
            case MeadowPlatform.F7CoreComputeV2:
                await Task.Delay(TimeSpan.FromSeconds(NetworkRetryTimeoutSeconds));
                break;
        }

        Resolver.Device.PlatformOS.TimeChanged += (s) =>
        {
            _lastAuthenticationTime = DateTime.UtcNow;
        };

        Initialize();

        ConnectionState = CloudConnectionState.Disconnected;

        var nic = Resolver.Device?.NetworkAdapters?.Primary<INetworkAdapter>();

        if (nic == null)
        {
            Resolver.Log.Error($"Meadow.Cloud service detected no network interface!", "meadow-cloud");
            return;
        }
        else
        {
            Resolver.Log.Debug($"Meadow.Cloud service will use the {nic.GetType().Name} network interface", "meadow-cloud");
        }

        nic.NetworkDisconnected += (s, e) =>
        {
            ConnectionState = CloudConnectionState.Disconnected;
        };

        var stopwatch = new Stopwatch();

        // update state machine
        try
        {
            while (!_stopService) // do not look at the App cancellation token - if the app is being shutdown, we still want to handle potential OtA updates
            {
                switch (ConnectionState)
                {
                    case CloudConnectionState.Disconnected:
                        if (nic == null || !nic.IsConnected) // not connected yet
                        {
                            if (!stopwatch.IsRunning) stopwatch.Restart();
                            Resolver.Log.Debug($"Meadow.Cloud service waiting for network connection ({stopwatch.Elapsed.TotalSeconds} s)", "cloud");

                            if (stopwatch.Elapsed.TotalMinutes > Settings.MaximumDisconnectTimeMinutes) // Restart device if not connected for more than the configured time
                            {
                                ReportFatalErrorToReliabilityService(
                            new MeadowCloudException(
                                $"Device has not been connected to Meadow.Cloud for more than {Settings.MaximumDisconnectTimeMinutes} minutes. Restarting device...",
                                null));
                                Resolver.Device?.PlatformOS.Reset();
                            }
                            await Task.Delay(TimeSpan.FromSeconds(NetworkRetryTimeoutSeconds));
                            continue;
                        }
                        else // we are connected
                        {
                            if (ShouldAuthenticate())
                            {
                                ConnectionState = CloudConnectionState.Authenticating;
                            }
                            else
                            {
                                ConnectionState = CloudConnectionState.Connecting;
                            }
                        }
                        break;
                    case CloudConnectionState.Authenticating:
                        try
                        {
                            stopwatch.Restart();

                            // Wrap authentication with a timeout to prevent infinite hangs.
                            // Use Settings.AuthTimeoutSeconds so the outer wrapper matches the
                            // inner cancel-token deadline; otherwise a 60s wrapper around a 90s
                            // inner timeout fires prematurely and looks like a "network hang".
                            var authTask = Authenticate();
                            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(Settings.AuthTimeoutSeconds));
                            var completedTask = await Task.WhenAny(authTask, timeoutTask);

                            if (completedTask == timeoutTask)
                            {
                                Resolver.Log.Error($"Authentication timed out after {Settings.AuthTimeoutSeconds} seconds - network may be hung");
                                ConnectionState = CloudConnectionState.Disconnected;
                                await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                            }
                            else if (await authTask)
                            {
                                Resolver.Log.Debug($"Authentication took {stopwatch.ElapsedMilliseconds:N} ms", "cloud");
                                stopwatch.Stop();
                                _lastAuthenticationTime = DateTime.UtcNow;
                                ConnectionState = CloudConnectionState.Connecting;
                            }
                            else
                            {
                                Resolver.Log.Error("Failed to authenticate with Meadow.Cloud");
                                ConnectionState = CloudConnectionState.Disconnected;
                                await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                            }
                        }
                        catch (Exception ae)
                        {
                            Resolver.Log.Error($"Failed to authenticate with Meadow.Cloud: {ae.Message}");
                            if (ae.InnerException != null)
                            {
                                Resolver.Log.Error($"Inner Exception ({ae.InnerException.GetType().Name}): {ae.InnerException.Message}");
                            }
                            ConnectionState = CloudConnectionState.Disconnected;
                            await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                        }
                        break;
                    case CloudConnectionState.Connecting:
                        if (ClientOptions == null)
                        {
                            var client_id = Resolver.Device?.Information.UniqueID.ToUpper();
                            Resolver.Log.Debug($"Creating MQTT client ID {client_id}", "cloud");
                            var builder = new MqttClientOptionsBuilder()
                                .WithClientId(client_id)
                                .WithTcpServer(Settings.MqttHostname, Settings.MqttPort)
                                .WithTlsOptions(options =>
                                {
                                    options.UseTls(Settings.MqttPort == 8883);
                                })
                                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
                                .WithCleanSession(false)
                                // Default MQTTnet keepalive (15s). Left at default so the recvfrom
                                // non-blocking fix is tested honestly: if the connection survives idle,
                                // it's because PINGREQ now goes out (thread freed), not a tolerant broker.
                                .WithSessionExpiryInterval(86400) // Keep the session for 1 day
                                .WithTimeout(TimeSpan.FromSeconds(HttpClientTimeoutSeconds));

                            if (Settings.UseAuthentication)
                            {
                                Resolver.Log.Debug("Adding MQTT creds", "cloud");
                                builder.WithCredentials(Resolver.Device?.Information.UniqueID.ToUpper(), _jwt);
                            }

                            ClientOptions = builder.Build();
                        }

                        if (nic != null && nic.IsConnected)
                        {
                            stopwatch.Stop();
                            try
                            {
                                Resolver.Log.Debug("Connecting MQTT client", "cloud");

                                // Wrap MQTT connect with an outer timeout so a hung network
                                // can't stall the state machine. The inner 30s token bounds
                                // the protocol attempt; the outer bound covers MQTTnet's
                                // post-cancel adapter teardown (up to Options.Timeout more).
                                var connectTask = MqttClient.ConnectAsync(ClientOptions, new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);
                                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(HttpClientTimeoutSeconds + 15));
                                var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                                if (completedTask == timeoutTask)
                                {
                                    // The abandoned ConnectAsync is still running inside
                                    // MQTTnet and holds the client's connection status;
                                    // another ConnectAsync on the same instance would throw
                                    // InvalidOperationException. Recreate the client and log
                                    // (not throw) the abandoned attempt's eventual outcome.
                                    Resolver.Log.Error($"MQTT connect exceeded {HttpClientTimeoutSeconds + 15}s - network may be hung; recreating MQTT client", "cloud");
                                    _consecutiveMqttConnectFailures++;
                                    _ = connectTask.ContinueWith(
                                        t => Resolver.Log.Trace($"Abandoned MQTT connect completed: {t.Exception?.InnerException?.GetType().Name ?? t.Status.ToString()}", "cloud"),
                                        TaskContinuationOptions.ExecuteSynchronously);
                                    RecreateMqttClient();
                                    ConnectionState = CloudConnectionState.Disconnected;
                                    await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                                    break;
                                }

                                // Single await: surfaces the REAL exception type for the typed
                                // catches below. (The previous connectTask.Result access threw
                                // AggregateException on faulted connects, bypassing them all.)
                                var connectResult = await connectTask;

                                if (connectResult.ResultCode == MqttClientConnectResultCode.NotAuthorized ||
                                        _consecutiveMqttConnectFailures >= 3)
                                {
                                    // A broker rejecting a stale/expired token does not always
                                    // return a NotAuthorized CONNACK -- it may simply close the
                                    // connection mid-authentication, which surfaces here with a
                                    // generic result code. Retrying with the same token then
                                    // loops forever. Invalidate on explicit NotAuthorized OR
                                    // after repeated connect failures of any kind (re-auth is
                                    // cheap and idempotent).
                                    Resolver.Log.Debug($"MQTT authentication error ({connectResult.ResultCode}, {_consecutiveMqttConnectFailures} consecutive failures), invalidating credentials", "cloud");
                                    _consecutiveMqttConnectFailures = 0;
                                    InvalidateAuthentication();
                                    await MqttClient.DisconnectAsync();
                                }
                            }
                            catch (InvalidOperationException ioe)
                            {
                                // MQTTnet refuses ConnectAsync while a previous
                                // connect/disconnect is pending on this instance. That is a
                                // client-object state problem, not a device problem: replace
                                // the client and keep retrying instead of resetting the
                                // device (which this path previously did).
                                Resolver.Log.Error(DescribeBrief("MQTT client in unconnectable state; recreating", ioe), "cloud");
                                _consecutiveMqttConnectFailures++;
                                RecreateMqttClient();
                                ConnectionState = CloudConnectionState.Disconnected;
                                await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                            }
                            catch (MqttCommunicationTimedOutException)
                            {
                                Resolver.Log.Debug("Timeout connecting to Meadow.Cloud", "cloud");
                                _consecutiveMqttConnectFailures++;
                                ConnectionState = CloudConnectionState.Disconnected;
                                //  just delay for a while
                                await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                            }
                            catch (MqttConnectingFailedException ce)
                            {
                                Resolver.Log.Debug(DescribeBrief("MQTT Error connecting to Meadow.Cloud", ce), "cloud");
                                ConnectionState = CloudConnectionState.Disconnected;
                                _consecutiveMqttConnectFailures++;

                                //  just delay for a while
                                await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                            }
                            catch (OperationCanceledException)
                            {
                                // Inner 30s token fired mid-attempt (DNS stall, unreachable
                                // broker, TLS hang). MQTTnet has already torn the attempt
                                // down; count it toward credential invalidation and retry.
                                Resolver.Log.Debug($"MQTT connect attempt canceled after 30s ({_consecutiveMqttConnectFailures + 1} consecutive failures)", "cloud");
                                _consecutiveMqttConnectFailures++;
                                ConnectionState = CloudConnectionState.Disconnected;
                                await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                            }
                            catch (MqttCommunicationException e)
                            {

                                Resolver.Log.Debug(DescribeBrief("MQTT Error connecting to Meadow.Cloud", e), "cloud");
                                _consecutiveMqttConnectFailures++;
                                ConnectionState = CloudConnectionState.Disconnected;
                                //  just delay for a while
                                await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                            }
                            catch (Exception ex)
                            {
                                Resolver.Log.Error(DescribeBrief("Error connecting to Meadow.Cloud", ex), "cloud");
                                if (ex.InnerException != null)
                                {
                                    Resolver.Log.Error($"Inner Exception ({ex.InnerException.GetType().Name}): {ex.InnerException.Message}", "cloud");
                                }
                                ConnectionState = CloudConnectionState.Disconnected;
                                //  just delay for a while
                                await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                            }
                        }
                        else
                        {
                            if (!stopwatch.IsRunning) stopwatch.Restart();

                            Resolver.Log.Debug($"Meadow.Cloud service waiting for network connection ({stopwatch.Elapsed.TotalSeconds} s)", "cloud");
                            await Task.Delay(TimeSpan.FromSeconds(NetworkRetryTimeoutSeconds));
                        }
                        break;
                    case CloudConnectionState.Subscribing:
                        try
                        {
                            JsonWebTokenPayload? jwtPayload = null;
                            if (Settings.UseAuthentication)
                            {
                                if (string.IsNullOrWhiteSpace(_jwt))
                                {
                                    throw new InvalidOperationException("Meadow.Cloud service authentication is enabled but no JWT is available");
                                }

                                jwtPayload = GetJsonWebTokenPayload(_jwt);
                            }

                            foreach (var topic in _subscriptionTopics)
                            {
                                string topicName = topic;

                                // look for macro-substitutions
                                if (topic.Contains("{OID}") && !string.IsNullOrWhiteSpace(jwtPayload?.OId))
                                {
                                    topicName = topicName.Replace("{OID}", jwtPayload.OId);
                                }

                                if (topic.Contains("{ID}"))
                                {
                                    topicName = topicName.Replace("{ID}", Resolver.Device?.Information.UniqueID.ToUpper());
                                }

                                Resolver.Log.Debug($"Meadow.Cloud service subscribing to '{topicName}'", "cloud");
                                await MqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                                                                    .WithTopic(topicName)
                                                                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                                                                    .Build());
                            }
                            ConnectionState = CloudConnectionState.Connected;
                        }
                        catch (Exception ex)
                        {
                            Resolver.Log.Error($"Error subscribing to Meadow.Cloud: {ex.Message}", "cloud");
                            if (ex.InnerException != null)
                            {
                                Resolver.Log.Error($"Inner Exception ({ex.InnerException.GetType().Name}): {ex.InnerException.Message}", "cloud");
                            }
                            // if subscribing fails, then we need to disconnect from the server
                            await MqttClient.DisconnectAsync();

                            ConnectionState = CloudConnectionState.Disconnected;
                        }
                        break;
                    case CloudConnectionState.Connected:
                        _lastConnectedTime = DateTime.UtcNow; // Update last connected time
                        if (_firstConection)
                        {
                            if (SendCrashReports())
                            {
                                _firstConection = false;
                            }
                        }

                        await Task.Delay(1000);
                        break;
                }
            }

        }
        catch (Exception ex)
        {
            ReportFatalErrorToReliabilityService(new MeadowCloudException("Meadow Cloud connection service has abnormally terminated", ex));
        }
        finally
        {
            ConnectionState = CloudConnectionState.Unknown;
            _stateMachineTask = null;
            // restart the device - see above TODO
            Resolver.Device?.PlatformOS.Reset();
        }
    }

    /// <summary>
    /// Bounded, allocation-light exception description for reconnect-path
    /// logging. Interpolating a full exception (ToString with stack traces)
    /// rents multi-KB char buffers; during reconnect churn the heap can be
    /// exhausted enough that the formatting itself throws OutOfMemory and
    /// kills the state machine (observed twice on hardware). Concatenating
    /// existing strings keeps the failure path cheap.
    /// </summary>
    private static string DescribeBrief(string context, Exception ex)
    {
        var inner = ex.InnerException;
        return inner == null
            ? string.Concat(context, ": ", ex.GetType().Name, ": ", ex.Message)
            : string.Concat(context, ": ", ex.GetType().Name, ": ", ex.Message,
                            " <- ", inner.GetType().Name, ": ", inner.Message);
    }

    private bool? _livenessDeviceExists;

    /// <summary>
    /// Pets the OS managed-liveness watchdog (F7: /dev/liveness). Running from
    /// this timer callback proves the .NET timer machinery is alive; if these
    /// pets stop, the OS lets the hardware watchdog reset the device instead
    /// of leaving it silently hung. No-op on platforms without the device.
    /// </summary>
    private void PetLivenessWatchdog()
    {
        try
        {
            _livenessDeviceExists ??= File.Exists("/dev/liveness");
            if (_livenessDeviceExists == true)
            {
                File.WriteAllText("/dev/liveness", "1");
            }
        }
        catch
        {
            // never let the heartbeat throw into the timer path
        }
    }

    private void InvalidateAuthentication()
    {
        Resolver.Log.Debug("Token explicitly invalidated", "cloud");
        // by setting this to null and retrying, Authenticate will get called
        ClientOptions = null;
        _jwt = null;
    }


    internal AuthenticationHeaderValue CreateAuthenticationHeaderValue()
    {
        return new AuthenticationHeaderValue("Bearer", _jwt);
    }

    // MicroJson reads OId via reflection; the trimmer can't see that and
    // strips the public setter on JsonWebTokenPayload. Anchor it.
    [System.Diagnostics.CodeAnalysis.DynamicDependency(
        System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties
        | System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor,
        typeof(JsonWebTokenPayload))]
    private JsonWebTokenPayload GetJsonWebTokenPayload(string jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt))
        {
            throw new ArgumentException($"'{nameof(jwt)}' cannot be null or whitespace.", nameof(jwt));
        }

        var tokenParts = jwt.Split('.');
        if (tokenParts.Length < 2)
        {
            throw new ArgumentException("Argument is an invalid JsonWebToken format.", nameof(jwt));
        }

        var payloadJson = Base64UrlDecode(tokenParts[1]);
        var payload = Resolver.JsonSerializer.Deserialize<JsonWebTokenPayload>(payloadJson)
            ?? throw new Exception("Payload could not be deserialized from JWT argument.");

        return payload;
    }

    // In order to not require the use of a library, this was taken from
    // RFC7515, Appendix C: https://datatracker.ietf.org/doc/html/rfc7515#appendix-C
    private static string Base64UrlDecode(string arg)
    {
        string s = arg;
        s = s.Replace('-', '+'); // 62nd char of encoding
        s = s.Replace('_', '/'); // 63rd char of encoding
        switch (s.Length % 4) // Pad with trailing '='s
        {
            case 0: break; // No pad chars in this case
            case 2: s += "=="; break; // Two pad chars
            case 3: s += "="; break; // One pad char
            default:
                throw new ArgumentException("Argument is an invalid base64url string", nameof(arg));
        }

        var bytes = Convert.FromBase64String(s);
        return Encoding.UTF8.GetString(bytes);
    }

    private class JsonWebTokenPayload
    {
        public string? OId { get; set; }
    }

    private class JsonIdPayload
    {
        public JsonIdPayload(string id)
        {
            Id = id;
        }

        public string Id { get; set; } = default!;
    }

    /// <inheritdoc/>
    // MicroJson reads/writes these private payloads via reflection; the trimmer
    // can't see that and strips their accessors. Anchor them so authentication
    // doesn't fail with "Property Get method was not found" on a published F7.
    [System.Diagnostics.CodeAnalysis.DynamicDependency(
        System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties,
        typeof(JsonIdPayload))]
    [System.Diagnostics.CodeAnalysis.DynamicDependency(
        System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties
        | System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor,
        typeof(MeadowCloudLoginResponseMessage))]
    public async Task<bool> Authenticate()
    {
        string errorMessage;

        Resolver.Log.Info($"Starting authentication with Meadow.Cloud...", "cloud");
        var json = Resolver.JsonSerializer.Serialize(new JsonIdPayload(Resolver.Device.Information.UniqueID.ToUpper()));
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var endpoint = $"{Settings.AuthHostname}/api/devices/login";
        Resolver.Log.Info($"Attempting to login to {endpoint} with {json}...", "cloud");

        try
        {
            Resolver.Log.Info($"Posting authentication request...", "cloud");
            // Honor the configured AuthTimeoutSeconds. The first HTTPS request after
            // boot is dominated by the one-time mono JIT of the SslStream/HTTP stack
            // (tens of seconds on the F7), so a generous timeout is required; a small
            // hard-coded value here would spuriously cancel the cold-start auth.
            var authTimeoutMs = Math.Max(5_000, Settings.AuthTimeoutSeconds * 1000);
            // Force a fresh TCP+TLS connection per attempt to dodge a NuttX/mbedTLS-port
            // bug: a second HTTPS request reusing a kept-alive TLS connection produces a
            // record the server rejects (bad_record_mac), ~50% failure rate. Real fix
            // belongs in pal_ssl_mbedtls.c; this is a working harness until then.
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };
            // Close the connection after the response so each auth uses a fresh TCP+TLS
            // connection (belt-and-suspenders with PooledConnectionLifetime=Zero), avoiding
            // the mbedTLS keep-alive bad_record_mac reuse bug.
            request.Headers.ConnectionClose = true;
            using var response = await _authHttpClient.SendAsync(request, new CancellationTokenSource(millisecondsDelay: authTimeoutMs).Token);
            Resolver.Log.Info($"Authentication response received: {response.StatusCode}", "cloud");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Resolver.Log.Info($"authentication successful. extracting token", "cloud");

                var payload = Resolver.JsonSerializer.Deserialize<MeadowCloudLoginResponseMessage>(responseContent);

                if (payload == null)
                {
                    Resolver.Log.Error($"invalid auth payload - deserialization failed", "cloud");
                    _jwt = null;
                    return false;
                }

                Resolver.Log.Info($"decrypting auth payload", "cloud");
                var encryptedKeyBytes = System.Convert.FromBase64String(payload.EncryptedKey);

                byte[]? decryptedKey;
                try
                {
                    var privateKey = GetPrivateKeyInPemFormat();
                    if (privateKey == null)
                    {
                        return false;
                    }

                    decryptedKey = Resolver.Device.PlatformOS.RsaDecrypt(encryptedKeyBytes, privateKey);
                }
                catch (OverflowException)
                {
                    // dev note: bug in pre-0.9.6.3 on F7 will provision with a bad key and end up here
                    // TODO: add platform and OS checking for this?
                    LogAndRaiseOnErrorOccurredEvent("RSA decrypt failure. This device likely needs to be reprovisioned.");

                    _jwt = null;
                    return false;
                }
                catch (Exception ex)
                {
                    LogAndRaiseOnErrorOccurredEvent("RSA decrypt failure", ex);

                    _jwt = null;
                    return false;
                }

                // then need to call method to AES decrypt the EncryptedToken with IV
                try
                {
                    var encryptedTokenBytes = Convert.FromBase64String(payload.EncryptedToken);
                    var ivBytes = Convert.FromBase64String(payload.Iv);
                    var decryptedToken = Resolver.Device.PlatformOS.AesDecrypt(encryptedTokenBytes, decryptedKey, ivBytes);

                    _jwt = Encoding.UTF8.GetString(decryptedToken);

                    // trim any "unprintable character" padding.  in my testing it was a 0x05, but unsure if that's consistent, so this is safer
                    // DO NOT USE REGEX!  They are horrible in Mono.
                    _jwt = SanitizeJwt(_jwt);

                    Resolver.Log.Info($"✓ Authentication completed successfully", "cloud");
                    return true;
                }
                catch (Exception ex)
                {
                    LogAndRaiseOnErrorOccurredEvent("AES decrypt failure", ex);

                    _jwt = null;
                    return false;
                }
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // device is likely not provisioned?
                errorMessage = $"Meadow.Cloud service returned 'Not Found': this device has likely not been provisioned";
                Resolver.Log.Error(errorMessage, "cloud");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                errorMessage = $"Meadow.Cloud service login returned {response.StatusCode}: {responseContent}";
                Resolver.Log.Error(errorMessage, "cloud");
            }
            else
            {
                errorMessage = $"Meadow.Cloud service login returned {response.StatusCode}";
                Resolver.Log.Error(errorMessage, "cloud");
            }

            LogAndRaiseOnErrorOccurredEvent(errorMessage);

            _jwt = null;
            return false;
        }
        catch (Exception ex)
        {
            errorMessage = $"Exception authenticating with Meadow.Cloud @{endpoint}";
            if (ex.InnerException != null)
            {
                if (ex.InnerException is IOException && ex.InnerException.HResult == -2146232800 /*0x80131620*/)
                {
                    errorMessage += $"{Environment.NewLine}Is your device clock correct? (UTC date is {DateTime.Now.ToShortDateString()})";
                }
            }
            LogAndRaiseOnErrorOccurredEvent(errorMessage, ex);

            _jwt = null;
            return false;
        }
    }

    private string SanitizeJwt(string jwt)
    {
        // DO NOT USE REGEX!  They are horrible in Mono.  The line below is a legacy reminder
        //_jwt = Regex.Replace(_jwt, @"[^\w\.@-]", ""); // this takes 4200 ms
        return
            new string(jwt.TakeWhile(c =>
            c switch
            {
                >= 'A' and <= 'Z' => true,
                >= 'a' and <= 'z' => true,
                >= '-' and <= '9' => true,
                '_' or '+' or '=' => true,
                _ => false
            }).ToArray());
    }

    private bool SendCrashReports()
    {
        var result = true;

        var reliabilityService = Resolver.Services.Get<IReliabilityService>();
        if (reliabilityService != null)
        {
            if (reliabilityService.IsCrashDataAvailable)
            {
                Resolver.Log.Info($"Sending crash data to Meadow Cloud...");

                foreach (var report in reliabilityService.GetCrashData())
                {
                    try
                    {
                        (this as IMeadowCloudService).SendLog("fatal", "device crashed", report);
                    }
                    catch (Exception ex)
                    {
                        LogAndRaiseOnErrorOccurredEvent("Unable to send crash report", ex);
                        result &= false;
                    }
                }

                reliabilityService.ClearCrashData();
            }
            else
            {
                Resolver.Log.Info($"No crash data to send to Meadow Cloud.");
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public Task SendLog(
        CloudLog log,
        CloudTelemetryPriority priority = CloudTelemetryPriority.Normal,
        bool throwIfDisabled = true)
    {
        if (!IsEnabled && throwIfDisabled)
        {
            throw new Exception("Meadow Cloud Service is not enabled");
        }

        // enqueue and trigger the timer - this will send any older data before this record
        _dataQueue.Enqueue(log, priority);
        DataForwarderProc().RethrowUnhandledExceptions();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task SendEvent(CloudEvent cloudEvent, bool throwIfDisabled = true)
    {
        if (!IsEnabled && throwIfDisabled)
        {
            throw new Exception("Meadow Cloud Service is not enabled");
        }

        // enqueue and trigger the timer - this will send any older data before this record
        _dataQueue.Enqueue(cloudEvent);
        DataForwarderProc().RethrowUnhandledExceptions();
        return Task.CompletedTask;
    }

    // MicroJson serializes the cloud payload via reflection (PropertyInfo.GetValue);
    // the trimmer can't see that usage and strips the property getters on the payload
    // types, so a published/trimmed F7 throws "Property Get method was not found" on
    // every SendEvent/SendLog. Anchor the serialized payload types (CloudEvent, CloudLog),
    // mirroring the auth-payload anchors above.
    [System.Diagnostics.CodeAnalysis.DynamicDependency(
        System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties
        | System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor,
        typeof(CloudEvent))]
    [System.Diagnostics.CodeAnalysis.DynamicDependency(
        System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties
        | System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor,
        typeof(CloudLog))]
    private async Task<bool> Send<T>(T item, string endpoint)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        if (!IsEnabled)
        {
            Resolver.Log.Warn("MeadowCloud is not enabled. Send call will not deliver data");
        }

        if (ConnectionState != CloudConnectionState.Connected)
        {
            return false;
        }

        try
        {
            if (!await _semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(60)))
            {
                Resolver.Log.Error($">>> Send() semaphore timeout after 60 seconds - possible deadlock!", "cloud");
                return false;
            }

            int attempt = 0;
            int maxRetries = 1;
            int authRetries = 0;
            int maxAuthRetries = 3;
            string errorMessage;

            var json = Resolver.JsonSerializer.Serialize(item);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

        retry:

            if (Settings.UseAuthentication)
            {
                if (_jwt == null)
                {
                    if (await Authenticate() == false)
                    {
                        authRetries++;
                        if (authRetries >= maxAuthRetries)
                        {
                            Resolver.Log.Error($"Send() failed to authenticate after {maxAuthRetries} attempts, giving up", "cloud");
                            return false;
                        }
                        Resolver.Log.Error($"Failed to authenticate with Meadow.Cloud. Retrying in {Settings.ConnectRetrySeconds} seconds... (attempt {authRetries}/{maxAuthRetries})");
                        await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                        goto retry;
                    }
                }
                _dataHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
            }

            Resolver.Log.Debug($"making cloud request to {endpoint} with payload: {json}", "cloud");

            // Wrap HTTP POST with timeout to prevent infinite hangs
            var postTask = _dataHttpClient.PostAsync($"{endpoint}", content);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(HttpClientTimeoutSeconds));
            var completedTask = await Task.WhenAny(postTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                Resolver.Log.Error($"Send() HTTP POST to {endpoint} timed out after {HttpClientTimeoutSeconds} seconds - network may be hung", "cloud");
                return false;
            }

            HttpResponseMessage response = await postTask;

            try
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized && attempt < maxRetries)
                {
                    attempt++;
                    Resolver.Log.Trace($"Send() unauthorized, invalidating auth and retry", "cloud");
                    InvalidateAuthentication();
                    _dataHttpClient.DefaultRequestHeaders.Authorization = null; // Clear old auth header
                    goto retry;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        errorMessage = $"cloud request to {endpoint} failed with {response.StatusCode}: '{responseContent}'";
                    }
                    else
                    {
                        errorMessage = $"cloud request to {endpoint} failed with {response.StatusCode}";
                    }
                    LogAndRaiseOnErrorOccurredEvent(errorMessage);
                    return false;
                }
                else
                {
                    Resolver.Log.Debug($"cloud request to {endpoint} completed successfully", messageGroup: "cloud");
                    MessageSent?.Invoke(this, EventArgs.Empty);
                    LastSuccessfulSend = DateTimeOffset.UtcNow;
                    return true;
                }
            }
            finally
            {
                response.Dispose();
            }
        }
        catch (TaskCanceledException ex)
        {
            LogAndRaiseOnErrorOccurredEvent($"Cloud request to {endpoint} timed out after {HttpClientTimeoutSeconds} seconds", ex);
            return false;
        }
        catch (Exception ex)
        {
            LogAndRaiseOnErrorOccurredEvent("Exception sending cloud message", ex);
            return false;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    /// <inheritdoc/>
    public string? GetPrivateKeyInPemFormat()
    {
        // Check the Meadow platform FIRST. F7 private keys are baked into the
        // hardware (the firmware's RsaDecrypt uses them), so we return a sentinel
        // here. This MUST precede the IsOSPlatform checks: on .NET 10 the NuttX
        // runtime reports IsOSPlatform(Linux)==true, so the F7 would otherwise take
        // the desktop Linux path (~/.ssh/id_rsa) and fail with "SSH folder not found"
        // -> null private key -> RSA decrypt skipped -> cloud auth fails. (Legacy
        // Mono did not identify NuttX as Linux, so it fell through to the F7 branch.)
        switch (Resolver.Device.Information.Platform)
        {
            case MeadowPlatform.F7FeatherV1:
            case MeadowPlatform.F7FeatherV2:
            case MeadowPlatform.F7CoreComputeV2:
                // F7 Private Keys are baked in - we have no API to extract yet
                return "F7 PRIVATE KEY";
        }

        if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var sshFolder = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh"));

            if (!sshFolder.Exists)
            {
                Resolver.Log.Error("SSH folder not found");
                return null;
            }
            else
            {
                var pkFile = Path.Combine(sshFolder.FullName, "id_rsa");
                if (!File.Exists(pkFile))
                {
                    Resolver.Log.Error("Private key not found");
                    return null;
                }

                var pkFileContent = File.ReadAllText(pkFile);
                if (!pkFileContent.Contains("BEGIN RSA PRIVATE KEY", StringComparison.OrdinalIgnoreCase))
                {
                    pkFileContent = ExecuteWindowsCommandLine("ssh-keygen", $"-e -m pem -f {pkFile}");
                }

                return pkFileContent;
            }
        }
        else if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            || SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var sshFolder = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh"));

            if (!sshFolder.Exists)
            {
                Resolver.Log.Error("SSH folder not found");
                return null;
            }
            else
            {
                var pkFile = Path.Combine(sshFolder.FullName, "id_rsa");
                if (!File.Exists(pkFile))
                {
                    Resolver.Log.Error("Private key not found");
                    return null;
                }

                var pkFileContent = File.ReadAllText(pkFile);
                if (!pkFileContent.Contains("BEGIN RSA PRIVATE KEY", StringComparison.OrdinalIgnoreCase))
                {
                    // DEV NOTE:  this is not ideal.  On the Mac, we *convert* the private key from OpenSSH to our desired format in place.
                    //            we *overwrite* the existing key file this way (we'll make a backup first).  Calling ssh-keyget always yields the
                    //            public key, even when called on the private key as input
                    File.Copy(pkFile, $"{pkFile}.bak");

                    ExecuteBashCommandLine($"ssh-keygen -p -m pem -N '' -f {pkFile}");

                    pkFileContent = File.ReadAllText(pkFile);
                }

                return pkFileContent;
            }

        }
        else
        {
            // we're on an F7 (probably)
            return Resolver.Device.Information.Platform switch
            {
                // F7 Private Keys are baked in - we have no API to extract yet
                MeadowPlatform.F7FeatherV1 => "F7 PRIVATE KEY",
                MeadowPlatform.F7FeatherV2 => "F7 PRIVATE KEY",
                MeadowPlatform.F7CoreComputeV2 => "F7 PRIVATE KEY",
                _ => throw new PlatformNotSupportedException()
            };
        }
    }

    private string ExecuteBashCommandLine(string command)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);

        process?.WaitForExit();

        return process?.StandardOutput.ReadToEnd() ?? string.Empty;
    }

    private string ExecuteWindowsCommandLine(string command, string args)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);

        process?.WaitForExit();

        return process?.StandardOutput.ReadToEnd() ?? string.Empty;
    }
}
