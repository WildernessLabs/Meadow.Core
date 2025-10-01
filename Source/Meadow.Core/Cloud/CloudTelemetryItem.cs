using System.Threading;

namespace Meadow;

/// <summary>
/// Represents a telemetry item to be sent to the cloud, with priority and reception order
/// </summary>
public class CloudTelemetryItem
{
    private static long _receptionCounter = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudTelemetryItem"/> class with the specified item, endpoint, and
    /// priority.
    /// </summary>
    /// <param name="item">The telemetry item to be sent to the cloud. This object represents the data to be transmitted.</param>
    /// <param name="endpoint">The endpoint URL to which the telemetry item will be sent. Cannot be null or empty.</param>
    /// <param name="priority">The priority level of the telemetry item. Higher priority items may be processed before lower priority ones. The
    /// default value is 3.</param>
    public CloudTelemetryItem(object item, string endpoint, int priority = 3)
    {
        Item = item;
        EndPoint = endpoint;
        Priority = priority;
        ReceptionSequence = Interlocked.Increment(ref _receptionCounter);
    }

    /// <summary>
    /// Gets or sets the value associated with the current instance.
    /// </summary>
    public object Item { get; set; }
    /// <summary>
    /// Gets or sets the endpoint address used to connect to the service.
    /// </summary>
    public string EndPoint { get; set; }
    /// <summary>
    /// Gets or sets the priority level of the item.
    /// </summary>
    public int Priority { get; set; }
    /// <summary>
    /// Gets or sets the sequence number representing the order in which a message or event was received.
    /// </summary>
    public long ReceptionSequence { get; set; }
}
