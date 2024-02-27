using Meadow.Cloud;
using Meadow.Hardware;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Meadow.Update")]

namespace Meadow.Update;

/// <summary>
/// The default Meadow implementation of IUpdateService
/// </summary>
public class UpdateService : IUpdateService, ICommandService
{
    /// <summary>
    /// Retry period the service will use to attempt network reconnection
    /// </summary>
    public const int NetworkRetryTimeoutSeconds = 15;

    /// <summary>
    /// Specifies the maximum number of download attempts before giving up.
    /// </summary>
    public const int MaxDownloadRetries = 10;

    /// <summary>
    /// Period the service will wait between download attempts in 
    /// case of failure.
    /// </summary>
    public const int RetryDelayMilliseconds = 1000;

    /// <summary>
    /// Auth token expiration period in minutes.
    /// TODO: Replace this hard-coded value with one retrieved from the Meadow Cloud.
    /// </summary>
    public const int TokenExpirationPeriod = 60;

    private const string DefaultUpdateStoreDirectoryName = "update-store";
    private const string DefaultUpdateDirectoryName = "update";

    private string UpdateDirectory { get; }
    private string UpdateStoreDirectory { get; }

    /// <inheritdoc/>
    public event EventHandler<UpdateState> OnStateChanged = default!;
    /// <inheritdoc/>
    public event UpdateEventHandler OnUpdateAvailable = default!;
    /// <inheritdoc/>
    public event UpdateEventHandler OnUpdateProgress = default!;
    /// <inheritdoc/>
    public event UpdateEventHandler OnUpdateRetrieved = default!;
    /// <inheritdoc/>
    public event UpdateEventHandler OnUpdateSuccess = default!;
    /// <inheritdoc/>
    public event UpdateEventHandler OnUpdateFailure = default!;

    private UpdateState _state;
    private bool _stopService = false;
    private bool _downloadInProgress = false;
    private string? _jwt = null;
    private DateTime _lastAuthenticationTime = DateTime.MinValue;

    private IUpdateSettings Config { get; }
    private IMqttClient MqttClient { get; set; } = default!;
    private IMqttClientOptions? ClientOptions { get; set; } = default!;
    private UpdateStore Store { get; }

    internal UpdateService(string fsRoot, IUpdateSettings config)
    {
        UpdateStoreDirectory = Path.Combine(fsRoot, DefaultUpdateStoreDirectoryName);
        UpdateDirectory = Path.Combine(fsRoot, DefaultUpdateDirectoryName);

        Store = new UpdateStore(UpdateStoreDirectory);

        Config = config;
    }

    /// <inheritdoc/>
    public void Shutdown()
    {
        _stopService = true;
    }

