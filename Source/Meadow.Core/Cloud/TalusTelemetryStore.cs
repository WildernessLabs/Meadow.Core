using Meadow.Foundation.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WildernessLabs.TalusDB;

namespace Meadow.Cloud;

/// <summary>
/// TalusDB-based telemetry store with priority support
/// Preserves type information and uses proper priority enum
/// </summary>
internal class TalusTelemetryStore : IMeadowCloudTelemetryStore, IDisposable
{
    // TODO: we probablyu need to sanigty-check these values and make them configurable
    private const int HighPriorityMaxElements = 100;
    private const int NormalPriorityMaxElements = 1000;
    private const int LowPriorityMaxElements = 100;

    private readonly Database _database;
    private readonly Dictionary<(Type, CloudTelemetryPriority), object> _tables;
    private readonly SimpleStringPool _stringPool;
    private readonly string _baseDirectory;
    private readonly object _lock = new object();
    private long _receptionCounter = 0;
    private bool _disposed = false;

    public int Count
    {
        get
        {
            lock (_lock)
            {
                int total = 0;
                foreach (var kvp in _tables)
                {
                    if (kvp.Value is Table<CloudLogRecord> logTable)
                    {
                        total += logTable.Count;
                    }
                    else if (kvp.Value is Table<CloudEventRecord> eventTable)
                    {
                        total += eventTable.Count;
                    }
                }
                return total;
            }
        }
    }

    public TalusTelemetryStore(string baseDirectory)
    {
        _baseDirectory = baseDirectory ?? throw new ArgumentNullException(nameof(baseDirectory));

        if (!Directory.Exists(_baseDirectory))
        {
            Directory.CreateDirectory(_baseDirectory);
        }

        // Create database with KeepOpen for best performance
        _database = new Database(_baseDirectory, StreamBehavior.KeepOpen);

        // Initialize table dictionary
        _tables = new Dictionary<(Type, CloudTelemetryPriority), object>();

        // Create or open tables for CloudLog (3 priorities)
        CreateOrOpenLogTable(CloudTelemetryPriority.High, HighPriorityMaxElements);
        CreateOrOpenLogTable(CloudTelemetryPriority.Normal, NormalPriorityMaxElements);
        CreateOrOpenLogTable(CloudTelemetryPriority.Low, LowPriorityMaxElements);

        // Create or open tables for CloudEvent (3 priorities)
        CreateOrOpenEventTable(CloudTelemetryPriority.High, HighPriorityMaxElements);
        CreateOrOpenEventTable(CloudTelemetryPriority.Normal, NormalPriorityMaxElements);
        CreateOrOpenEventTable(CloudTelemetryPriority.Low, LowPriorityMaxElements);

        // Initialize string pool
        _stringPool = new SimpleStringPool(Path.Combine(_baseDirectory, "strings.pool"));

        // Recover reception counter from existing data
        RecoverReceptionCounter();

        Resolver.Log.Info($"TalusDB Telemetry Store initialized with 6 tables at {_baseDirectory}");
    }

    private void CreateOrOpenLogTable(CloudTelemetryPriority priority, int maxElements)
    {
        var tablePath = Path.Combine(_baseDirectory, $"CloudLog_{priority}");
        if (!Directory.Exists(tablePath))
        {
            Directory.CreateDirectory(tablePath);
        }

        var db = new Database(tablePath, StreamBehavior.KeepOpen);

        Table<CloudLogRecord> table;

        // Check if table files exist on disk
        var tableExists = Directory.GetFiles(tablePath, "*.tdb").Length > 0;

        if (tableExists)
        {
            table = db.GetTable<CloudLogRecord>();
        }
        else
        {
            table = db.CreateTable<CloudLogRecord>(maxElements);
        }

        _tables[(typeof(CloudLog), priority)] = table;
    }

