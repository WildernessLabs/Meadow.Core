namespace Meadow;

internal class CloudDataQueue
{
    public const int DefaultQueueDepth = 30;

    public class DataInfo
    {
        public DataInfo(object item, string endpoint)
        {
            Item = item;
            EndPoint = endpoint;
        }

        public object Item { get; set; }
        public string EndPoint { get; set; }
    }

    private readonly CircularBuffer<DataInfo> _items;

    public int Count => _items.Count;
    public int MaxQueueItems { get; }

    public CloudDataQueue(int maxQueueItems = DefaultQueueDepth)
    {
        Resolver.Log.Info($"Cloud Data Queue Depth: {maxQueueItems}");
        MaxQueueItems = maxQueueItems <= 0 ? DefaultQueueDepth : maxQueueItems;
        _items = new CircularBuffer<DataInfo>(maxQueueItems);

        _items.Overrun += OnQueueOverrun;
    }

    private void OnQueueOverrun(object sender, System.EventArgs e)
    {
        // DEV NOTE: don't elevate this above Info or it will become circular/re-entrant
        Resolver.Log.Info($"Cloud Data Queue overrun (data loss)");
    }

    public DataInfo? Peek()
    {
        return _items.Peek();
    }

    public DataInfo? Dequeue()
    {
        return _items.Remove();
    }

    public void Enqueue(DataInfo info)
    {
        _items.Append(info);
    }

    public void Enqueue<T>(T item, string endPoint)
    {
        if (item == null) { return; }

        Enqueue(new DataInfo(item, endPoint));
    }
}
