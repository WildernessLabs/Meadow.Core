using System.Runtime.InteropServices;

namespace Meadow.Cloud;

/// <summary>
/// Fixed-length record for CloudEvent telemetry in TalusDB
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CloudEventRecord
{
    /// <summary>
    /// Reception sequence number for FIFO ordering within priority
    /// </summary>
    public long ReceptionSequence;

    /// <summary>
    /// String pool ID for the serialized CloudEvent JSON
    /// </summary>
    public int EventJsonPoolId;

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudEventRecord"/> class with the specified reception sequence and
    /// event JSON pool identifier.
    /// </summary>
    /// <param name="receptionSequence">The sequence number representing the order in which the event was received.</param>
    /// <param name="eventJsonPoolId">The identifier of the JSON pool associated with the event.</param>
    public CloudEventRecord(long receptionSequence, int eventJsonPoolId)
    {
        ReceptionSequence = receptionSequence;
        EventJsonPoolId = eventJsonPoolId;
    }
}
