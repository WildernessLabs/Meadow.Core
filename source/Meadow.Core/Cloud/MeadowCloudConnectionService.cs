﻿using Meadow.Cloud;
using Meadow.Hardware;
using Meadow.Update;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
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

internal class MeadowCloudConnectionService : IMeadowCloudService
{
    /// <inheritdoc/>
    public event EventHandler<Exception>? ErrorOccurred;

    /// <inheritdoc/>
    public event EventHandler<CloudConnectionState>? ConnectionStateChanged;

    internal event EventHandler<MqttApplicationMessage>? MqttMessageReceived;

    /// <summary>
    /// Retry period the service will use to attempt network reconnection
    /// </summary>
    public const int NetworkRetryTimeoutSeconds = 15;
    /// <summary>
    /// Auth token expiration period in minutes.
    /// TODO: Replace this hard-coded value with one retrieved from the Meadow Cloud.
    /// </summary>
    public const int TokenExpirationPeriod = 60;

    internal IMeadowCloudSettings Settings { get; private set; }

    private List<string> _subscriptionTopics = new();
    private bool _stopService = true;
    private bool _firstConection = true;
    private DateTime _lastAuthenticationTime = DateTime.MinValue;
    private string? _jwt = null;
    private CloudConnectionState _connectionState = CloudConnectionState.Unknown;
    private Thread? _stateMachineThread;
    private static readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private readonly CloudDataQueue _dataQueue;
    private AutoResetEvent _dataReadyEvent = new(false);

    private IMqttClientOptions? ClientOptions { get; set; } = default!;
    private IMqttClient MqttClient { get; set; } = default!;

    /// <inheritdoc/>
    public bool IsEnabled { get; private set; }

