using System;
using System.IO;

namespace Meadow.Cloud;

/// <summary>
/// Factory for creating telemetry store instances based on configuration
/// </summary>
public static class TelemetryStoreFactory
{
    /// <summary>
    /// Creates a telemetry store of the specified type
    /// </summary>
    /// <param name="storeType">Type of store to create</param>
    /// <param name="baseDirectory">Base directory for persistent stores (ignored for InMemory)</param>
    /// <param name="sqliteBatchSize">Batch size for SQLite (default: 50)</param>
    /// <param name="sqliteBatchIntervalMs">Batch interval in milliseconds for SQLite (default: 1000)</param>
    /// <param name="sqliteMaxItems">Maximum items in SQLite database (default: 50000)</param>
    /// <returns>Configured telemetry store</returns>
    public static IMeadowCloudTelemetryStore Create(
        TelemetryStoreType storeType,
        string? baseDirectory = null,
        int sqliteBatchSize = 50,
        int sqliteBatchIntervalMs = 1000,
        int sqliteMaxItems = 50000)
    {
        switch (storeType)
        {
            case TelemetryStoreType.InMemory:
                Resolver.Log.Info("Creating InMemory telemetry store");
                return new InMemoryTelemetryStore();

            case TelemetryStoreType.TalusDB:
                if (string.IsNullOrEmpty(baseDirectory))
                {
                    baseDirectory = GetDefaultDirectory();
                }

                Resolver.Log.Info($"Creating TalusDB telemetry store at {baseDirectory}");
                return new TalusTelemetryStore(baseDirectory);

            case TelemetryStoreType.SQLite:
                if (string.IsNullOrEmpty(baseDirectory))
                {
                    baseDirectory = GetDefaultDirectory();
                }

                var dbPath = Path.Combine(baseDirectory, "telemetry.db");
                Resolver.Log.Info($"Creating SQLite telemetry store at {dbPath}");
                return new SqliteTelemetryStore(dbPath, sqliteBatchSize, sqliteBatchIntervalMs, sqliteMaxItems);

            default:
                throw new ArgumentException($"Unknown store type: {storeType}", nameof(storeType));
        }
    }

    /// <summary>
    /// Creates a telemetry store with auto-detection based on environment
    /// </summary>
    /// <param name="baseDirectory">Base directory for persistent stores</param>
    /// <returns>Best telemetry store for the current environment</returns>
    public static IMeadowCloudTelemetryStore CreateAutoDetect(string? baseDirectory = null)
    {
        // For embedded Linux systems (likely on NAND flash), prefer SQLite with batching
        if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            // Check if we're on an embedded system by looking for typical embedded paths
            if (Directory.Exists("/dev/mtd") || Directory.Exists("/sys/class/mtd"))
            {
                Resolver.Log.Info("Detected embedded system with NAND flash, using SQLite store");
                return Create(TelemetryStoreType.SQLite, baseDirectory);
            }
        }

        // For desktop/server environments, TalusDB is fine
        Resolver.Log.Info("Using TalusDB store");
        return Create(TelemetryStoreType.TalusDB, baseDirectory);
    }

    private static string GetDefaultDirectory()
    {
        // Use app data directory or temp if not available
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        if (string.IsNullOrEmpty(appData))
        {
            appData = Path.Combine(Path.GetTempPath(), "meadow");
        }

        var telemetryDir = Path.Combine(appData, "meadow", "telemetry");

        if (!Directory.Exists(telemetryDir))
        {
            Directory.CreateDirectory(telemetryDir);
        }

        return telemetryDir;
    }
}
