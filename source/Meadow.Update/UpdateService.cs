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
}

public class UpdateService : IUpdateService
{
    public event UpdateEventHandler OnUpdateAvailable;
    public event UpdateEventHandler OnUpdateRetrieved;
    public event UpdateEventHandler OnUpdateSuccess;
    public event UpdateEventHandler OnUpdateFailure;

    private UpdateState _state;

    private UpdateConfig Config { get; }
    private IMqttClient MqttClient { get; set; }
    private MqttClientOptions ClientOptions { get; set; }
    private Queue<(UpdateInfo, UpdateMessage)> UpdateQueue { get; set; } = new Queue<(UpdateInfo, UpdateMessage)>();
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

            var message = JsonSerializer.Deserialize<UpdateMessage>(json, opts);

            var info = new UpdateInfo
            {
                // TODO: fill this
                 PublicationDate = message.PublishedOn
            };

            lock (UpdateQueue)
            {
                UpdateQueue.Enqueue(new(info, message));
            }

            Resolver.Log.Trace($"Updater Message Received: {message.MpakID}");
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
                    if (UpdateQueue.Count > 0)
                    {
                        State = UpdateState.UpdateAvailable;
                    }
                    else
                    {
                        await Task.Delay(1000);
                    }
                    break;
                case UpdateState.UpdateAvailable:
                    State = UpdateState.Idle;
                    OnUpdateAvailable?.Invoke(this, null); // TODO: track these notifications
                    break;
                case UpdateState.DownloadingFile:
                case UpdateState.UpdateInProgress:
                default:
                    await Task.Delay(1000);
                    break;
            }
        }

        State = UpdateState.Dead;
    }

    public void RetrieveUpdate(UpdateInfo updateInfo)
    {
        State = UpdateState.DownloadingFile;
        OnUpdateRetrieved(this, updateInfo);
    }

    public void ApplyUpdate(UpdateInfo updateInfo)
    {
        State = UpdateState.UpdateInProgress;

        OnUpdateSuccess(this, updateInfo);
    }
}