    private void CreateOrOpenEventTable(CloudTelemetryPriority priority, int maxElements)
    {
        var tablePath = Path.Combine(_baseDirectory, $"CloudEvent_{priority}");
        if (!Directory.Exists(tablePath))
        {
            Directory.CreateDirectory(tablePath);
        }

        var db = new Database(tablePath, StreamBehavior.KeepOpen);

        Table<CloudEventRecord> table;

        // Check if table files exist on disk
        var tableExists = Directory.GetFiles(tablePath, "*.tdb").Length > 0;

        if (tableExists)
        {
            table = db.GetTable<CloudEventRecord>();
        }
        else
        {
            table = db.CreateTable<CloudEventRecord>(maxElements);
        }

        _tables[(typeof(CloudEvent), priority)] = table;
    }

    private void RecoverReceptionCounter()
    {
        long maxSequence = 0;

        // Scan all tables to find the highest reception sequence
        foreach (var kvp in _tables)
        {
            if (kvp.Value is Table<CloudLogRecord> logTable && logTable.Count > 0)
            {
                var item = logTable.Peek();
                if (item.HasValue && item.Value.ReceptionSequence > maxSequence)
                {
                    maxSequence = item.Value.ReceptionSequence;
                }
            }
            else if (kvp.Value is Table<CloudEventRecord> eventTable && eventTable.Count > 0)
            {
                var item = eventTable.Peek();
                if (item.HasValue && item.Value.ReceptionSequence > maxSequence)
                {
                    maxSequence = item.Value.ReceptionSequence;
                }
            }
        }

        _receptionCounter = maxSequence;

        if (_receptionCounter > 0)
        {
            Resolver.Log.Info($"TalusDB: Recovered reception counter: {_receptionCounter}");
        }
    }

    public void Enqueue(CloudTelemetryItem item)
    {
        if (item == null || item.Item == null) return;

        var priority = item.Priority;

        lock (_lock)
        {
            try
            {
                var itemType = item.Item.GetType();

                // Serialize item to JSON
                var json = MicroJson.Serialize(item.Item);
                if (json == null) return;

                // Store in pool and get ID
                var jsonPoolId = _stringPool.Add(json);

                // Increment sequence
                var sequence = ++_receptionCounter;

                // Determine which table to use based on type
                if (itemType.Name == "CloudLog")
                {
                    var key = (typeof(CloudLog), priority);
                    if (_tables.TryGetValue(key, out var tableObj) && tableObj is Table<CloudLogRecord> table)
                    {
                        var record = new CloudLogRecord(sequence, jsonPoolId);
                        table.Insert(record);
                    }
                }
                else if (itemType.Name == "CloudEvent")
                {
                    var key = (typeof(CloudEvent), priority);
                    if (_tables.TryGetValue(key, out var tableObj) && tableObj is Table<CloudEventRecord> table)
                    {
                        var record = new CloudEventRecord(sequence, jsonPoolId);
                        table.Insert(record);
                    }
                }
                else
                {
                    Resolver.Log.Warn($"TalusDB: Unknown item type {itemType.Name}, cannot enqueue");
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"TalusDB: Failed to enqueue item: {ex.Message}");
            }
        }
    }

    public CloudTelemetryItem? Peek()
    {
        lock (_lock)
        {
            return GetNextItem(remove: false);
        }
    }

    public CloudTelemetryItem? Dequeue()
    {
        lock (_lock)
        {
            return GetNextItem(remove: true);
        }
    }

    private CloudTelemetryItem? GetNextItem(bool remove)
    {
        // Check tables in priority order (High -> Normal -> Low)
        // Within each priority, check CloudLog first, then CloudEvent
        var priorities = new[] { CloudTelemetryPriority.High, CloudTelemetryPriority.Normal, CloudTelemetryPriority.Low };

        foreach (var priority in priorities)
        {
            // Try CloudLog first
            var logKey = (typeof(CloudLog), priority);
            if (_tables.TryGetValue(logKey, out var logTableObj) && logTableObj is Table<CloudLogRecord> logTable)
            {
                if (logTable.Count > 0)
                {
                    var item = TryGetLogItem(logTable, priority, remove);
                    if (item != null) return item;
                }
            }

            // Then try CloudEvent
            var eventKey = (typeof(CloudEvent), priority);
            if (_tables.TryGetValue(eventKey, out var eventTableObj) && eventTableObj is Table<CloudEventRecord> eventTable)
            {
                if (eventTable.Count > 0)
                {
                    var item = TryGetEventItem(eventTable, priority, remove);
                    if (item != null) return item;
                }
            }
        }

        return null;
    }

