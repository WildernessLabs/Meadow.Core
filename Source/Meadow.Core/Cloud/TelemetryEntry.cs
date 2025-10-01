namespace Meadow;

/// <summary>
/// Represents a telemetry entry with priority and reception ordering
/// </summary>
public class TelemetryEntry
{
    /// <summary>
    /// Gets or sets the priority level of the item.
    /// </summary>
    public int Priority { get; set; }
    /// <summary>
    /// Gets or sets the sequence number representing the order in which a message or event was received.
    /// </summary>
    public long ReceptionSequence { get; set; }
    /// <summary>
    /// Gets or sets the value associated with this item.
    /// </summary>
    public object Item { get; set; }
    /// <summary>
    /// Gets or sets the endpoint address used to connect to the service.
    /// </summary>
    public string EndPoint { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryEntry"/> class.
    /// </summary>
    /// <remarks>The <see cref="Item"/> property is initialized to a non-null default value, and the <see
    /// cref="EndPoint"/> property is initialized to an empty string.</remarks>
    public TelemetryEntry()
    {
        Item = null!;
        EndPoint = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryEntry"/> class with the specified item, endpoint,
    /// priority, and reception sequence.
    /// </summary>
    /// <param name="item">The telemetry item to be associated with this entry. This value cannot be <see langword="null"/>.</param>
    /// <param name="endpoint">The endpoint to which the telemetry item is associated. This value cannot be <see langword="null"/> or empty.</param>
    /// <param name="priority">The priority level of the telemetry entry. Higher values indicate higher priority.</param>
    /// <param name="receptionSequence">The sequence number indicating the order in which the telemetry item was received.</param>
    public TelemetryEntry(object item, string endpoint, int priority, long receptionSequence)
    {
        Item = item;
        EndPoint = endpoint;
        Priority = priority;
        ReceptionSequence = receptionSequence;
    }
}
