using System.Runtime.InteropServices;

namespace Meadow.Cloud;

/// <summary>
/// Fixed-length telemetry record for TalusDB storage
/// Stores string pool references instead of actual strings
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CloudTelemetryRecord
{
    /// <summary>
    /// Reception sequence number for FIFO ordering within priority
    /// </summary>
    public long ReceptionSequence;

    /// <summary>
    /// String pool ID for the serialized item JSON
    /// </summary>
    public int ItemJsonPoolId;

    /// <summary>
    /// String pool ID for the endpoint string
    /// </summary>
    public int EndpointPoolId;

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudTelemetryRecord"/> class with the specified reception
    /// sequence, item JSON pool ID, and endpoint pool ID.
    /// </summary>
    /// <param name="receptionSequence">The sequence number indicating the order in which the telemetry record was received.</param>
    /// <param name="itemJsonPoolId">The identifier for the JSON pool associated with the telemetry item.</param>
    /// <param name="endpointPoolId">The identifier for the endpoint pool associated with the telemetry record.</param>
    public CloudTelemetryRecord(long receptionSequence, int itemJsonPoolId, int endpointPoolId)
    {
        ReceptionSequence = receptionSequence;
        ItemJsonPoolId = itemJsonPoolId;
        EndpointPoolId = endpointPoolId;
    }
}
