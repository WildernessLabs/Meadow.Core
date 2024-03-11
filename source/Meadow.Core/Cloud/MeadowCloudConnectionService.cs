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
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SRI = System.Runtime.InteropServices;

namespace Meadow;

internal class MeadowCloudConnectionService
{
    public event EventHandler<string>? ConnectionError;
    public event EventHandler<MqttApplicationMessage>? MqttMessageReceived;
    /// <summary>
    /// Retry period the service will use to attempt network reconnection
    /// </summary>
    public const int NetworkRetryTimeoutSeconds = 15;
    /// <summary>
    /// Auth token expiration period in minutes.
    /// TODO: Replace this hard-coded value with one retrieved from the Meadow Cloud.
    /// </summary>
    public const int TokenExpirationPeriod = 60;

    private bool _useAuthentication = true; // TODO: get from config
    private int _cloudConnectRetrySeconds = 15; // TODO: get from config
    private string _mqttServer = "mqtt.meadowcloud.co"; // TODO: get from config
    private int _mqttPort = 8883; // TODO: get from config
    private const int _authTimeoutSeconds = 120; // TODO: get from config
    private string _cloudAuthHostname = "https://www.meadowcloud.co";

    private List<string> _subscriptionTopics = new();
    private bool _stopService = false;
    private DateTime _lastAuthenticationTime = DateTime.MinValue;
    private string? _jwt = null;

    public CloudConnectionState State { get; private set; } = CloudConnectionState.Unknown;

    private IMqttClientOptions? ClientOptions { get; set; } = default!;
    private IMqttClient MqttClient { get; set; } = default!;

    public void AddSubscription(string topic)
    {
        // pause the state machine
        var previousState = State;
        State = CloudConnectionState.Paused;

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
                State = CloudConnectionState.Subscribing;
            }

