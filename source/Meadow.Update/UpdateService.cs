using Meadow;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Text;
using System.Text.Json;

public class UpdateService
{
    private UpdateState _state;

    private IUpdateHandler Handler { get; }
    private UpdateConfig Config { get; }
    private IMqttClient MqttClient { get; set; }
    private MqttClientOptions ClientOptions { get; set; }

    public UpdateService(IUpdateHandler handler, UpdateConfig config)
    {
        Handler = handler;
        Config = config;
    }

    private UpdateState State
    {
        get => _state;
        set
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
                    await Task.Delay(1000);
                    break;
            }
        }

        State = UpdateState.Dead;
    }
}