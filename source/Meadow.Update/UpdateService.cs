using Meadow;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Text;
using System.Text.Json;

public delegate void UpdateEventHandler(IUpdateService updateService, UpdateInfo info);

public interface IUpdateService
{
    event UpdateEventHandler OnUpdateAvailable;
    event UpdateEventHandler OnUpdateRetrieved;
    event UpdateEventHandler OnUpdateSuccess;
    event UpdateEventHandler OnUpdateFailure;

    bool CanUpdate => State == UpdateState.Idle;
    UpdateState State { get; }
    void RetrieveUpdate(UpdateInfo updateInfo);
    void ApplyUpdate(UpdateInfo updateInfo);
    void ClearUpdates();
}

public class UpdateService : IUpdateService
{
    public event UpdateEventHandler OnUpdateAvailable = delegate { };
    public event UpdateEventHandler OnUpdateRetrieved = delegate { };
    public event UpdateEventHandler OnUpdateSuccess = delegate { };
    public event UpdateEventHandler OnUpdateFailure = delegate { };

    private UpdateState _state;

    private UpdateConfig Config { get; }
    private IMqttClient MqttClient { get; set; }
    private MqttClientOptions ClientOptions { get; set; }
    private UpdateStore Store { get; }
    
    public UpdateService(UpdateConfig config)
    {
        var folder = Path.Combine(Path.GetTempPath(), "meadow-update");

        Store = new UpdateStore(folder);

        Config = config;
    }

    public UpdateState State
    {
        get => _state;
        private set
        {
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
                PropertyNameCaseInsensitive = true
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
        while(true)
        {
            switch(State)
            {
                case UpdateState.Disconnected:
                    State = UpdateState.Connecting;
                    await MqttClient.ConnectAsync(ClientOptions);
                    break;
                case UpdateState.Connected:
                    State = UpdateState.Idle;
                    await MqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(Config.RootTopic).Build());
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


        if(!Store.TryGetMessage(updateInfo.ID, out message))
        {
            throw new Exception($"Cannot find update with ID {updateInfo.ID}");
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
            OnUpdateRetrieved(this, message);
            Store.SetRetrieved(message);

            State = UpdateState.Idle;
        }
        catch(Exception ex)
        {
            // TODO: raise some event?
            Resolver.Log.Error($"Failed to download Update: {ex.Message}");
        }
    }

    public void ApplyUpdate(UpdateInfo updateInfo)
    {
        State = UpdateState.UpdateInProgress;

        // TODO: do stuff

        Store.SetApplied(updateInfo);
        OnUpdateSuccess(this, updateInfo);

        State = UpdateState.Idle;
    }
}