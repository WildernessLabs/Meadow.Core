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
    public event NetworkConnectionHandler NetworkConnected;
    public event NetworkDisconnectionHandler NetworkDisconnected;

    private List<NmCliNetworkAdapter> _adapters = new();

    public INetworkAdapter this[int index] => throw new NotImplementedException();

    internal NmCliNetworkAdapterCollection()
    {
        Refresh().Wait();
    }

    public IEnumerator<INetworkAdapter> GetEnumerator()
    {
        return _adapters.GetEnumerator();
    }

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
