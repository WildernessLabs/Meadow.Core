using System;
using System.Collections.Generic;

namespace Meadow;

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
        _store.Enqueue(info.Item, info.EndPoint, info.Priority);
    }

    public void Enqueue<T>(T item, string endPoint, int priority = 3)
    {
        if (item == null) { return; }

        _store.Enqueue(item, endPoint, priority);
    }

    public Dictionary<int, int> CountByPriority()
    {
        return _store.CountByPriority();
    }
}
