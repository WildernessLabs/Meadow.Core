using System.Collections.Generic;

namespace Meadow;

/// <summary>
/// Interface for persistent telemetry storage with priority and reception-order support
/// </summary>
public interface IMeadowCloudTelemetryStore
{
    /// <summary>
    /// Gets the total number of items in the store
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Adds an item to the store with the specified priority
    /// </summary>
    /// <param name="item">The data item to store</param>
    /// <param name="endpoint">The cloud endpoint for this data</param>
    /// <param name="priority">Priority level (0 = highest/errors, 3+ = lowest/general)</param>
    void Enqueue(object item, string endpoint, int priority = 3);

    /// <summary>
    /// Retrieves the next item without removing it from the store
    /// Priority-first (lowest priority value first), then reception-order (FIFO within priority)
    /// </summary>
    /// <returns>The next item or null if store is empty</returns>
    CloudTelemetryItem? Peek();

    /// <summary>
    /// Retrieves and removes the next item from the store
    /// Priority-first (lowest priority value first), then reception-order (FIFO within priority)
    /// </summary>
    /// <returns>The next item or null if store is empty</returns>
    CloudTelemetryItem? Dequeue();

    /// <summary>
    /// Gets the count of items by priority level
    /// </summary>
    /// <returns>Dictionary mapping priority levels to counts</returns>
    Dictionary<int, int> CountByPriority();
}
