using System.Runtime.InteropServices;

namespace Meadow.Cloud;

/// <summary>
/// Fixed-length record for CloudLog telemetry in TalusDB
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CloudLogRecord
{
    /// <summary>
    /// Reception sequence number for FIFO ordering within priority
    /// </summary>
    public long ReceptionSequence;

    /// <summary>
    /// String pool ID for the serialized CloudLog JSON
    /// </summary>
    public int LogJsonPoolId;

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudLogRecord"/> class with the specified reception sequence and
    /// log JSON pool identifier.
    /// </summary>
    /// <param name="receptionSequence">The sequence number indicating the order in which the log was received.</param>
    /// <param name="logJsonPoolId">The identifier of the JSON pool associated with the log entry.</param>
    public CloudLogRecord(long receptionSequence, int logJsonPoolId)
    {
        ReceptionSequence = receptionSequence;
        LogJsonPoolId = logJsonPoolId;
    }
}
