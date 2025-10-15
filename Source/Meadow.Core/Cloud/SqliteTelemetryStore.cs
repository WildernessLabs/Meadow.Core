using Meadow.Foundation.Serialization;
using SQLite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Cloud;

/// <summary>
/// SQLite-based telemetry store with batched writes
/// </summary>
internal class SqliteTelemetryStore : IMeadowCloudTelemetryStore, IDisposable
{
    private const int DefaultBatchSize = 50;
    private const int DefaultBatchIntervalMs = 1000;
    private const int MaxPendingItems = 10000;
    private const int DefaultMaxDbItems = 50000;

    private readonly string _databasePath;
    private readonly ConcurrentQueue<CloudTelemetryItem> _pendingWrites;
    private readonly SemaphoreSlim _batchSignal;
    private readonly CancellationTokenSource _cancellationSource;
    private readonly Task _batchWriterTask;
    private readonly object _dbLock = new object();
    private readonly int _batchSize;
    private readonly int _batchIntervalMs;
    private readonly int _maxDbItems;

    private SQLiteConnection? _connection;
    private long _nextSequence = 1;
    private bool _disposed = false;

    private volatile int _pendingCount = 0;

    public int Count
    {
        get
        {
            lock (_dbLock)
            {
                EnsureConnection();
                return _connection!.ExecuteScalar<int>("SELECT COUNT(*) FROM telemetry");
            }
        }
    }

