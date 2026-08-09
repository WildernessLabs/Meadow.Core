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
    private const int MaxPendingItems = 800;
    private const int DefaultMaxDbItems = 1000;
    private const int CleanupBatchSize = 50;
    private const int DbLockTimeoutMs = 10000;

    private const string LogGroup = "SqliteTelemetryStore";

    private readonly string _databasePath;
    private readonly ConcurrentQueue<CloudTelemetryItem> _pendingWrites;
    private readonly SemaphoreSlim _batchSignal;
    private readonly SemaphoreSlim _cleanupSignal;
    private readonly CancellationTokenSource _cancellationSource;
    private readonly Task _batchWriterTask;
    private readonly Task _cleanupTask;
    private readonly Lock _dbLock = new();
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
            if (!_dbLock.TryEnter(DbLockTimeoutMs))
            {
                Resolver.Log.Warn($"Count: Lock acquisition timeout after {DbLockTimeoutMs}ms", LogGroup);
                return -1;
            }

            try
            {
                EnsureConnection();
                return _connection!.ExecuteScalar<int>("SELECT COUNT(*) FROM telemetry");
            }
            finally
            {
                _dbLock.Exit();
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
        _cleanupSignal = new SemaphoreSlim(0);
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

        // Warm up the ORM paths now, while memory is plentiful. The first
        // InsertAll/Query<T> otherwise JIT-compiles sqlite-net's
        // reflection-heavy mapping at first batch write -- which lands at
        // MQTT-connect time, on top of the TLS + connect JIT spike, and has
        // been observed to exhaust the F7's native arena (fatal ~475KB
        // g_malloc failure). Boot-time warm-up moves that cost to a moment
        // with megabytes of headroom.
        WarmUpOrmPaths();

        // Start background batch writer
        _batchWriterTask = Task.Run(BatchWriterLoop, _cancellationSource.Token);

        // Start background cleanup task
        _cleanupTask = Task.Run(CleanupLoop, _cancellationSource.Token);

        Resolver.Log.Info($"Telemetry Store initialized at {_databasePath} (batch size: {_batchSize}, interval: {_batchIntervalMs}ms, max items: {_maxDbItems})", LogGroup);
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

                // Bound the page cache to 1MB. The default request here used to
                // be 10MB, which on the F7's ~29MB native arena is a large
                // fraction of total memory for a ~1000-row telemetry table;
                // sqlite grows the cache on demand under load, competing with
                // the JIT/TLS allocation spikes that already run the arena
                // close to exhaustion.
                _connection.Execute("PRAGMA cache_size=-1024");

                // verify the settings we just sent
                var checks = new string[] { "journal_mode", "synchronous", "cache_size" };
                foreach (var check in checks)
                {
                    var pragma = _connection!.ExecuteScalar<string?>($"PRAGMA {check}");

                    if (pragma is not null)
                    {
                        Resolver.Log.Info($"check: {pragma}", LogGroup);
                    }
                }

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

                Resolver.Log.Info("Database initialized successfully", LogGroup);
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to initialize SQLite database: {ex.Message}", LogGroup);
                throw;
            }
        }
    }

    private void WarmUpOrmPaths()
    {
        lock (_dbLock)
        {
            try
            {
                EnsureConnection();

                var warmup = new TelemetryDbRecord
                {
                    Sequence = 0,
                    Priority = int.MaxValue,
                    Endpoint = "__warmup__",
                    JsonData = "{}",
                    CreatedAt = 0
                };
                _connection!.InsertAll(new[] { warmup });
                _connection.Query<TelemetryRecord>(
                    "SELECT * FROM telemetry WHERE endpoint = ? LIMIT 1", "__warmup__");
                _connection.Execute("DELETE FROM telemetry WHERE endpoint = ?", "__warmup__");
            }
            catch (Exception ex)
            {
                // Warm-up is an optimization; never fail construction over it.
                Resolver.Log.Warn($"ORM warm-up failed: {ex.Message}", LogGroup);
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
                    Resolver.Log.Info($"Recovered sequence counter: {_nextSequence}", LogGroup);
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to recover sequence counter: {ex.Message}", LogGroup);
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
            Resolver.Log.Warn($"Pending queue full ({_pendingCount} items), dropping oldest", LogGroup);
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
                Resolver.Log.Error($"Batch writer error: {ex.Message}", LogGroup);
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

    private async Task CleanupLoop()
    {
        while (!_cancellationSource.Token.IsCancellationRequested)
        {
            try
            {
                // Wait for cleanup signal (or timeout every 30 seconds to check)
                await _cleanupSignal.WaitAsync(30000, _cancellationSource.Token);

                // Keep deleting batches until we're under the limit
                bool needsMoreCleanup = true;
                while (needsMoreCleanup && !_cancellationSource.Token.IsCancellationRequested)
                {
                    int currentCount = 0;
                    int deleted = 0;

                    if (!_dbLock.TryEnter(DbLockTimeoutMs))
                    {
                        Resolver.Log.Warn($"CleanupLoop: Lock acquisition timeout after {DbLockTimeoutMs}ms, skipping cleanup cycle", LogGroup);
                        needsMoreCleanup = false;
                        continue;
                    }

                    try
                    {
                        EnsureConnection();

                        Resolver.Log.Trace($"Cleanup: max items {_maxDbItems})", LogGroup);

                        // Check current count
                        currentCount = _connection!.ExecuteScalar<int>("SELECT COUNT(*) FROM telemetry");

                        Resolver.Log.Trace($"Cleanup: current count {currentCount})", LogGroup);

                        if (currentCount > _maxDbItems)
                        {
                            // Delete one batch of oldest items
                            _connection.Execute(@"
                                DELETE FROM telemetry
                                WHERE id IN (
                                    SELECT id FROM telemetry
                                    ORDER BY priority DESC, sequence ASC
                                    LIMIT ?
                                )", CleanupBatchSize);

                            deleted = CleanupBatchSize;
                            Resolver.Log.Trace($"Cleanup: Deleted batch of {deleted} items (count was {currentCount})", LogGroup);
                        }
                        else
                        {
                            needsMoreCleanup = false;
                            Resolver.Log.Trace($"Cleanup: Nothing to delete (count: {currentCount})", LogGroup);
                        }
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Error($"Cleanup: Error during delete: {ex.Message}", LogGroup);
                        needsMoreCleanup = false;
                    }
                    finally
                    {
                        _dbLock.Exit();
                    }

                    // Release lock between batches to allow other operations
                    if (needsMoreCleanup)
                    {
                        await Task.Delay(100); // Small delay between batches
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected during shutdown
                break;
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Cleanup loop error: {ex.Message}", LogGroup);
                await Task.Delay(5000); // Back off on error
            }
        }
    }

    private void WriteBatch(List<CloudTelemetryItem> items)
    {
        if (items.Count == 0) return;

        if (!_dbLock.TryEnter(DbLockTimeoutMs))
        {
            Resolver.Log.Warn($"WriteBatch: Lock acquisition timeout after {DbLockTimeoutMs}ms, skipping batch of {items.Count} items", LogGroup);
            return;
        }

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
                    Resolver.Log.Error($"Failed to serialize item: {ex.Message}", LogGroup);
                }
            }

            if (records.Count > 0)
            {
                // Use InsertAll for true bulk insert - much faster than individual inserts
                _connection!.InsertAll(records);

                // Check if we've exceeded max capacity - signal cleanup task if needed
                var currentCount = _connection.ExecuteScalar<int>("SELECT COUNT(*) FROM telemetry");
                if (currentCount > _maxDbItems)
                {
                    // Signal cleanup task to delete in background
                    _cleanupSignal.Release();
                    Resolver.Log.Trace($"Signaled cleanup task (count: {currentCount}, max: {_maxDbItems})", LogGroup);
                }

                if (records.Count > 1)
                {
                    Resolver.Log.Trace($"Wrote batch of {records.Count} items", LogGroup);
                }
            }
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Batch write failed: {ex.Message}", LogGroup);
        }
        finally
        {
            _dbLock.Exit();
        }
    }

    public CloudTelemetryItem? Peek()
    {
        if (!_dbLock.TryEnter(DbLockTimeoutMs))
        {
            Resolver.Log.Warn($"Peek: Lock acquisition timeout after {DbLockTimeoutMs}ms", LogGroup);
            return null;
        }

        try
        {
            return GetNextItem(remove: false);
        }
        finally
        {
            _dbLock.Exit();
        }
    }

    public CloudTelemetryItem? Dequeue()
    {
        if (!_dbLock.TryEnter(DbLockTimeoutMs))
        {
            Resolver.Log.Warn($"Dequeue: Lock acquisition timeout after {DbLockTimeoutMs}ms", LogGroup);
            return null;
        }

        try
        {
            return GetNextItem(remove: true);
        }
        finally
        {
            _dbLock.Exit();
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
                    Resolver.Log.Warn($"Corrupted item {record.Id}, removed", LogGroup);
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
            Resolver.Log.Error($"Failed to retrieve item: {ex.Message}", LogGroup);
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
            Resolver.Log.Error($"Failed to delete item {id}: {ex.Message}", LogGroup);
        }
    }

    public Dictionary<int, int> CountByPriority()
    {
        if (!_dbLock.TryEnter(DbLockTimeoutMs))
        {
            Resolver.Log.Warn($"CountByPriority: Lock acquisition timeout after {DbLockTimeoutMs}ms", LogGroup);
            return new Dictionary<int, int>();
        }

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
            Resolver.Log.Error($"Failed to count by priority: {ex.Message}", LogGroup);
            return new Dictionary<int, int>();
        }
        finally
        {
            _dbLock.Exit();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        // Signal shutdown and wait for background tasks to finish
        _cancellationSource.Cancel();

        try
        {
            _batchWriterTask.Wait(5000); // Wait up to 5 seconds
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Error waiting for batch writer shutdown: {ex.Message}", LogGroup);
        }

        try
        {
            _cleanupTask.Wait(5000); // Wait up to 5 seconds
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Error waiting for cleanup task shutdown: {ex.Message}", LogGroup);
        }

        if (!_dbLock.TryEnter(10000)) // Longer timeout for dispose
        {
            Resolver.Log.Error($"Dispose: Lock acquisition timeout after 10000ms, forcing disposal", LogGroup);
        }
        else
        {
            try
            {
                _connection?.Close();
                _connection?.Dispose();
                _connection = null;
            }
            finally
            {
                _dbLock.Exit();
            }
        }

        _batchSignal?.Dispose();
        _cleanupSignal?.Dispose();
        _cancellationSource?.Dispose();

        _disposed = true;

        Resolver.Log.Info("Telemetry Store disposed", LogGroup);
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
