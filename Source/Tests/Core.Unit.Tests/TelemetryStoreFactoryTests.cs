using Meadow.Cloud;

namespace Core.Unit.Tests;

/// <summary>
/// Tests for TelemetryStoreFactory
/// </summary>
public class TelemetryStoreFactoryTests : IDisposable
{
    private readonly string _testDir;

    public TelemetryStoreFactoryTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"telemetry-factory-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }
        catch
        {
            // Best effort cleanup
        }
    }

    [Fact]
    public void CreatesInMemoryStore()
    {
        var store = TelemetryStoreFactory.Create(TelemetryStoreType.InMemory);

        Assert.NotNull(store);
        Assert.IsType<InMemoryTelemetryStore>(store);
        Assert.Equal(0, store.Count);
    }

    [Fact]
    public void CreatesTalusDBStore()
    {
        var store = TelemetryStoreFactory.Create(TelemetryStoreType.TalusDB, _testDir);

        Assert.NotNull(store);
        Assert.IsType<TalusTelemetryStore>(store);
        Assert.Equal(0, store.Count);

        (store as IDisposable)?.Dispose();
    }

    [Fact]
    public void CreatesSQLiteStore()
    {
        var store = TelemetryStoreFactory.Create(TelemetryStoreType.SQLite, _testDir);

        Assert.NotNull(store);
        Assert.IsType<SqliteTelemetryStore>(store);
        Assert.Equal(0, store.Count);

        (store as IDisposable)?.Dispose();
    }

    [Fact]
    public void SQLiteStoreSupportsCustomBatchSettings()
    {
        var store = TelemetryStoreFactory.Create(
            TelemetryStoreType.SQLite,
            _testDir,
            sqliteBatchSize: 100,
            sqliteBatchIntervalMs: 2000);

        Assert.NotNull(store);
        Assert.IsType<SqliteTelemetryStore>(store);

        (store as IDisposable)?.Dispose();
    }

    [Fact]
    public void AllStoresImplementInterface()
    {
        var inMemory = TelemetryStoreFactory.Create(TelemetryStoreType.InMemory);
        var talusDB = TelemetryStoreFactory.Create(TelemetryStoreType.TalusDB, _testDir);
        var sqlite = TelemetryStoreFactory.Create(TelemetryStoreType.SQLite, _testDir);

        Assert.IsAssignableFrom<IMeadowCloudTelemetryStore>(inMemory);
        Assert.IsAssignableFrom<IMeadowCloudTelemetryStore>(talusDB);
        Assert.IsAssignableFrom<IMeadowCloudTelemetryStore>(sqlite);

        (talusDB as IDisposable)?.Dispose();
        (sqlite as IDisposable)?.Dispose();
    }

    [Fact]
    public void AllStoresHaveSameBehavior()
    {
        var stores = new IMeadowCloudTelemetryStore[]
        {
            TelemetryStoreFactory.Create(TelemetryStoreType.InMemory),
            TelemetryStoreFactory.Create(TelemetryStoreType.TalusDB, _testDir),
            TelemetryStoreFactory.Create(TelemetryStoreType.SQLite, _testDir)
        };

        foreach (var store in stores)
        {
            // Test basic operations
            store.Enqueue(new CloudTelemetryItem(
                new Dictionary<string, object> { { "test", "value" } },
                "/api/test",
                CloudTelemetryPriority.High));

            store.Enqueue(new CloudTelemetryItem(
                new Dictionary<string, object> { { "test", "value" } },
                "/api/test",
                CloudTelemetryPriority.Normal));

            // SQLite needs time to batch
            if (store is SqliteTelemetryStore)
            {
                Thread.Sleep(300);
            }

            Assert.Equal(2, store.Count);

            var item = store.Dequeue();
            Assert.NotNull(item);
            Assert.Equal(CloudTelemetryPriority.High, item!.Priority);

            Assert.Equal(1, store.Count);

            (store as IDisposable)?.Dispose();
        }
    }

    [Fact]
    public void AutoDetectReturnsValidStore()
    {
        var store = TelemetryStoreFactory.CreateAutoDetect(_testDir);

        Assert.NotNull(store);
        Assert.IsAssignableFrom<IMeadowCloudTelemetryStore>(store);

        (store as IDisposable)?.Dispose();
    }
}
