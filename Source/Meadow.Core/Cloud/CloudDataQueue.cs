using System.Collections.Generic;

namespace Meadow;

internal class CloudDataQueue
{
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

    private Queue<DataInfo> _items = new();

    public int Count => _items.Count;
    public int MaxQueueItems { get; }

    public CloudDataQueue(int maxQueueItems = 30)
    {
        MaxQueueItems = maxQueueItems;
    }

    public DataInfo? Peek()
    {
        lock (_items)
        {
            if (_items.Count == 0) { return null; }

            return _items.Peek();
        }
    }

    public DataInfo? Dequeue()
    {
        lock (_items)
        {
            if (_items.Count == 0) { return null; }

            return _items.Dequeue();
        }
    }

    public void Enqueue(DataInfo info)
    {
        lock (_items)
        {
            _items.Enqueue(info);

            while (_items.Count > MaxQueueItems)
            {
                // prevent OOMs
                _items.Dequeue();
            }
        }
    }

    public void Enqueue<T>(T item, string endPoint)
    {
        if (item == null) { return; }

        Enqueue(new DataInfo(item, endPoint));
    }
}