    private CloudTelemetryItem? TryGetLogItem(Table<CloudLogRecord> table, CloudTelemetryPriority priority, bool remove)
    {
        try
        {
            CloudLogRecord? record = remove ? table.Remove() : table.Peek();

            if (record.HasValue)
            {
                // Retrieve JSON from pool
                var json = _stringPool.Get(record.Value.LogJsonPoolId);
                if (json == null)
                {
                    Resolver.Log.Warn($"TalusDB: String pool lookup failed for CloudLog (poolId={record.Value.LogJsonPoolId}, seq={record.Value.ReceptionSequence}) - data lost but continuing");
                    // This record is corrupted, remove it if we're dequeuing
                    if (remove)
                    {
                        // Already removed above, just continue
                    }
                    else
                    {
                        // If peeking and corrupted, remove it to avoid getting stuck
                        table.Remove();
                        Resolver.Log.Info($"TalusDB: Removed corrupted CloudLog record to continue operation");
                    }
                    return null;
                }

                // Deserialize as dictionary to preserve structure for re-serialization
                var log = MicroJson.Deserialize<Dictionary<string, object>>(json);

                // Release from pool if removed
                if (remove)
                {
                    _stringPool.Release(record.Value.LogJsonPoolId);
                }

                return new CloudTelemetryItem(log, "/api/logs", priority)
                {
                    ReceptionSequence = record.Value.ReceptionSequence
                };
            }
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"TalusDB: Failed to retrieve CloudLog: {ex.Message}");
            // Try to remove corrupted record if we haven't already
            if (!remove)
            {
                try
                {
                    table.Remove();
                    Resolver.Log.Info($"TalusDB: Removed corrupted CloudLog record after error");
                }
                catch
                {
                    // Best effort - continue even if removal fails
                }
            }
        }

        return null;
    }

    private CloudTelemetryItem? TryGetEventItem(Table<CloudEventRecord> table, CloudTelemetryPriority priority, bool remove)
    {
        try
        {
            CloudEventRecord? record = remove ? table.Remove() : table.Peek();

            if (record.HasValue)
            {
                // Retrieve JSON from pool
                var json = _stringPool.Get(record.Value.EventJsonPoolId);
                if (json == null)
                {
                    Resolver.Log.Warn($"TalusDB: String pool lookup failed for CloudEvent (poolId={record.Value.EventJsonPoolId}, seq={record.Value.ReceptionSequence}) - data lost but continuing");
                    // This record is corrupted, remove it if we're dequeuing
                    if (remove)
                    {
                        // Already removed above, just continue
                    }
                    else
                    {
                        // If peeking and corrupted, remove it to avoid getting stuck
                        table.Remove();
                        Resolver.Log.Info($"TalusDB: Removed corrupted CloudEvent record to continue operation");
                    }
                    return null;
                }

                // Deserialize as dictionary to preserve structure for re-serialization
                var cloudEvent = MicroJson.Deserialize<Dictionary<string, object>>(json);

                // Release from pool if removed
                if (remove)
                {
                    _stringPool.Release(record.Value.EventJsonPoolId);
                }

                return new CloudTelemetryItem(cloudEvent, "/api/events", priority)
                {
                    ReceptionSequence = record.Value.ReceptionSequence
                };
            }
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"TalusDB: Failed to retrieve CloudEvent: {ex.Message}");
            // Try to remove corrupted record if we haven't already
            if (!remove)
            {
                try
                {
                    table.Remove();
                    Resolver.Log.Info($"TalusDB: Removed corrupted CloudEvent record after error");
                }
                catch
                {
                    // Best effort - continue even if removal fails
                }
            }
        }

        return null;
    }

    public Dictionary<int, int> CountByPriority()
    {
        lock (_lock)
        {
            var counts = new Dictionary<int, int>();

            foreach (CloudTelemetryPriority priority in Enum.GetValues(typeof(CloudTelemetryPriority)))
            {
                int count = 0;

                // Count CloudLog items
                var logKey = (typeof(CloudLog), priority);
                if (_tables.TryGetValue(logKey, out var logTableObj) && logTableObj is Table<CloudLogRecord> logTable)
                {
                    count += logTable.Count;
                }

                // Count CloudEvent items
                var eventKey = (typeof(CloudEvent), priority);
                if (_tables.TryGetValue(eventKey, out var eventTableObj) && eventTableObj is Table<CloudEventRecord> eventTable)
                {
                    count += eventTable.Count;
                }

                if (count > 0)
                {
                    counts[(int)priority] = count;
                }
            }

            return counts;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        lock (_lock)
        {
            // Close all tables
            foreach (var tableObj in _tables.Values)
            {
                if (tableObj is Table<CloudLogRecord> logTable)
                {
                    logTable.Close();
                }
                else if (tableObj is Table<CloudEventRecord> eventTable)
                {
                    eventTable.Close();
                }
            }

            _database?.CloseAllTables();

            // Save string pool
            _stringPool?.Dispose();

            _disposed = true;
        }

        Resolver.Log.Info("TalusDB Telemetry Store disposed");
    }
}