    /// <inheritdoc/>
    public UpdateState State
    {
        get => _state;
        private set
        {
            if (value == State) return;

            _state = value;
            OnStateChanged?.Invoke(this, State);
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

    private void Initialize()
    {
        var factory = new MqttFactory();
        MqttClient = factory.CreateMqttClient();

        MqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate((f) =>
        {
            Resolver.Log.Debug("MQTT connected");
            State = UpdateState.Connected;
            return Task.CompletedTask;
        });

        MqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate((f) =>
        {
            Resolver.Log.Debug("MQTT disconnected");
            State = UpdateState.Disconnected;
            return Task.CompletedTask;
        });

        MqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate((f) =>
        {
            Resolver.Log.Debug("MQTT message received");

            if (f.ApplicationMessage.Topic.EndsWith($"/commands/{Resolver.Device.Information.UniqueID}", StringComparison.OrdinalIgnoreCase))
            {
                Resolver.Log.Trace("Meadow command received");
                ProcessPublishedCommand(f.ApplicationMessage);
                return Task.CompletedTask;
            }

            var json = Encoding.UTF8.GetString(f.ApplicationMessage.Payload);

            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var info = JsonSerializer.Deserialize<UpdateMessage>(json, opts);

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

                        OnUpdateAvailable?.Invoke(this, info);
                    }
                    else
                    {
                        Resolver.Log.Trace($"Update {info.ID} has already been retrieved");
                    }
                }
                else
                {
                    Store.Add(info);
                    OnUpdateAvailable?.Invoke(this, info);
                }

            }

            return Task.CompletedTask;
        });
    }

    private async Task<bool> AuthenticateWithServer()
    {
        var meadowCloudService = Resolver.MeadowCloudService;

        if (await meadowCloudService.Authenticate())
        {
            _jwt = meadowCloudService.CurrentJwt;
        }

        return _jwt != null;
    }

    /// <summary>
    /// Method to determine if the authentication is required based on whether the time since 
    /// the last authentication exceeds the token expiration period (in minutes).
    /// </summary>
    private bool ShouldAuthenticate()
    {
        return (DateTime.UtcNow - _lastAuthenticationTime).TotalMinutes >= TokenExpirationPeriod;
    }

    private async void UpdateStateMachine()
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

        Initialize();

        State = UpdateState.Disconnected;

        var nic = Resolver.Device?.NetworkAdapters?.Primary<INetworkAdapter>();

        // update state machine
        while (!_stopService)
        {
            switch (State)
            {
                case UpdateState.Disconnected:
                    if (Config.UseAuthentication && ShouldAuthenticate())
                    {
                        State = UpdateState.Authenticating;
                    }
                    else
                    {
                        State = UpdateState.Connecting;
                    }
                    break;
                case UpdateState.Authenticating:
                    try
                    {
                        if (await AuthenticateWithServer())
                        {
                            // Update the last authentication time when successfully authenticated
                            _lastAuthenticationTime = DateTime.UtcNow;
                            State = UpdateState.Connecting;
                        }
                        else
                        {
                            Resolver.Log.Error("Failed to authenticate with Update Service");
                            await Task.Delay(TimeSpan.FromSeconds(Config.CloudConnectRetrySeconds));
                        }
                    }
                    catch (Exception ae)
                    {
                        Resolver.Log.Error($"Failed to authenticate with Update Service: {ae.Message}");
                        if (ae.InnerException != null)
                        {
                            Resolver.Log.Error($" Inner Exception ({ae.InnerException.GetType().Name}): {ae.InnerException.Message}");
                        }
                        await Task.Delay(TimeSpan.FromSeconds(Config.CloudConnectRetrySeconds));
                    }
                    break;
                case UpdateState.Connecting:
                    if (ClientOptions == null)
                    {
                        Resolver.Log.Debug("Creating MQTT client options");
                        var builder = new MqttClientOptionsBuilder()
                            .WithTcpServer(Config.UpdateServer, Config.UpdatePort)
                            .WithTls(tlsParameters =>
                            {
                                tlsParameters.UseTls = Config.UpdatePort == 8883;
                            })
                            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
                            .WithCommunicationTimeout(TimeSpan.FromSeconds(30));

                        if (Config.UseAuthentication)
                        {
                            Resolver.Log.Debug("Adding MQTT creds");
                            builder.WithCredentials(Resolver.Device?.Information.UniqueID.ToUpper(), _jwt);
                        }

                        ClientOptions = builder.Build();
                    }

                    if (nic != null && nic.IsConnected)
                    {
                        try
                        {
                            Resolver.Log.Debug("Connecting MQTT client");
                            await MqttClient.ConnectAsync(ClientOptions);
                        }
                        catch (MqttCommunicationTimedOutException)
                        {
                            Resolver.Log.Debug("Timeout connecting to Update Service");
                            State = UpdateState.Disconnected;
                            //  just delay for a while
                            await Task.Delay(TimeSpan.FromSeconds(Config.CloudConnectRetrySeconds));
                        }
                        catch (MqttCommunicationException e)
                        {
                            Resolver.Log.Debug($"MQTT Error connecting to Update Service: {e.Message}");
                            State = UpdateState.Disconnected;
                            //  just delay for a while
                            await Task.Delay(TimeSpan.FromSeconds(Config.CloudConnectRetrySeconds));
                        }
                        catch (Exception ex)
                        {
                            Resolver.Log.Error($"Error connecting to Update Service: {ex.Message}");
                            State = UpdateState.Disconnected;
                            //  just delay for a while
                            await Task.Delay(TimeSpan.FromSeconds(Config.CloudConnectRetrySeconds));
                        }
                    }
                    else
                    {
                        Resolver.Log.Debug("Update Service waiting for network connection");
                        Thread.Sleep(TimeSpan.FromSeconds(NetworkRetryTimeoutSeconds));
                    }
                    break;
                case UpdateState.Connected:
                    if (!_downloadInProgress)
                    {
                        State = UpdateState.Idle;
                        try
                        {
                            JsonWebTokenPayload? jwtPayload = null;
                            if (Config.UseAuthentication)
                            {
                                if (string.IsNullOrWhiteSpace(_jwt))
                                {
                                    throw new InvalidOperationException("Update service authentication is enabled but no JWT is available");
                                }

                                jwtPayload = GetJsonWebTokenPayload(_jwt);
                            }

                            // the config RootTopic can have multiple semicolon-delimited topics
                            var topics = Config.RootTopic.Split(';', StringSplitOptions.RemoveEmptyEntries);

                            foreach (var topic in topics)
                            {
                                string topicName = topic;

                                // look for macro-substitutions
                                if (topic.Contains("{OID}") && !string.IsNullOrWhiteSpace(jwtPayload?.OrganizationId))
                                {
                                    topicName = topicName.Replace("{OID}", jwtPayload.OrganizationId);
                                }

                                if (topic.Contains("{ID}"))
                                {
                                    topicName = topicName.Replace("{ID}", Resolver.Device?.Information.UniqueID.ToUpper());
                                }

                                Resolver.Log.Debug($"Update service subscribing to '{topicName}'");
                                await MqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topicName).Build());
                            }
                        }
                        catch (Exception ex)
                        {
                            Resolver.Log.Error($"Error subscribing to Meadow.Cloud: {ex.Message}");

                            // if subscribing fails, then we need to disconnect from the server
                            await MqttClient.DisconnectAsync();

                            State = UpdateState.Disconnected;
                        }
                    }
                    else
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
            if (Config.UseAuthentication)
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
                    if (Config.UseAuthentication)
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
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

                                OnUpdateProgress?.Invoke(this, message);
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

                OnUpdateRetrieved?.Invoke(this, message);
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

    // In order to not require the use of a library, this was taken from
    // RFC7515, Appendix C: https://datatracker.ietf.org/doc/html/rfc7515#appendix-C
    private string base64UrlDecode(string arg)
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

        var payloadJson = base64UrlDecode(tokenParts[1]);
        var payload = JsonSerializer.Deserialize<JsonWebTokenPayload>(payloadJson)
            ?? throw new JsonException("Payload could not be deserialized from JWT argument.");

        return payload;
    }

    private class JsonWebTokenPayload
    {
        [JsonPropertyName("oid")]
        public string? OrganizationId { get; set; }
    }

    /// <inheritdoc/>
    public void ApplyUpdate(UpdateInfo updateInfo)
    {
        State = UpdateState.UpdateInProgress;

        var sourcePath = Store.GetUpdateArchivePath(updateInfo.ID);

        Resolver.Log.Debug($"Applying update from '{sourcePath}'");

        if (sourcePath == null)
        {
            OnUpdateFailure?.Invoke(this, updateInfo);
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
            OnUpdateFailure?.Invoke(this, updateInfo);
            throw ex;
        }

        // do we actually contain an update?
        var updateValid = CurrentUpdateContainsAppUpdate() || CurrentUpdateContainsOSUpdate();

        // TODO: should we verify paths, etc?

        if (!updateValid)
        {
            Resolver.Log.Warn($"Update {updateInfo.ID} contains no valid Update data");
            OnUpdateFailure?.Invoke(this, updateInfo);
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
        OnUpdateSuccess?.Invoke(this, updateInfo);

        State = UpdateState.Idle;
    }

    private readonly ConcurrentDictionary<string, (Type CommandType, Action<object> Action)> CommandSubscriptions = new();
    private const string UntypedCommandTypeName = "<<<MEADOWCOMMAND>>>";

    void ICommandService.Subscribe(Action<MeadowCommand> action)
    {
        CommandSubscriptions[UntypedCommandTypeName] = (CommandType: typeof(MeadowCommand), Action: x => action((MeadowCommand)x));
    }

    void ICommandService.Subscribe<T>(Action<T> action)
    {
        var commandTypeName = typeof(T).Name;
        CommandSubscriptions[commandTypeName.ToUpperInvariant()] = (CommandType: typeof(T), Action: x => action((T)x));
    }

    void ICommandService.Unsubscribe()
    {
        CommandSubscriptions.TryRemove(UntypedCommandTypeName, out _);
    }

    void ICommandService.Unsubscribe<T>()
    {
        var commandTypeName = typeof(T).Name;
        CommandSubscriptions.TryRemove(commandTypeName.ToUpperInvariant(), out _);
    }

    internal void ProcessPublishedCommand(MqttApplicationMessage message)
    {
        if (message.UserProperties == null)
        {
            Resolver.Log.Error("Unable to process published command without a command name.");
            return;
        }

        var properties = message.UserProperties.ToDictionary(x => x.Name.ToUpperInvariant(), x => x.Value);

        if (!properties.TryGetValue("COMMANDNAME", out string commandName) ||
            string.IsNullOrWhiteSpace(commandName))
        {
            Resolver.Log.Error("Unable to process published command without a command name.");
            return;
        }

        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        // First attempt to run the untyped command subscription, Action<MeadowCommand>, if available.
        if (CommandSubscriptions.TryGetValue(UntypedCommandTypeName, out (Type CommandType, Action<object> Action) value))
        {
            Resolver.Log.Trace($"Processing generic Meadow command with command name '{commandName}'...");

            IReadOnlyDictionary<string, object>? arguments;
            try
            {
                arguments = message.Payload != null
                    ? JsonSerializer.Deserialize<IReadOnlyDictionary<string, object>>(message.Payload, jsonSerializerOptions)
                    : null;
            }
            catch (JsonException ex)
            {
                Resolver.Log.Error($"Unable to deserialize command arguments: {ex.Message}");
                return;
            }

            var command = new MeadowCommand(commandName, arguments);
            value.Action(command);
        }

        // Then attempt to run the typed command subscription, Action<T> where T : ICommand, new(),
        // if available. Also prevent user from running the untyped command subscription.
        if (CommandSubscriptions.TryGetValue(commandName.ToUpperInvariant(), out value))
        {
            Resolver.Log.Trace($"Processing Meadow command of type '{value.CommandType.Name}'...");

            object command;
            try
            {
                command = message.Payload != null
                    ? JsonSerializer.Deserialize(message.Payload, value.CommandType, jsonSerializerOptions) ?? Activator.CreateInstance(value.CommandType)
                    : Activator.CreateInstance(value.CommandType);
            }
            catch (JsonException ex)
            {
                Resolver.Log.Error($"Unable to deserialize command arguments: {ex.Message}");
                return;
            }

            value.Action(command);
        }
    }
}