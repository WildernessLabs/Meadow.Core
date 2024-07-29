using Meadow.Hardware;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meadow.Networking;

/// <summary>
/// Provides a collection of network adapters from the Linux Network manager
/// </summary>
public class NmCliNetworkAdapterCollection : INetworkAdapterCollection
{
#pragma warning disable CS0067 // The event 'NmCliNetworkAdapter.NetworkConnecting' is never used
    /// <inheritdoc/>
    public event NetworkConnectionHandler? NetworkConnected;
    /// <inheritdoc/>
    public event NetworkDisconnectionHandler? NetworkDisconnected;
#pragma warning restore CS0067 // The event 'NmCliNetworkAdapter.NetworkConnecting' is never used

    private List<NmCliNetworkAdapter> _adapters = new();

    /// <inheritdoc/>
    public INetworkAdapter this[int index] => throw new NotImplementedException();

    internal NmCliNetworkAdapterCollection()
    {
        Refresh().Wait();
    }

    /// <inheritdoc/>
    public IEnumerator<INetworkAdapter> GetEnumerator()
    {
        return _adapters.GetEnumerator();
    }

    /// <inheritdoc/>
    public Task Refresh()
    {
        _adapters.Clear();

        foreach (var d in NmCli.GetDevices())
        {
            if (d.Type == "wifi")
            {
                _adapters.Add(new NmCliWiFiNetworkAdapter(d));
            }
            else
            {
                _adapters.Add(new NmCliNetworkAdapter(d));
            }
        }

        return Task.CompletedTask;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
