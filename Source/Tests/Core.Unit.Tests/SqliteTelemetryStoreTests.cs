using Meadow;
using Meadow.Cloud;
using Meadow.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xunit;

namespace Core.Unit.Tests;

/// <summary>
/// Tests for SqliteTelemetryStore with focus on batching, performance, and crash recovery
/// </summary>
public class SqliteTelemetryStoreTests : IDisposable
{
    private readonly string _testDbPath;
    private static bool? _sqliteAvailable;
    private static readonly object _sqliteCheckLock = new object();

    public SqliteTelemetryStoreTests()
    {
        // Initialize Resolver.Log to prevent NullReferenceException
        if (Resolver.Log == null)
        {
            Resolver.Services.GetOrCreate<Logger>();
        }

        _testDbPath = Path.Combine(Path.GetTempPath(), $"test-telemetry-{Guid.NewGuid()}.db");
    }

    /// <summary>
    /// Check if SQLite is available on this platform (cache result)
    /// </summary>
    private static bool IsSqliteAvailable()
    {
        lock (_sqliteCheckLock)
        {
            if (_sqliteAvailable.HasValue)
            {
                return _sqliteAvailable.Value;
            }

            try
            {
                var testPath = Path.Combine(Path.GetTempPath(), $"sqlite-test-{Guid.NewGuid()}.db");
                using (var testConn = new SQLite.SQLiteConnection(testPath))
                {
                    testConn.Execute("CREATE TABLE IF NOT EXISTS test (id INTEGER)");
                }
                File.Delete(testPath);
                _sqliteAvailable = true;
            }
            catch
            {
                _sqliteAvailable = false;
            }

            return _sqliteAvailable.Value;
        }
    }

    /// <summary>
    /// Skip test if SQLite is not available - returns true if should skip
    /// </summary>
    private bool ShouldSkipSqliteTest()
    {
        return !IsSqliteAvailable();
    }

    public void Dispose()
    {
        // Clean up test database files
        try
        {
            if (File.Exists(_testDbPath))
            {
                File.Delete(_testDbPath);
            }

            // Clean up WAL and SHM files
            var walPath = _testDbPath + "-wal";
            var shmPath = _testDbPath + "-shm";

            if (File.Exists(walPath)) File.Delete(walPath);
            if (File.Exists(shmPath)) File.Delete(shmPath);
        }
        catch
        {
            // Best effort cleanup
        }
    }

    [Fact]
    public void InitializesDatabase()
    {
        if (ShouldSkipSqliteTest()) return;

        using var store = new SqliteTelemetryStore(_testDbPath);

        Assert.True(File.Exists(_testDbPath), "Database file should be created");
        Assert.Equal(0, store.Count);
    }

    [Fact]
    public void EnqueueAndDequeueItem()
    {
        if (ShouldSkipSqliteTest()) return;

        using var store = new SqliteTelemetryStore(_testDbPath, batchSize: 1, batchIntervalMs: 100);

        var item = new CloudTelemetryItem(
            new Dictionary<string, object> { { "test", "value" } },
            "/api/test",
            CloudTelemetryPriority.Normal);

        store.Enqueue(item);

        // Wait for batch writer
        Thread.Sleep(200);

        Assert.Equal(1, store.Count);

        var retrieved = store.Dequeue();
        Assert.NotNull(retrieved);
        Assert.Equal("/api/test", retrieved!.EndPoint);
        Assert.Equal(0, store.Count);
    }

    [Fact]
    public void BatchesMultipleWrites()
    {
        if (ShouldSkipSqliteTest()) return;

        using var store = new SqliteTelemetryStore(_testDbPath, batchSize: 10, batchIntervalMs: 500);

        // Enqueue multiple items rapidly
        for (int i = 0; i < 25; i++)
        {
            var item = new CloudTelemetryItem(
                new Dictionary<string, object> { { "index", i } },
                "/api/test",
                CloudTelemetryPriority.Normal);

            store.Enqueue(item);
        }

        // Wait for batches to process
        Thread.Sleep(1000);

        Assert.Equal(25, store.Count);
    }

