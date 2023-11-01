using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meadow;

/// <summary>
/// A collection of INetworkAdapter-derived instances
/// </summary>
public class NetworkAdapterCollection : INetworkAdapterCollection
{
    /// <inheritdoc/>
    public event NetworkConnectionHandler NetworkConnected = default!;
    /// <inheritdoc/>
    public event NetworkDisconnectionHandler NetworkDisconnected = default!;

    private List<INetworkAdapter> _adapters = new List<INetworkAdapter>();

    /// <inheritdoc/>
    public int Count => _adapters.Count;

    /// <summary>
    /// Gets an INetworkAdapter from the collection by position index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public INetworkAdapter this[int index] => _adapters[index];

    /// <summary>
    /// Adds an INetworkAdapter to the collection
    /// </summary>
    /// <param name="adapter"></param>
    public void Add(INetworkAdapter adapter)
    {
        _adapters.Add(adapter);

        adapter.NetworkConnected += (s, e) => NetworkConnected.Invoke(s, e);
        adapter.NetworkDisconnected += (s) => NetworkDisconnected.Invoke(s);
    }

    /// <summary>
    /// Override this method to refresh the collection
    /// </summary>
    public virtual Task Refresh() { return Task.CompletedTask; }

    /// <summary>
    /// Enumerates all INetworkAdapters in the collection
    /// </summary>
    /// <returns></returns>
    public IEnumerator<INetworkAdapter> GetEnumerator()
    {
        return _adapters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
