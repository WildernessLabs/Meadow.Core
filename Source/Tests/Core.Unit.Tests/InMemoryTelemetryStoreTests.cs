using Meadow;
using Meadow.Cloud;
using Meadow.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Core.Unit.Tests;

/// <summary>
/// Tests for InMemoryTelemetryStore
/// </summary>
public class InMemoryTelemetryStoreTests : IDisposable
{
    public InMemoryTelemetryStoreTests()
    {
        // Initialize Resolver.Log to prevent NullReferenceException
        if (Resolver.Log == null)
        {
            try
            {
                Resolver.Services.GetOrCreate<Logger>();
            }
            catch (ArgumentException)
            {
                // Logger already exists, that's fine
            }
        }
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
    [Fact]
    public void InitializesEmpty()
    {
        var store = new InMemoryTelemetryStore();
        Assert.Equal(0, store.Count);
    }

    [Fact]
    public void EnqueueAndDequeueItem()
    {
        var store = new InMemoryTelemetryStore();

        var item = new CloudTelemetryItem(
            new Dictionary<string, object> { { "test", "value" } },
            "/api/test",
            CloudTelemetryPriority.Normal);

        store.Enqueue(item);

        Assert.Equal(1, store.Count);

        var retrieved = store.Dequeue();
        Assert.NotNull(retrieved);
        Assert.Equal("/api/test", retrieved!.EndPoint);
        Assert.Equal(0, store.Count);
    }

    [Fact]
    public void RespectsPriorityOrdering()
    {
        var store = new InMemoryTelemetryStore();

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
        var store = new InMemoryTelemetryStore();

        // Add multiple items with same priority
        for (int i = 0; i < 5; i++)
        {
            store.Enqueue(new CloudTelemetryItem(
                new Dictionary<string, object> { { "index", i } },
                "/api/test",
                CloudTelemetryPriority.Normal));
        }

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
    public void PeekDoesNotRemoveItem()
    {
        var store = new InMemoryTelemetryStore();

        store.Enqueue(new CloudTelemetryItem(
            new Dictionary<string, object> { { "test", "value" } },
            "/api/test",
            CloudTelemetryPriority.Normal));

        var peeked = store.Peek();
        Assert.NotNull(peeked);
        Assert.Equal(1, store.Count);

        var peeked2 = store.Peek();
        Assert.NotNull(peeked2);
        Assert.Equal(1, store.Count);

        var dequeued = store.Dequeue();
        Assert.NotNull(dequeued);
        Assert.Equal(0, store.Count);
    }

    [Fact]
    public void CountByPriorityWorks()
    {
        var store = new InMemoryTelemetryStore();

        store.Enqueue(new CloudTelemetryItem(new Dictionary<string, object>(), "/api/test", CloudTelemetryPriority.High));
        store.Enqueue(new CloudTelemetryItem(new Dictionary<string, object>(), "/api/test", CloudTelemetryPriority.High));
        store.Enqueue(new CloudTelemetryItem(new Dictionary<string, object>(), "/api/test", CloudTelemetryPriority.Normal));
        store.Enqueue(new CloudTelemetryItem(new Dictionary<string, object>(), "/api/test", CloudTelemetryPriority.Low));

        var counts = store.CountByPriority();

        Assert.Equal(2, counts[(int)CloudTelemetryPriority.High]);
        Assert.Equal(1, counts[(int)CloudTelemetryPriority.Normal]);
        Assert.Equal(1, counts[(int)CloudTelemetryPriority.Low]);
    }

    [Fact]
    public void HandlesHighThroughput()
    {
        const int itemCount = 10000;

        // Create store with sufficient capacity for the test
        var store = new InMemoryTelemetryStore(
            highPriorityCapacity: 100,
            normalPriorityCapacity: itemCount,
            lowPriorityCapacity: 100);

        // Rapidly enqueue items
        for (int i = 0; i < itemCount; i++)
        {
            store.Enqueue(new CloudTelemetryItem(
                new Dictionary<string, object> { { "index", i } },
                "/api/test",
                CloudTelemetryPriority.Normal));
        }

        Assert.Equal(itemCount, store.Count);

        // Dequeue all
        int count = 0;
        while (store.Dequeue() != null)
        {
            count++;
        }

        Assert.Equal(itemCount, count);
        Assert.Equal(0, store.Count);
    }

    [Fact]
    public void ReturnsNullWhenEmpty()
    {
        var store = new InMemoryTelemetryStore();

        Assert.Null(store.Peek());
        Assert.Null(store.Dequeue());
    }

    [Fact]
    public void ThreadSafeEnqueueDequeue()
    {
        const int itemsPerThread = 1000;
        const int threadCount = 4;

        // Create store with sufficient capacity for the test
        var store = new InMemoryTelemetryStore(
            highPriorityCapacity: 100,
            normalPriorityCapacity: threadCount * itemsPerThread,
            lowPriorityCapacity: 100);

        // Enqueue from multiple threads
        var enqueueTasks = new Task[threadCount];
        for (int t = 0; t < threadCount; t++)
        {
            int threadId = t;
            enqueueTasks[t] = Task.Run(() =>
            {
                for (int i = 0; i < itemsPerThread; i++)
                {
                    store.Enqueue(new CloudTelemetryItem(
                        new Dictionary<string, object> { { "thread", threadId }, { "index", i } },
                        "/api/test",
                        CloudTelemetryPriority.Normal));
                }
            });
        }

        Task.WaitAll(enqueueTasks);

        Assert.Equal(threadCount * itemsPerThread, store.Count);

        // Dequeue from multiple threads
        int totalDequeued = 0;
        var dequeueTasks = new Task[threadCount];
        for (int t = 0; t < threadCount; t++)
        {
            dequeueTasks[t] = Task.Run(() =>
            {
                int localCount = 0;
                while (store.Dequeue() != null)
                {
                    localCount++;
                }
                Interlocked.Add(ref totalDequeued, localCount);
            });
        }

        Task.WaitAll(dequeueTasks);

        Assert.Equal(threadCount * itemsPerThread, totalDequeued);
        Assert.Equal(0, store.Count);
    }
}
