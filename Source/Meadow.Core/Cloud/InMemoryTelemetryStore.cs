using System;
using System.Collections.Generic;
using System.Threading;

namespace Meadow.Cloud;

/// <summary>
/// In-memory telemetry store with no persistence and configurable capacity
/// Fastest option but data lost on restart
/// Uses three CircularBuffers (one per priority) for automatic capacity management
/// </summary>
internal class InMemoryTelemetryStore : IMeadowCloudTelemetryStore
{
    private readonly CircularBuffer<CloudTelemetryItem> _highPriorityBuffer;
    private readonly CircularBuffer<CloudTelemetryItem> _normalPriorityBuffer;
    private readonly CircularBuffer<CloudTelemetryItem> _lowPriorityBuffer;
    private readonly Lock _lock = new();
    private long _nextSequence = 1;

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _highPriorityBuffer.Count + _normalPriorityBuffer.Count + _lowPriorityBuffer.Count;
            }
        }
    }

    /// <summary>
    /// Creates an in-memory telemetry store with specified capacity per priority
    /// </summary>
    /// <param name="highPriorityCapacity">Maximum items for high priority (default: 100)</param>
    /// <param name="normalPriorityCapacity">Maximum items for normal priority (default: 1000)</param>
    /// <param name="lowPriorityCapacity">Maximum items for low priority (default: 100)</param>
    public InMemoryTelemetryStore(
        int highPriorityCapacity = 100,
        int normalPriorityCapacity = 1000,
        int lowPriorityCapacity = 100)
    {
        _highPriorityBuffer = new CircularBuffer<CloudTelemetryItem>(highPriorityCapacity);
        _normalPriorityBuffer = new CircularBuffer<CloudTelemetryItem>(normalPriorityCapacity);
        _lowPriorityBuffer = new CircularBuffer<CloudTelemetryItem>(lowPriorityCapacity);

        Resolver.Log.Info($"In-Memory Telemetry Store initialized (High: {highPriorityCapacity}, Normal: {normalPriorityCapacity}, Low: {lowPriorityCapacity})");
    }

    public void Enqueue(CloudTelemetryItem item)
    {
        if (item == null || item.Item == null)
        {
            return;
        }

        lock (_lock)
        {
            // Assign sequence number
            item.ReceptionSequence = Interlocked.Increment(ref _nextSequence);

            // Add to appropriate buffer based on priority
            // CircularBuffer automatically drops oldest when full
            switch (item.Priority)
            {
                case CloudTelemetryPriority.High:
                    _highPriorityBuffer.Append(item);
                    break;
                case CloudTelemetryPriority.Normal:
                    _normalPriorityBuffer.Append(item);
                    break;
                case CloudTelemetryPriority.Low:
                    _lowPriorityBuffer.Append(item);
                    break;
            }
        }
    }

    public CloudTelemetryItem? Peek()
    {
        lock (_lock)
        {
            // Check buffers in priority order: High -> Normal -> Low
            if (_highPriorityBuffer.Count > 0)
            {
                return _highPriorityBuffer.Peek();
            }

            if (_normalPriorityBuffer.Count > 0)
            {
                return _normalPriorityBuffer.Peek();
            }

            if (_lowPriorityBuffer.Count > 0)
            {
                return _lowPriorityBuffer.Peek();
            }

            return null;
        }
    }

    public CloudTelemetryItem? Dequeue()
    {
        lock (_lock)
        {
            // Check buffers in priority order: High -> Normal -> Low
            if (_highPriorityBuffer.Count > 0)
            {
                return _highPriorityBuffer.Remove();
            }

            if (_normalPriorityBuffer.Count > 0)
            {
                return _normalPriorityBuffer.Remove();
            }

            if (_lowPriorityBuffer.Count > 0)
            {
                return _lowPriorityBuffer.Remove();
            }

            return null;
        }
    }

    public Dictionary<int, int> CountByPriority()
    {
        lock (_lock)
        {
            var counts = new Dictionary<int, int>();

            if (_highPriorityBuffer.Count > 0)
            {
                counts[(int)CloudTelemetryPriority.High] = _highPriorityBuffer.Count;
            }

            if (_normalPriorityBuffer.Count > 0)
            {
                counts[(int)CloudTelemetryPriority.Normal] = _normalPriorityBuffer.Count;
            }

            if (_lowPriorityBuffer.Count > 0)
            {
                counts[(int)CloudTelemetryPriority.Low] = _lowPriorityBuffer.Count;
            }

            return counts;
        }
    }
}