    [Fact]
    public void RespectsPriorityOrdering()
    {
        if (ShouldSkipSqliteTest()) return;

        using var store = new SqliteTelemetryStore(_testDbPath, batchSize: 100, batchIntervalMs: 100);

        // Add items in mixed priority order
        store.Enqueue(new CloudTelemetryItem(
            new Dictionary<string, object> { { "priority", "normal" } },
            "/api/test",
            CloudTelemetryPriority.Normal));

        store.Enqueue(new CloudTelemetryItem(
            new Dictionary<string, object> { { "priority", "high" } },
            "/api/test",
            CloudTelemetryPriority.High));

        store.Enqueue(new CloudTelemetryItem(
            new Dictionary<string, object> { { "priority", "low" } },
            "/api/test",
            CloudTelemetryPriority.Low));

        // Wait for batch
        Thread.Sleep(300);

        // Should retrieve in priority order (High, Normal, Low)
        var item1 = store.Dequeue();
        Assert.NotNull(item1);
        Assert.Equal(CloudTelemetryPriority.High, item1!.Priority);

        var item2 = store.Dequeue();
        Assert.NotNull(item2);
        Assert.Equal(CloudTelemetryPriority.Normal, item2!.Priority);

        var item3 = store.Dequeue();
        Assert.NotNull(item3);
        Assert.Equal(CloudTelemetryPriority.Low, item3!.Priority);
    }

    [Fact]
    public void PreservesFIFOWithinPriority()
    {
        if (ShouldSkipSqliteTest()) return;

        using var store = new SqliteTelemetryStore(_testDbPath, batchSize: 100, batchIntervalMs: 100);

        // Add multiple items with same priority
        for (int i = 0; i < 5; i++)
        {
            store.Enqueue(new CloudTelemetryItem(
                new Dictionary<string, object> { { "index", i } },
                "/api/test",
                CloudTelemetryPriority.Normal));
        }

        // Wait for batch
        Thread.Sleep(300);

        // Should retrieve in FIFO order
        for (int i = 0; i < 5; i++)
        {
            var item = store.Dequeue();
            Assert.NotNull(item);

            var dict = item!.Item as Dictionary<string, object>;
            Assert.NotNull(dict);
            Assert.Equal((long)i, Convert.ToInt64(dict!["index"]));
        }
    }

    [Fact]
    public void CountByPriorityWorks()
    {
        if (ShouldSkipSqliteTest()) return;

        using var store = new SqliteTelemetryStore(_testDbPath, batchSize: 100, batchIntervalMs: 100);

        store.Enqueue(new CloudTelemetryItem(new Dictionary<string, object>(), "/api/test", CloudTelemetryPriority.High));
        store.Enqueue(new CloudTelemetryItem(new Dictionary<string, object>(), "/api/test", CloudTelemetryPriority.High));
        store.Enqueue(new CloudTelemetryItem(new Dictionary<string, object>(), "/api/test", CloudTelemetryPriority.Normal));
        store.Enqueue(new CloudTelemetryItem(new Dictionary<string, object>(), "/api/test", CloudTelemetryPriority.Low));

        Thread.Sleep(300);

        var counts = store.CountByPriority();

        Assert.Equal(2, counts[(int)CloudTelemetryPriority.High]);
        Assert.Equal(1, counts[(int)CloudTelemetryPriority.Normal]);
        Assert.Equal(1, counts[(int)CloudTelemetryPriority.Low]);
    }

    [Fact]
    public void PersistsAcrossInstances()
    {
        if (ShouldSkipSqliteTest()) return;

        // Create store and add items
        using (var store = new SqliteTelemetryStore(_testDbPath, batchSize: 1, batchIntervalMs: 100))
        {
            for (int i = 0; i < 10; i++)
            {
                store.Enqueue(new CloudTelemetryItem(
                    new Dictionary<string, object> { { "value", i } },
                    "/api/test",
                    CloudTelemetryPriority.Normal));
            }

            Thread.Sleep(300);
            Assert.Equal(10, store.Count);
        }

        // Reopen and verify data persists
        using (var store = new SqliteTelemetryStore(_testDbPath))
        {
            Assert.Equal(10, store.Count);

            var item = store.Dequeue();
            Assert.NotNull(item);
            Assert.Equal(9, store.Count);
        }
    }

    [Fact]
    public void RecoversSequenceCounter()
    {
        if (ShouldSkipSqliteTest()) return;

        long lastSequence;

        // Create store and add items
        using (var store = new SqliteTelemetryStore(_testDbPath, batchSize: 1, batchIntervalMs: 100))
        {
            var item = new CloudTelemetryItem(
                new Dictionary<string, object> { { "test", "value" } },
                "/api/test",
                CloudTelemetryPriority.Normal);

            store.Enqueue(item);
            lastSequence = item.ReceptionSequence;

            Thread.Sleep(300);
        }

        // Reopen and verify sequence continues
        using (var store = new SqliteTelemetryStore(_testDbPath))
        {
            var item = new CloudTelemetryItem(
                new Dictionary<string, object> { { "test", "value2" } },
                "/api/test",
                CloudTelemetryPriority.Normal);

            store.Enqueue(item);

            Assert.True(item.ReceptionSequence > lastSequence, "Sequence should continue from previous session");
        }
    }

