namespace Meadow.Cloud;

/// <summary>
/// Types of telemetry storage backends
/// </summary>
public enum TelemetryStoreType
{
    /// <summary>
    /// In-memory only - fastest but no persistence
    /// Data lost on restart
    /// </summary>
    InMemory,

    /// <summary>
    /// TalusDB - custom binary format
    /// Good balance of speed and persistence
    /// Lower memory overhead
    /// </summary>
    TalusDB,

    /// <summary>
    /// SQLite with batched writes - best for slow NAND flash
    /// Non-blocking enqueue, automatic crash recovery
    /// Higher memory overhead, best durability
    /// </summary>
    SQLite
}
