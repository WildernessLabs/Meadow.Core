using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Meadow.Update")]

namespace Meadow.Update
{
    public class UpdateService : IUpdateService
    {
        private string UpdateDirectory { get; }
        private string UpdateStoreDirectory { get; }

        public event UpdateEventHandler OnUpdateAvailable = delegate { };
        public event UpdateEventHandler OnUpdateRetrieved = delegate { };
        public event UpdateEventHandler OnUpdateSuccess = delegate { };
        public event UpdateEventHandler OnUpdateFailure = delegate { };

        private UpdateState _state;

        private IUpdateSettings Config { get; }
        private IMqttClient MqttClient { get; set; }
        private MqttClientOptions ClientOptions { get; set; }
        private UpdateStore Store { get; }

        internal UpdateService(IUpdateSettings config)
        {
            string root;

            // NOTE: this is a temporary path for desktop testing right now
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                root = Path.Combine(Path.GetTempPath(), "meadow");
            }
            else
            {
                root = "/meadow0";
            }

            UpdateStoreDirectory = Path.Combine(root, "update-store");
            UpdateDirectory = Path.Combine(root, "update");

            Store = new UpdateStore(UpdateStoreDirectory);

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
                Initialize();
                Task.Run(UpdateStateMachine);
            }
        }

        private void Initialize()
        {
            var factory = new MqttFactory();
            MqttClient = factory.CreateMqttClient();

            MqttClient.ConnectedAsync += (f) =>
            {
                State = UpdateState.Connected;
                return Task.CompletedTask;
            };

            MqttClient.DisconnectedAsync += (f) =>
            {
                State = UpdateState.Disconnected;
                return Task.CompletedTask;
            };

            MqttClient.ApplicationMessageReceivedAsync += (f) =>
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
                        Resolver.Log.Trace($"Already know about Update {info.ID}");
                    }
                    else
                    {
                        Store.Add(info);
                        OnUpdateAvailable?.Invoke(this, info);
                    }

                }
                return Task.CompletedTask;
            };

            ClientOptions = new MqttClientOptionsBuilder()
                            .WithClientId(Config.ClientID)
                            .WithTcpServer(Config.UpdateServer, Config.UpdatePort)
                            .WithCleanSession()
                            .Build();
        }

        private async Task UpdateStateMachine()
        {
            State = UpdateState.Disconnected;

            // update state machine
            while (true)
            {
                switch (State)
                {
                    case UpdateState.Disconnected:
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
                        catch (MqttCommunicationException)
                        {
                            Resolver.Log.Debug("Error connecting to Meadow.Cloud");
                            State = UpdateState.Disconnected;
                            //  just delay for a while
                            await Task.Delay(TimeSpan.FromSeconds(Config.CloudConnectRetrySeconds));
                        }
                        catch (Exception ex)
                        {
                            Resolver.Log.Error($"Error connecting to Meadow.Cloud: {ex.Message}");
                            State = UpdateState.Disconnected;
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
                        await Task.Delay(1000);
                        break;
                }
            }

            State = UpdateState.Dead;
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
                            await stream.CopyToAsync(fileStream);
                        }
                    }
                }

                // TODO: verify hash
                var path = Store.GetUpdateArchivePath(message.ID);
                var hash = Store.GetFileHash(new FileInfo(path));

                if (!string.IsNullOrWhiteSpace(message.DownloadHash))
                {
                    if (hash != message.DownloadHash)
                    {
                        Resolver.Log.Warn("Downloaded Hash does not match expected Hash");
                        // TODO: what do we do? Retry? Ignore?
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

            // TODO: cancel the app run cancellation token

            // wait for the app to return, or the timeout
            Task.WaitAny(shutDownTask, shutdownTimeoutTask);

            // restart the device
            Resolver.Device.Reset();

            // the OS will handle updates on the next boot

            // TODO: these will never happen due to the above reset. need to be moved to "post update boot"
            Store.SetApplied(updateInfo);
            OnUpdateSuccess(this, updateInfo);

            State = UpdateState.Idle;
        }
    }
}