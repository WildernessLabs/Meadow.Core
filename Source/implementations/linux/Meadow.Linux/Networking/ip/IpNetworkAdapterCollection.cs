using Meadow.Hardware;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meadow.Networking;

/// <summary>
/// Provides a collection of network adapters using the ip command
/// </summary>
public class IpNetworkAdapterCollection : INetworkAdapterCollection
{
#pragma warning disable CS0067 // The event is never used
    /// <inheritdoc/>
    public event NetworkConnectionHandler? NetworkConnected;
    /// <inheritdoc/>
    public event NetworkDisconnectionHandler? NetworkDisconnected;
#pragma warning restore CS0067

    private List<IpNetworkAdapter> _adapters = new();

    /// <inheritdoc/>
    public INetworkAdapter this[int index] => _adapters[index];

    internal IpNetworkAdapterCollection()
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

        foreach (var d in IpCommand.GetDevices())
        {
            // Skip loopback interface
            if (d.Type == "loopback")
            {
                continue;
            }

            if (d.Type == "wifi")
            {
                _adapters.Add(new IpWiFiNetworkAdapter(d));
            }
            else
            {
                _adapters.Add(new IpNetworkAdapter(d));
            }
        }

        return Task.CompletedTask;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