    public SqliteTelemetryStore(
        string databasePath,
        int batchSize = DefaultBatchSize,
        int batchIntervalMs = DefaultBatchIntervalMs,
        int maxDbItems = DefaultMaxDbItems)
    {
        _databasePath = databasePath ?? throw new ArgumentNullException(nameof(databasePath));
        _batchSize = batchSize;
        _batchIntervalMs = batchIntervalMs;
        _maxDbItems = maxDbItems;

        _pendingWrites = new ConcurrentQueue<CloudTelemetryItem>();
        _batchSignal = new SemaphoreSlim(0);
        _cancellationSource = new CancellationTokenSource();

        // Ensure directory exists
        var directory = Path.GetDirectoryName(_databasePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Initialize database
        InitializeDatabase();

        // Recover sequence counter
        RecoverSequenceCounter();

        // Start background batch writer
        _batchWriterTask = Task.Run(BatchWriterLoop, _cancellationSource.Token);

        Resolver.Log.Info($"SQLite Telemetry Store initialized at {_databasePath} (batch size: {_batchSize}, interval: {_batchIntervalMs}ms, max items: {_maxDbItems})");
    }

    private void InitializeDatabase()
    {
        lock (_dbLock)
        {
            try
            {
                EnsureConnection();

                // Enable WAL mode for crash safety and better concurrency
                // PRAGMA statements return results, so use ExecuteScalar instead of Execute
                _connection!.ExecuteScalar<string>("PRAGMA journal_mode=WAL");

                // Set synchronous mode to NORMAL for balance of safety and performance
                // FULL is safest but very slow on NAND flash
                _connection.ExecuteScalar<string>("PRAGMA synchronous=NORMAL");

                // Increase cache size for better performance (10MB)
                _connection.Execute("PRAGMA cache_size=-10000");

                // Create telemetry table
                _connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS telemetry (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        sequence INTEGER NOT NULL,
                        priority INTEGER NOT NULL,
                        endpoint TEXT NOT NULL,
                        json_data TEXT NOT NULL,
                        created_at INTEGER NOT NULL
                    )");

                // Create index for efficient priority-based retrieval
                _connection.Execute(@"
                    CREATE INDEX IF NOT EXISTS idx_priority_sequence
                    ON telemetry(priority ASC, sequence ASC)");

                // Create index for sequence lookup
                _connection.Execute(@"
                    CREATE INDEX IF NOT EXISTS idx_sequence
                    ON telemetry(sequence ASC)");

                Resolver.Log.Info("SQLite database initialized successfully");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to initialize SQLite database: {ex.Message}");
                throw;
            }
        }
    }

    private void EnsureConnection()
    {
        if (_connection == null)
        {
            try
            {
                _connection = new SQLiteConnection(_databasePath);
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to create SQLite connection: {ex.GetType().Name} - {ex.Message}");
                throw new InvalidOperationException($"Unable to initialize SQLite database at {_databasePath}. This may be due to missing native libraries or platform incompatibility.", ex);
            }
        }
    }

    private void RecoverSequenceCounter()
    {
        lock (_dbLock)
        {
            try
            {
                EnsureConnection();
                var result = _connection!.ExecuteScalar<long?>("SELECT MAX(sequence) FROM telemetry");

                if (result.HasValue)
                {
                    _nextSequence = result.Value + 1;
                    Resolver.Log.Info($"SQLite: Recovered sequence counter: {_nextSequence}");
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to recover sequence counter: {ex.Message}");
            }
        }
    }

    public void Enqueue(CloudTelemetryItem item)
    {
        if (item == null || item.Item == null)
        {
            return;
        }

        // Check if queue is getting too full (backpressure)
        if (_pendingCount >= MaxPendingItems)
        {
            Resolver.Log.Warn($"SQLite: Pending queue full ({_pendingCount} items), dropping oldest");
            // Drop one item to make room
            if (_pendingWrites.TryDequeue(out _))
            {
                Interlocked.Decrement(ref _pendingCount);
            }
        }

        // Assign sequence number
        item.ReceptionSequence = Interlocked.Increment(ref _nextSequence);

        // Add to queue (non-blocking)
        _pendingWrites.Enqueue(item);
        Interlocked.Increment(ref _pendingCount);

        // Signal batch writer
        _batchSignal.Release();
    }

    private async Task BatchWriterLoop()
    {
        var batch = new List<CloudTelemetryItem>(_batchSize);

        while (!_cancellationSource.Token.IsCancellationRequested)
        {
            try
            {
                // Wait for signal or timeout
                await _batchSignal.WaitAsync(_batchIntervalMs, _cancellationSource.Token);

                // Collect items for batch
                batch.Clear();
                while (batch.Count < _batchSize && _pendingWrites.TryDequeue(out var item))
                {
                    batch.Add(item);
                    Interlocked.Decrement(ref _pendingCount);
                }

                // Write batch if we have items
                if (batch.Count > 0)
                {
                    await Task.Run(() => WriteBatch(batch));
                }
            }
            catch (OperationCanceledException)
            {
                // Expected during shutdown
                break;
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"SQLite batch writer error: {ex.Message}");
                // Continue running despite errors
                await Task.Delay(1000); // Back off on error
            }
        }

        // Final flush on shutdown
        batch.Clear();
        while (_pendingWrites.TryDequeue(out var item))
        {
            batch.Add(item);
        }

        if (batch.Count > 0)
        {
            WriteBatch(batch);
        }
    }

    private void WriteBatch(List<CloudTelemetryItem> items)
    {
        if (items.Count == 0) return;

        lock (_dbLock)
        {
            try
            {
                EnsureConnection();

                // Prepare records for bulk insert
                var records = new List<TelemetryDbRecord>(items.Count);
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                foreach (var item in items)
                {
                    try
                    {
                        var json = MicroJson.Serialize(item.Item);
                        if (json == null) continue;

                        records.Add(new TelemetryDbRecord
                        {
                            Sequence = item.ReceptionSequence,
                            Priority = (int)item.Priority,
                            Endpoint = item.EndPoint ?? "",
                            JsonData = json,
                            CreatedAt = timestamp
                        });
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Error($"SQLite: Failed to serialize item: {ex.Message}");
                    }
                }

                if (records.Count > 0)
                {
                    // Use InsertAll for true bulk insert - much faster than individual inserts
                    _connection!.InsertAll(records);

                    // Check if we've exceeded max capacity and trim if needed
                    var currentCount = _connection.ExecuteScalar<int>("SELECT COUNT(*) FROM telemetry");
                    if (currentCount > _maxDbItems)
                    {
                        // Delete oldest items (lowest priority first, then oldest sequence)
                        var deleteCount = currentCount - _maxDbItems;
                        _connection.Execute(@"
                            DELETE FROM telemetry
                            WHERE id IN (
                                SELECT id FROM telemetry
                                ORDER BY priority DESC, sequence ASC
                                LIMIT ?
                            )", deleteCount);

                        Resolver.Log.Trace($"SQLite: Trimmed {deleteCount} oldest items (max capacity: {_maxDbItems})");
                    }

                    if (records.Count > 1)
                    {
                        Resolver.Log.Trace($"SQLite: Wrote batch of {records.Count} items");
                    }
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"SQLite: Batch write failed: {ex.Message}");
            }
        }
    }

    public CloudTelemetryItem? Peek()
    {
        lock (_dbLock)
        {
            return GetNextItem(remove: false);
        }
    }

    public CloudTelemetryItem? Dequeue()
    {
        lock (_dbLock)
        {
            return GetNextItem(remove: true);
        }
    }

    private CloudTelemetryItem? GetNextItem(bool remove)
    {
        try
        {
            EnsureConnection();

            // Select oldest item by priority (lowest priority value first), then by sequence (FIFO)
            var results = _connection!.Query<TelemetryRecord>(
                "SELECT id, sequence, priority, endpoint, json_data FROM telemetry ORDER BY priority ASC, sequence ASC LIMIT 1");

            if (results.Count > 0)
            {
                var record = results[0];

                // Deserialize
                var obj = MicroJson.Deserialize<Dictionary<string, object>>(record.JsonData);
                if (obj == null)
                {
                    // Corrupted data, remove it
                    if (remove)
                    {
                        DeleteItem(record.Id);
                    }
                    Resolver.Log.Warn($"SQLite: Corrupted item {record.Id}, removed");
                    return null;
                }

                // Remove if requested
                if (remove)
                {
                    DeleteItem(record.Id);
                }

                return new CloudTelemetryItem(obj, record.Endpoint, (CloudTelemetryPriority)record.Priority)
                {
                    ReceptionSequence = record.Sequence
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"SQLite: Failed to retrieve item: {ex.Message}");
            return null;
        }
    }

    private void DeleteItem(long id)
    {
        try
        {
            _connection!.Execute("DELETE FROM telemetry WHERE id = ?", id);
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"SQLite: Failed to delete item {id}: {ex.Message}");
        }
    }

    public Dictionary<int, int> CountByPriority()
    {
        lock (_dbLock)
        {
            try
            {
                EnsureConnection();

                var counts = new Dictionary<int, int>();

                var results = _connection!.Query<PriorityCount>(
                    "SELECT priority, COUNT(*) as count FROM telemetry GROUP BY priority");

                foreach (var result in results)
                {
                    counts[result.Priority] = result.Count;
                }

                return counts;
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"SQLite: Failed to count by priority: {ex.Message}");
                return new Dictionary<int, int>();
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        // Signal shutdown and wait for batch writer to finish
        _cancellationSource.Cancel();

        try
        {
            _batchWriterTask.Wait(5000); // Wait up to 5 seconds
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"SQLite: Error waiting for batch writer shutdown: {ex.Message}");
        }

        lock (_dbLock)
        {
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
        }

        _batchSignal?.Dispose();
        _cancellationSource?.Dispose();

        _disposed = true;

        Resolver.Log.Info("SQLite Telemetry Store disposed");
    }

    // Database record class for InsertAll bulk operations
    [Table("telemetry")]
    private class TelemetryDbRecord
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        public long Sequence { get; set; }
        public int Priority { get; set; }
        public string Endpoint { get; set; } = "";

        [Column("json_data")]
        public string JsonData { get; set; } = "";

        [Column("created_at")]
        public long CreatedAt { get; set; }
    }

    // Helper class for query results
    private class TelemetryRecord
    {
        public long Id { get; set; }
        public long Sequence { get; set; }
        public int Priority { get; set; }
        public string Endpoint { get; set; } = "";

        [Column("json_data")]
        public string JsonData { get; set; } = "";
    }

    private class PriorityCount
    {
        public int Priority { get; set; }
        public int Count { get; set; }
    }
}