/// <summary>
/// Simple reference-counted string pool for TalusDB
/// </summary>
internal class SimpleStringPool : IDisposable
{
    private readonly Dictionary<int, PoolEntry> _pool = new Dictionary<int, PoolEntry>();
    private readonly Dictionary<string, int> _stringToId = new Dictionary<string, int>();
    private readonly string _filePath;
    private int _nextId = 1;
    private readonly object _lock = new object();

    private class PoolEntry
    {
        public string Value { get; set; }
        public int RefCount { get; set; }

        public PoolEntry(string value)
        {
            Value = value;
            RefCount = 1;
        }
    }

    public SimpleStringPool(string filePath)
    {
        _filePath = filePath;
        Load();
    }

    public int Add(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return 0;
        }

        lock (_lock)
        {
            // Check if string already exists
            if (_stringToId.TryGetValue(value, out var existingId))
            {
                _pool[existingId].RefCount++;
                // Save pool after ref count change for crash safety
                Save();
                return existingId;
            }

            // Add new string
            var id = _nextId++;
            _pool[id] = new PoolEntry(value);
            _stringToId[value] = id;

            // Save pool immediately for crash safety
            Save();

            return id;
        }
    }

    public string? Get(int id)
    {
        if (id == 0) return null;

        lock (_lock)
        {
            if (_pool.TryGetValue(id, out var entry))
            {
                return entry.Value;
            }
            return null;
        }
    }

    public void Release(int id)
    {
        if (id == 0) return;

        lock (_lock)
        {
            if (_pool.TryGetValue(id, out var entry))
            {
                entry.RefCount--;
                if (entry.RefCount <= 0)
                {
                    // Remove from pool
                    _stringToId.Remove(entry.Value);
                    _pool.Remove(id);
                }
                // Save pool after release for crash safety
                Save();
            }
        }
    }

    private void Load()
    {
        // Try loading from main file first, then backup if that fails
        if (TryLoadFromFile(_filePath))
        {
            return;
        }

        // Main file failed, try backup
        var backupPath = _filePath + ".bak";
        if (File.Exists(backupPath))
        {
            Resolver.Log.Warn($"Main string pool corrupted or missing, attempting recovery from backup");
            if (TryLoadFromFile(backupPath))
            {
                // Restore backup as main file
                try
                {
                    File.Copy(backupPath, _filePath, true);
                    Resolver.Log.Info($"Restored string pool from backup");
                }
                catch (Exception ex)
                {
                    Resolver.Log.Error($"Failed to restore backup as main file: {ex.Message}");
                }
                return;
            }
        }

        Resolver.Log.Warn($"No valid string pool found, starting fresh");
    }

    private bool TryLoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }

        try
        {
            var allLines = File.ReadAllLines(filePath);
            if (allLines.Length == 0)
            {
                return false;
            }

            // Check for checksum (first line)
            int? expectedChecksum = null;
            int startLine = 0;

            if (allLines[0].StartsWith("#CHECKSUM:"))
            {
                if (int.TryParse(allLines[0].Substring(10), out var checksum))
                {
                    expectedChecksum = checksum;
                    startLine = 1;
                }
            }

            // Collect data lines (excluding checksum line)
            var dataLines = allLines.Skip(startLine).ToList();

            // Validate checksum if present
            if (expectedChecksum.HasValue)
            {
                var content = string.Join("\n", dataLines);
                var actualChecksum = ComputeSimpleChecksum(content);

                if (actualChecksum != expectedChecksum.Value)
                {
                    Resolver.Log.Error($"String pool checksum mismatch in {filePath} (expected: {expectedChecksum.Value}, actual: {actualChecksum}) - file is corrupted");
                    return false;
                }
            }

            // Parse data lines
            int validLines = 0;
            int invalidLines = 0;

            foreach (var line in dataLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var parts = line.Split('|');
                if (parts.Length == 3 && int.TryParse(parts[0], out var id) && int.TryParse(parts[2], out var refCount))
                {
                    var value = parts[1];
                    _pool[id] = new PoolEntry(value) { RefCount = refCount };
                    _stringToId[value] = id;
                    if (id >= _nextId)
                    {
                        _nextId = id + 1;
                    }
                    validLines++;
                }
                else
                {
                    invalidLines++;
                    Resolver.Log.Warn($"Skipping corrupted string pool entry: {line.Substring(0, Math.Min(50, line.Length))}...");
                }
            }

            if (validLines == 0 && invalidLines > 0)
            {
                Resolver.Log.Error($"String pool in {filePath} has no valid entries");
                return false;
            }

            Resolver.Log.Info($"Loaded {validLines} strings from {filePath}" +
                (invalidLines > 0 ? $" ({invalidLines} corrupted entries skipped)" : ""));
            return true;
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Failed to load string pool from {filePath}: {ex.Message}");
            return false;
        }
    }

    private void Save()
    {
        try
        {
            var lines = _pool.Select(kvp => $"{kvp.Key}|{kvp.Value.Value}|{kvp.Value.RefCount}").ToList();

            // Calculate checksum for integrity verification
            var content = string.Join("\n", lines);
            var checksum = ComputeSimpleChecksum(content);

            // Use atomic write with temp file and fsync to prevent corruption from power loss
            var tempPath = _filePath + ".tmp";

            // Write to temp file with explicit flush to disk
            using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
            using (var writer = new StreamWriter(stream))
            {
                // Write checksum as first line
                writer.WriteLine($"#CHECKSUM:{checksum}");

                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }

                writer.Flush();
                stream.Flush(flushToDisk: true); // Ensure data hits physical disk
            }

            // Now perform atomic rename
            var backupPath = _filePath + ".bak";

            if (File.Exists(_filePath))
            {
                // Move current file to backup
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
                File.Move(_filePath, backupPath);
            }

            // Move temp to current (atomic on most filesystems)
            File.Move(tempPath, _filePath);

            // Keep backup file for recovery - don't delete it
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Failed to save string pool: {ex.Message}");

            // Clean up temp file if it exists
            try
            {
                var tempPath = _filePath + ".tmp";
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
            catch
            {
                // Best effort cleanup
            }
        }
    }

    private int ComputeSimpleChecksum(string content)
    {
        unchecked
        {
            int hash = 17;
            foreach (char c in content)
            {
                hash = hash * 31 + c;
            }
            return hash;
        }
    }

    public void Dispose()
    {
        Save();
    }
}