    internal MeadowCloudConnectionService(IMeadowCloudSettings settings)
    {
        Settings = settings;
        _dataQueue = new CloudDataQueue();
        _ = Task.Run(DataForwarderProc);
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
        while (true)
        {
            _dataReadyEvent.WaitOne(TimeSpan.FromSeconds(30));

            while (_dataQueue.Count > 0)
            {
                try
                {
                    if (ConnectionState == CloudConnectionState.Connected)
                    {
                        // get the head item
                        var info = _dataQueue.Peek();
                        if (info != null)
                        {
                            // failed to send, leave the item in the queue
                            if (await Send(info.Item, info.EndPoint))
                            {
                                // item was sent, remove from the queue and toss away
                                _dataQueue.Dequeue();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogAndRaiseOnErrorOccurredEvent("Unable to forward Meadow Cloud record", ex);
                    break;
                }

                // throttle bulk sends
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
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

        if (ex != null && ex.HResult == -76)
        {
            ReportFatalErrorToReliabilityService(raisedException);
        }

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
        if (_stateMachineThread == null)
        {
            _stateMachineThread = new Thread(() => ConnectionStateMachine());
            _stateMachineThread.Start();

            IsEnabled = true;
        }
    }

    /// <inheritdoc/>
    public void Stop()
    {
        _stopService = true;
        IsEnabled = false;
    }

    private void Initialize()
    {
        var factory = new MqttFactory();
        MqttClient = factory.CreateMqttClient();

        MqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate((f) =>
        {
            Resolver.Log.Debug("MQTT connected", "cloud");
            ConnectionState = CloudConnectionState.Subscribing;
            return Task.CompletedTask;
        });

        MqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate((f) =>
        {
            Resolver.Log.Debug("MQTT disconnected", "cloud");
            ConnectionState = CloudConnectionState.Disconnected;
            return Task.CompletedTask;
        });

        MqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate((f) =>
        {
            Resolver.Log.Debug($"MQTT message received at topic: {f.ApplicationMessage.Topic}", "cloud");
            MqttMessageReceived?.Invoke(this, f.ApplicationMessage);
            return Task.CompletedTask;
        });
    }

    private async void ConnectionStateMachine()
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

        ConnectionState = CloudConnectionState.Disconnected;

        var nic = Resolver.Device?.NetworkAdapters?.Primary<INetworkAdapter>();

        if (nic == null)
        {
            Resolver.Log.Error($"Meadow.Cloud service detected no network interface!");
            return;
        }
        else
        {
            Resolver.Log.Debug($"Meadow.Cloud service will use the {nic.GetType().Name} network interface");
        }

        nic.NetworkDisconnected += (s, e) =>
        {
            Resolver.Log.Debug($"Meadow.Cloud detected network disconnect");
            ConnectionState = CloudConnectionState.Disconnected;
        };
        nic.NetworkConnected += (s, e) =>
        {
            Resolver.Log.Debug($"Meadow.Cloud detected network connect");
        };

        var stopwatch = new Stopwatch();

        // update state machine
        while (!_stopService)
        {
            switch (ConnectionState)
            {
                case CloudConnectionState.Disconnected:
                    if (ShouldAuthenticate())
                    {
                        ConnectionState = CloudConnectionState.Authenticating;
                    }
                    else
                    {
                        ConnectionState = CloudConnectionState.Connecting;
                    }
                    break;
                case CloudConnectionState.Authenticating:
                    try
                    {
                        if (nic != null && nic.IsConnected)
                        {
                            stopwatch.Restart();

                            if (await Authenticate())
                            {
                                Resolver.Log.Debug($"Authentication took {stopwatch.ElapsedMilliseconds:N} ms");
                                stopwatch.Stop();
                                // Update the last authentication time when successfully authenticated
                                _lastAuthenticationTime = DateTime.UtcNow;
                                ConnectionState = CloudConnectionState.Connecting;
                            }
                            else
                            {
                                Resolver.Log.Error("Failed to authenticate with Meadow.Cloud");
                                await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                            }
                        }
                        else
                        {
                            if (!stopwatch.IsRunning) stopwatch.Restart();

                            Resolver.Log.Debug($"Meadow.Cloud service waiting for network connection ({stopwatch.Elapsed.TotalSeconds} s)", "cloud");
                            Thread.Sleep(TimeSpan.FromSeconds(NetworkRetryTimeoutSeconds));
                        }
                    }
                    catch (Exception ae)
                    {
                        Resolver.Log.Error($"Failed to authenticate with Meadow.Cloud: {ae.Message}");
                        if (ae.InnerException != null)
                        {
                            Resolver.Log.Error($"Inner Exception ({ae.InnerException.GetType().Name}): {ae.InnerException.Message}");
                        }
                        await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                    }
                    break;
                case CloudConnectionState.Connecting:
                    if (ClientOptions == null)
                    {
                        Resolver.Log.Debug("Creating MQTT client options", "cloud");
                        var builder = new MqttClientOptionsBuilder()
                            .WithTcpServer(Settings.MqttHostname, Settings.MqttPort)
                            .WithTls(tlsParameters =>
                            {
                                tlsParameters.UseTls = Settings.MqttPort == 8883;
                            })
                            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
                            .WithCommunicationTimeout(TimeSpan.FromSeconds(30));

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
                            await MqttClient.ConnectAsync(ClientOptions);
                        }
                        catch (MqttCommunicationTimedOutException)
                        {
                            Resolver.Log.Debug("Timeout connecting to Meadow.Cloud", "cloud");
                            ConnectionState = CloudConnectionState.Disconnected;
                            //  just delay for a while
                            await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                        }
                        catch (MqttCommunicationException e)
                        {
                            Resolver.Log.Debug($"MQTT Error connecting to Meadow.Cloud: {e.Message}", "cloud");
                            ConnectionState = CloudConnectionState.Disconnected;
                            //  just delay for a while
                            await Task.Delay(TimeSpan.FromSeconds(Settings.ConnectRetrySeconds));
                        }
                        catch (Exception ex)
                        {
                            Resolver.Log.Error($"Error connecting to Meadow.Cloud: {ex.Message}");
                            if (ex.InnerException != null)
                            {
                                Resolver.Log.Error($"Inner Exception ({ex.InnerException.GetType().Name}): {ex.InnerException.Message}");
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
                        Thread.Sleep(TimeSpan.FromSeconds(NetworkRetryTimeoutSeconds));
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

                        // the config RootTopic can have multiple semicolon-delimited topics
                        //                        var topics = Config.RootTopic.Split(';', StringSplitOptions.RemoveEmptyEntries);

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
                            await MqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topicName).Build());
                        }
                        ConnectionState = CloudConnectionState.Connected;
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Error($"Error subscribing to Meadow.Cloud: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Resolver.Log.Error($"Inner Exception ({ex.InnerException.GetType().Name}): {ex.InnerException.Message}");
                        }
                        // if subscribing fails, then we need to disconnect from the server
                        await MqttClient.DisconnectAsync();

                        ConnectionState = CloudConnectionState.Disconnected;
                    }
                    break;
                case CloudConnectionState.Connected:
                    if (_firstConection)
                    {
                        if (SendCrashReports())
                        {
                            _firstConection = false;
                        }
                    }

                    Thread.Sleep(1000);
                    break;
            }
        }

        ConnectionState = CloudConnectionState.Unknown;
        _stateMachineThread = null;
    }

    internal AuthenticationHeaderValue CreateAuthenticationHeaderValue()
    {
        return new AuthenticationHeaderValue("Bearer", _jwt);
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
    public async Task<bool> Authenticate()
    {
        string errorMessage;

        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(Settings.AuthTimeoutSeconds);

            var json = Resolver.JsonSerializer.Serialize(new JsonIdPayload(Resolver.Device.Information.UniqueID.ToUpper()));
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = $"{Settings.AuthHostname}/api/devices/login";
            Resolver.Log.Debug($"Attempting to login to {endpoint} with {json}...", "cloud");

            try
            {
                using var response = await client.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Resolver.Log.Debug($"authentication successful. extracting token", "cloud");

                    var payload = Resolver.JsonSerializer.Deserialize<MeadowCloudLoginResponseMessage>(responseContent);

                    if (payload == null)
                    {
                        Resolver.Log.Warn($"invalid auth payload");
                        _jwt = null;
                        return false;
                    }

                    Resolver.Log.Debug($"decrypting auth payload", "cloud");
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

                        Resolver.Log.Debug($"auth token successfully received", "cloud");
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
                    Resolver.Log.Warn(errorMessage);
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    errorMessage = $"Meadow.Cloud service login returned {response.StatusCode}: {responseContent}";
                }
                else
                {
                    errorMessage = $"Meadow.Cloud service login returned {response.StatusCode}";
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
        }

        return result;
    }

    /// <inheritdoc/>
    public Task SendLog(CloudLog log)
    {
        // enqueue and trigger the timer - this will send any older data before this record
        _dataQueue.Enqueue(log, "/api/logs");
        _dataReadyEvent.Set();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task SendEvent(CloudEvent cloudEvent)
    {
        // enqueue and trigger the timer - this will send any older data before this record
        _dataQueue.Enqueue(cloudEvent, "/api/events");
        _dataReadyEvent.Set();
        return Task.CompletedTask;
    }

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

        HttpClient client = new HttpClient();

        try
        {
            await _semaphoreSlim.WaitAsync();

            int attempt = 0;
            int maxRetries = 1;
            string errorMessage;

            var json = Resolver.JsonSerializer.Serialize(item);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

        retry:
            client.BaseAddress = new Uri(Settings.DataHostname);

            if (Settings.UseAuthentication)
            {
                if (_jwt == null)
                {
                    await Authenticate();
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
            }

            Resolver.Log.Debug($"making cloud request to {endpoint} with payload: {json}", "cloud");

            HttpResponseMessage response = await client.PostAsync($"{endpoint}", content);

            try
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized && attempt < maxRetries)
                {
                    attempt++;
                    // by setting this to null and retrying, Authenticate will get called
                    ClientOptions = null;
                    _jwt = null;
                    client.Dispose();
                    client = new HttpClient();
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
                    return true;
                }
            }
            finally
            {
                response.Dispose();
            }
        }
        catch (Exception ex)
        {
            LogAndRaiseOnErrorOccurredEvent("Exception sending cloud message", ex);
            return false;
        }
        finally
        {
            client.Dispose();
            _semaphoreSlim.Release();
        }
    }

    /// <inheritdoc/>
    public string? GetPrivateKeyInPemFormat()
    {
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