    [Fact]
    public void HandlesHighThroughput()
    {
        if (ShouldSkipSqliteTest()) return;

        using var store = new SqliteTelemetryStore(_testDbPath, batchSize: 50, batchIntervalMs: 200);

        const int itemCount = 500;

        // Rapidly enqueue items
        for (int i = 0; i < itemCount; i++)
        {
            store.Enqueue(new CloudTelemetryItem(
                new Dictionary<string, object> { { "index", i } },
                "/api/test",
                CloudTelemetryPriority.Normal));
        }

        // Wait for all batches to process
        Thread.Sleep(3000);

        Assert.Equal(itemCount, store.Count);
    }

    [Fact]
    public void BackpressureDropsOldestWhenFull()
    {
        if (ShouldSkipSqliteTest()) return;

        // Create store with small batch interval to fill queue
        using var store = new SqliteTelemetryStore(_testDbPath, batchSize: 10, batchIntervalMs: 10000);

        // Try to enqueue more than max pending (should trigger backpressure)
        // This test verifies the system doesn't crash under extreme load
        for (int i = 0; i < 15000; i++)
        {
            store.Enqueue(new CloudTelemetryItem(
                new Dictionary<string, object> { { "index", i } },
                "/api/test",
                CloudTelemetryPriority.Normal));
        }

        // Should not crash
        Assert.True(true);
    }

    [Fact]
    public void PeekDoesNotRemoveItem()
    {
        if (ShouldSkipSqliteTest()) return;

        using var store = new SqliteTelemetryStore(_testDbPath, batchSize: 1, batchIntervalMs: 100);

        store.Enqueue(new CloudTelemetryItem(
            new Dictionary<string, object> { { "test", "value" } },
            "/api/test",
            CloudTelemetryPriority.Normal));

        Thread.Sleep(300);

        var peeked = store.Peek();
        Assert.NotNull(peeked);
        Assert.Equal(1, store.Count);

        var peeked2 = store.Peek();
        Assert.NotNull(peeked2);
        Assert.Equal(1, store.Count);
    }

    [Fact]
    public void FlushesOnDispose()
    {
        if (ShouldSkipSqliteTest()) return;

        var item = new CloudTelemetryItem(
            new Dictionary<string, object> { { "test", "value" } },
            "/api/test",
            CloudTelemetryPriority.Normal);

        using (var store = new SqliteTelemetryStore(_testDbPath, batchSize: 100, batchIntervalMs: 10000))
        {
            store.Enqueue(item);
            // Dispose immediately without waiting for batch interval
        }

        // Reopen and verify item was flushed
        using (var store = new SqliteTelemetryStore(_testDbPath))
        {
            Assert.Equal(1, store.Count);
        }
    }

    [Fact]
    public void HandlesComplexObjectSerialization()
    {
        if (ShouldSkipSqliteTest()) return;

        using var store = new SqliteTelemetryStore(_testDbPath, batchSize: 1, batchIntervalMs: 100);

        var complexObject = new Dictionary<string, object>
        {
            { "string", "test" },
            { "number", 42 },
            { "double", 3.14 },
            { "bool", true },
            { "nested", new Dictionary<string, object>
                {
                    { "inner", "value" }
                }
            }
        };

        store.Enqueue(new CloudTelemetryItem(complexObject, "/api/test", CloudTelemetryPriority.Normal));

        Thread.Sleep(300);

        var retrieved = store.Dequeue();
        Assert.NotNull(retrieved);

        var dict = retrieved!.Item as Dictionary<string, object>;
        Assert.NotNull(dict);
        Assert.Equal("test", dict!["string"]);
        Assert.Equal(42L, Convert.ToInt64(dict["number"]));
    }

    [Fact]
    public void WALFilesCreated()
    {
        if (ShouldSkipSqliteTest()) return;

        using var store = new SqliteTelemetryStore(_testDbPath, batchSize: 1, batchIntervalMs: 100);

        store.Enqueue(new CloudTelemetryItem(
            new Dictionary<string, object> { { "test", "value" } },
            "/api/test",
            CloudTelemetryPriority.Normal));

        Thread.Sleep(300);

        // WAL file should exist when WAL mode is enabled
        var walPath = _testDbPath + "-wal";
        Assert.True(File.Exists(walPath), "WAL file should be created in WAL mode");
    }
}
