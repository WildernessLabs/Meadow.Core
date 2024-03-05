using Meadow.Hardware;
using Meadow.Update;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

internal class MeadowCloudConnectionService
{
    /// <summary>
    /// Retry period the service will use to attempt network reconnection
    /// </summary>
    public const int NetworkRetryTimeoutSeconds = 15;

    private bool _stopService = false;
    public CloudConnectionState State { get; private set; } = CloudConnectionState.Unknown;

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
            switch (State)
            {
                case CloudConnectionState.Disconnected:
                    if (Config.UseAuthentication && ShouldAuthenticate())
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
                        if (await AuthenticateWithServer())
                        {
                            // Update the last authentication time when successfully authenticated
                            _lastAuthenticationTime = DateTime.UtcNow;
                            State = CloudConnectionState.Connecting;
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
                case CloudConnectionState.Connecting:
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
                            State = CloudConnectionState.Disconnected;
                            //  just delay for a while
                            await Task.Delay(TimeSpan.FromSeconds(Config.CloudConnectRetrySeconds));
                        }
                        catch (MqttCommunicationException e)
                        {
                            Resolver.Log.Debug($"MQTT Error connecting to Update Service: {e.Message}");
                            State = CloudConnectionState.Disconnected;
                            //  just delay for a while
                            await Task.Delay(TimeSpan.FromSeconds(Config.CloudConnectRetrySeconds));
                        }
                        catch (Exception ex)
                        {
                            Resolver.Log.Error($"Error connecting to Update Service: {ex.Message}");
                            State = CloudConnectionState.Disconnected;
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
                case CloudConnectionState.Connected:
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

                            State = CloudConnectionState.Disconnected;
                        }
                    }
                    else
                    {
                        Resolver.Log.Debug($"Resuming download after reconnection...");
                        State = UpdateState.DownloadingFile;
                    }
                    break;
                default:
                    Thread.Sleep(1000);
                    break;
            }
        }

        State = CloudConnectionState.Unknown;
    }

}
