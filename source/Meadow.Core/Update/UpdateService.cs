using Meadow.Hardware;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Exceptions;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Meadow.Update")]

namespace Meadow.Update
{
    public class UpdateService : IUpdateService
    {
        public const int NetworkRetryTimeoutSeconds = 15;

        private string UpdateDirectory { get; }
        private string UpdateStoreDirectory { get; }

        public event UpdateEventHandler OnUpdateAvailable = delegate { };
        public event UpdateEventHandler OnUpdateRetrieved = delegate { };
        public event UpdateEventHandler OnUpdateSuccess = delegate { };
        public event UpdateEventHandler OnUpdateFailure = delegate { };

        private UpdateState _state;

        private IUpdateSettings Config { get; }
        private IMqttClient MqttClient { get; set; } = default!;
        private IMqttClientOptions ClientOptions { get; set; } = default!;
        private UpdateStore Store { get; }
        private string MqttClientID { get; }

        internal UpdateService(string fsRoot, IUpdateSettings config)
        {
            UpdateStoreDirectory = Path.Combine(fsRoot, "update-store");
            UpdateDirectory = Path.Combine(fsRoot, "update");

            Store = new UpdateStore(UpdateStoreDirectory);
            var id = Resolver.Device?.Information?.UniqueID ?? "simple";
            MqttClientID = $"{id}_client";

            Config = config;
        }

        public UpdateState State
        {
            get => _state;
            private set
            {
                if (value == State) return;

                _state = value;
                Resolver.Log.Trace($"Updater State -> {State}");
            }
        }

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
                State = UpdateState.Connected;
            });

            MqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate((f) =>
            {
                State = UpdateState.Disconnected;
            });

            MqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate((f) =>
            {
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
            });

            ClientOptions = new MqttClientOptionsBuilder()
                            .WithClientId(MqttClientID)
                            .WithTcpServer(Config.UpdateServer, Config.UpdatePort)
                            //.WithCleanSession()
                            .Build();
        }

        private async void UpdateStateMachine()
        {
            Thread.Sleep(TimeSpan.FromSeconds(NetworkRetryTimeoutSeconds));

            Initialize();

            State = UpdateState.Disconnected;

            var nic = Resolver.Device?.NetworkAdapters?.Primary<INetworkAdapter>();

            // update state machine
            while (true)
            {
                switch (State)
                {
                    case UpdateState.Disconnected:
                        if (nic.IsConnected)
                        {
                            State = UpdateState.Connecting;
                            try
                            {
                                await MqttClient.ConnectAsync(ClientOptions);
                            }
                            catch (MqttCommunicationTimedOutException)
                            {
                                Resolver.Log.Debug("Timeout connecting to Meadow.Cloud");
                                State = UpdateState.Disconnected;
                                //  just delay for a while
                                await Task.Delay(TimeSpan.FromSeconds(Config.CloudConnectRetrySeconds));
                            }
                            catch (MqttCommunicationException e)
                            {
                                Resolver.Log.Debug($"Error connecting to Meadow.Cloud: {e.Message}");
                                State = UpdateState.Disconnected;
                                //  just delay for a while
                                await Task.Delay(TimeSpan.FromSeconds(Config.CloudConnectRetrySeconds));
                            }
                            catch (Exception ex)
                            {
                                Resolver.Log.Error($"Error connecting to Meadow.Cloud: {ex.Message}");
                                State = UpdateState.Disconnected;
                            }
                        }
                        else
                        {
                            Resolver.Log.Debug("Update Service waiting for network connection");
                            Thread.Sleep(TimeSpan.FromSeconds(NetworkRetryTimeoutSeconds));
                        }
                        break;
                    case UpdateState.Connected:
                        State = UpdateState.Idle;
                        try
                        {
                            await MqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(Config.RootTopic).Build());
                        }
                        catch (Exception ex)
                        {
                            Resolver.Log.Error($"Error subscribing to Meadow.Cloud: {ex.Message}");
                            // TODO: what should be the state here?  What do we do (untested failure mode)
                            State = UpdateState.Disconnected;
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
        }

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
            try
            {
                // Note: this is infrequently called, so we don't want to follow the advice of "use one instance for all calls"
                using (var httpClient = new HttpClient())
                {
                    using (var stream = await httpClient.GetStreamAsync(message.MpakDownloadUrl))
                    {
                        using (var fileStream = Store.GetUpdateFileStream(message.ID))
                        {
                            Resolver.Log.Trace($"Copying update to {fileStream.Name}");

                            await stream.CopyToAsync(fileStream);
                        }
                    }
                }

                var path = Store.GetUpdateArchivePath(message.ID);
                var hash = Store.GetFileHash(new FileInfo(path));

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

                OnUpdateRetrieved(this, message);
                Store.SetRetrieved(message);

                State = UpdateState.Idle;
            }
            catch (Exception ex)
            {
                // TODO: raise some event?
                Resolver.Log.Error($"Failed to download Update: {ex.Message}");
                State = UpdateState.Idle;
            }
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
            Resolver.Log.Info(di.Name);

            foreach (var f in di.GetFiles())
            {
                Resolver.Log.Info($" {f.Name}");
            }

            foreach (var d in di.GetDirectories())
            {
                DisplayTree(d);
            }
        }

        public void ApplyUpdate(UpdateInfo updateInfo)
        {
            State = UpdateState.UpdateInProgress;

            var sourcePath = Store.GetUpdateArchivePath(updateInfo.ID);

            if (sourcePath == null)
            {
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

            // extract zip
            ZipFile.ExtractToDirectory(sourcePath, UpdateDirectory);

            DisplayTree(new DirectoryInfo(UpdateDirectory));

            // do we actually contain an update?
            var updateValid = CurrentUpdateContainsAppUpdate() || CurrentUpdateContainsOSUpdate();

            // TODO: should we verify paths, etc?

            if (!updateValid)
            {
                Resolver.Log.Warn($"Update {updateInfo.ID} contains no valid Update data");
                return;
            }

            // shut down the app (or timeout)
            var shutdownTimeoutTask = Task.Delay(TimeSpan.FromSeconds(5));
            var shutDownTask = Resolver.App?.OnShutdown();

            // cancel the app run cancellation token
            MeadowOS.AppAbort.Cancel();

            // wait for the app to return, or the timeout
            Task.WaitAny(shutDownTask, shutdownTimeoutTask);

            // restart the device
            Resolver.Device.PlatformOS.Reset();

            // the OS will handle updates on the next boot

            // TODO: these will never happen due to the above reset. need to be moved to "post update boot"
            Store.SetApplied(updateInfo);
            OnUpdateSuccess(this, updateInfo);

            State = UpdateState.Idle;
        }
    }
}