using MQTTnet;
using System;

namespace Meadow;

internal class MeadowCloudUpdateService
{
    private readonly MeadowCloudConnectionService _connectionService;

    public MeadowCloudUpdateService(MeadowCloudConnectionService connectionService)
    {
        _connectionService = connectionService;
        _connectionService.MqttMessageReceived += OnMqttMessageReceived;
        _connectionService.AddSubscription("{OID}/ota/{ID}");
    }

    private void OnMqttMessageReceived(object sender, MqttApplicationMessage e)
    {
        if (e.Topic.EndsWith($"/ota/{Resolver.Device.Information.UniqueID}", StringComparison.OrdinalIgnoreCase))
        {
            Resolver.Log.Info("Meadow update message received", "cloud");
        }
    }
}