            State = previousState;
        });
    }

    /// <summary>
    /// Method to determine if the authentication is required based on whether the time since 
    /// the last authentication exceeds the token expiration period (in minutes).
    /// </summary>
    private bool ShouldAuthenticate()
    {
        return _useAuthentication && (ClientOptions == null || (DateTime.UtcNow - _lastAuthenticationTime).TotalMinutes >= TokenExpirationPeriod);
    }

    /// <summary>
    /// Starts the service if it is not already running
    /// </summary>
    public void Start()
    {
        if (State == CloudConnectionState.Unknown)
        {
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
            State = CloudConnectionState.Subscribing;
            return Task.CompletedTask;
        });

        MqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate((f) =>
        {
            Resolver.Log.Debug("MQTT disconnected");
            State = CloudConnectionState.Disconnected;
            return Task.CompletedTask;
        });

        MqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate((f) =>
        {
            Resolver.Log.Debug($"MQTT message received at topic: {f.ApplicationMessage.Topic}");
            MqttMessageReceived?.Invoke(this, f.ApplicationMessage);
            return Task.CompletedTask;
        });
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

        State = CloudConnectionState.Disconnected;

        var nic = Resolver.Device?.NetworkAdapters?.Primary<INetworkAdapter>();

        // update state machine
        while (!_stopService)
        {
            Resolver.Log.Info($"Cloud Connection State: {State}");

            switch (State)
            {
                case CloudConnectionState.Disconnected:
                    if (ShouldAuthenticate())
                    {
                        State = CloudConnectionState.Authenticating;
                    }
                    else
                    {
                        State = CloudConnectionState.Connecting;
                    }
                    break;
                case CloudConnectionState.Authenticating:
                    try
                    {
                        if (await Authenticate())
                        {
                            // Update the last authentication time when successfully authenticated
                            _lastAuthenticationTime = DateTime.UtcNow;
                            State = CloudConnectionState.Connecting;
                        }
                        else
                        {
                            Resolver.Log.Error("Failed to authenticate with Update Service");
                            await Task.Delay(TimeSpan.FromSeconds(_cloudConnectRetrySeconds));
                        }
                    }
                    catch (Exception ae)
                    {
                        Resolver.Log.Error($"Failed to authenticate with Update Service: {ae.Message}");
                        if (ae.InnerException != null)
                        {
                            Resolver.Log.Error($" Inner Exception ({ae.InnerException.GetType().Name}): {ae.InnerException.Message}");
                        }
                        await Task.Delay(TimeSpan.FromSeconds(_cloudConnectRetrySeconds));
                    }
                    break;
                case CloudConnectionState.Connecting:
                    if (ClientOptions == null)
                    {
                        Resolver.Log.Debug("Creating MQTT client options");
                        var builder = new MqttClientOptionsBuilder()
                            .WithTcpServer(_mqttServer, _mqttPort)
                            .WithTls(tlsParameters =>
                            {
                                tlsParameters.UseTls = _mqttPort == 8883; // TODO: does this allow override?  Seems wrong
                            })
                            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
                            .WithCommunicationTimeout(TimeSpan.FromSeconds(30));

                        if (_useAuthentication)
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
                            State = CloudConnectionState.Disconnected;
                            //  just delay for a while
                            await Task.Delay(TimeSpan.FromSeconds(_cloudConnectRetrySeconds));
                        }
                        catch (MqttCommunicationException e)
                        {
                            Resolver.Log.Debug($"MQTT Error connecting to Update Service: {e.Message}");
                            State = CloudConnectionState.Disconnected;
                            //  just delay for a while
                            await Task.Delay(TimeSpan.FromSeconds(_cloudConnectRetrySeconds));
                        }
                        catch (Exception ex)
                        {
                            Resolver.Log.Error($"Error connecting to Update Service: {ex.Message}");
                            State = CloudConnectionState.Disconnected;
                            //  just delay for a while
                            await Task.Delay(TimeSpan.FromSeconds(_cloudConnectRetrySeconds));
                        }
                    }
                    else
                    {
                        Resolver.Log.Debug("Update Service waiting for network connection");
                        Thread.Sleep(TimeSpan.FromSeconds(NetworkRetryTimeoutSeconds));
                    }
                    break;
                case CloudConnectionState.Subscribing:
                    try
                    {
                        JsonWebTokenPayload? jwtPayload = null;
                        if (_useAuthentication)
                        {
                            if (string.IsNullOrWhiteSpace(_jwt))
                            {
                                throw new InvalidOperationException("Update service authentication is enabled but no JWT is available");
                            }

                            jwtPayload = GetJsonWebTokenPayload(_jwt);
                        }

                        // the config RootTopic can have multiple semicolon-delimited topics
                        //                        var topics = Config.RootTopic.Split(';', StringSplitOptions.RemoveEmptyEntries);

                        foreach (var topic in _subscriptionTopics)
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
                        State = CloudConnectionState.Connected;
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Error($"Error subscribing to Meadow.Cloud: {ex.Message}");

                        // if subscribing fails, then we need to disconnect from the server
                        await MqttClient.DisconnectAsync();

                        State = CloudConnectionState.Disconnected;
                    }
                    break;
                case CloudConnectionState.Connected:
                    Thread.Sleep(1000);
                    break;
            }
        }

        State = CloudConnectionState.Unknown;
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
        var payload = JsonSerializer.Deserialize<JsonWebTokenPayload>(payloadJson)
            ?? throw new JsonException("Payload could not be deserialized from JWT argument.");

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
        [JsonPropertyName("oid")]
        public string? OrganizationId { get; set; }
    }

    public async Task<bool> Authenticate()
    {
        string errorMessage;

        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(_authTimeoutSeconds);

            var json = JsonSerializer.Serialize<dynamic>(new { id = Resolver.Device.Information.UniqueID.ToUpper() });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = $"{_cloudAuthHostname}/api/devices/login";
            Resolver.Log.Debug($"Attempting to login to {endpoint} with {json}...");

            try
            {
                var response = await client.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Resolver.Log.Debug($"authentication successful. extracting token");
                    var opts = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    var payload = JsonSerializer.Deserialize<MeadowCloudLoginResponseMessage>(responseContent, opts);

                    if (payload == null)
                    {
                        Resolver.Log.Warn($"invalid auth payload");
                        _jwt = null;
                        return false;
                    }

                    Resolver.Log.Debug($"decrypting auth payload");
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
                        errorMessage = $"RSA decrypt failure. This device likely needs to be reprovisioned.";
                        Resolver.Log.Error(errorMessage);
                        ConnectionError?.Invoke(this, errorMessage);

                        _jwt = null;
                        return false;
                    }
                    catch (Exception ex)
                    {
                        errorMessage = $"RSA decrypt failure: {ex.Message}";
                        Resolver.Log.Error(errorMessage);
                        ConnectionError?.Invoke(this, errorMessage);

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
                        _jwt = Regex.Replace(_jwt, @"[^\w\.@-]", "");

                        Resolver.Log.Debug($"auth token successfully received");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        errorMessage = $"AES decrypt failure: {ex.Message}";
                        Resolver.Log.Error(errorMessage);
                        ConnectionError?.Invoke(this, errorMessage);

                        _jwt = null;
                        return false;
                    }
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // device is likely not provisioned?
                    errorMessage = $"Update service returned 'Not Found': this device has likely not been provisioned";
                }
                else
                {
                    errorMessage = $"Update service login returned {response.StatusCode}: {responseContent}";
                }

                Resolver.Log.Warn(errorMessage);
                ConnectionError?.Invoke(this, errorMessage);

                _jwt = null;
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Exception authenticating with Meadow.Cloud: {ex.Message}";
                Resolver.Log.Warn(errorMessage);
                ConnectionError?.Invoke(this, errorMessage);

                _jwt = null;
                return false;
            }
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
        else if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
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
        else if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            throw new PlatformNotSupportedException();
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
