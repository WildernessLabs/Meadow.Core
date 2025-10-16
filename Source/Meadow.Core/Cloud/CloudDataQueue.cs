using System;
using System.Collections.Generic;

namespace Meadow.Cloud;

internal class CloudDataQueue
{
    private readonly IMeadowCloudTelemetryStore _store;

    public int Count => _store.Count;

    public CloudDataQueue(IMeadowCloudTelemetryStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        Resolver.Log.Info($"Cloud Data Queue initialized with telemetry store");
    }

    public CloudTelemetryItem? Peek()
    {
        return _store.Peek();
    }

    public CloudTelemetryItem? Dequeue()
    {
        return _store.Dequeue();
    }

    public void Enqueue(CloudTelemetryItem info)
    {
        _store.Enqueue(info);
    }

    public void Enqueue(CloudLog cloudLog, CloudTelemetryPriority priority = CloudTelemetryPriority.Normal)
    {
        var item = new CloudTelemetryItem(cloudLog, "/api/logs", priority);
        _store.Enqueue(item);
    }

    public void Enqueue(CloudEvent cloudEvent, CloudTelemetryPriority priority = CloudTelemetryPriority.Normal)
    {
        var item = new CloudTelemetryItem(cloudEvent, "/api/events", priority);
        _store.Enqueue(item);
    }

    public Dictionary<int, int> CountByPriority()
    {
        return _store.CountByPriority();
    }
}
