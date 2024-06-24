using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Meadow.Devices;

/// <summary>
/// Provides a collection of .NET native network adapters
/// </summary>
public class NativeNetworkAdapterCollection : INetworkAdapterCollection
{
    /// <inheritdoc/>
    public event NetworkConnectionHandler NetworkConnected = delegate { };
    /// <inheritdoc/>
    public event NetworkDisconnectionHandler NetworkDisconnected = delegate { };

    // DEV NOTE: This collection is not used in the F7 - it is used by Linux and Windows
    //           For F7 implementations, see the NetworkAdapterCollection class
    private readonly List<INetworkAdapter> _adapters = new List<INetworkAdapter>();

    /// <summary>
    /// Gets an INetworkAdapter from the collection at a specified index
    /// </summary>
    /// <param name="index">The index of the adapter to retrieve</param>
    public INetworkAdapter this[int index] => _adapters[index];

    /// <summary>
    /// Creates a NativeNetworkAdapterCollection
    /// </summary>
    public NativeNetworkAdapterCollection()
    {
        Refresh();
    }

    /// <summary>
    /// Gets an enumerator for the collection
    /// </summary>
    public IEnumerator<INetworkAdapter> GetEnumerator()
    {
        return _adapters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Refreshes the collection
    /// </summary>
    public Task Refresh()
    {
        lock (_adapters)
        {
            _adapters.Clear();

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    switch (ni.NetworkInterfaceType)
                    {
                        case NetworkInterfaceType.Ethernet:
                            _adapters.Add(GetWiredNetworkAdapter(ni));
                            break;
                        case NetworkInterfaceType.Wireless80211:
                            _adapters.Add(GetWiFiNetworkAdapter(ni));
                            break;
                    }
                }
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates an IWiredNetworkAdapter from a NetworkInterface 
    /// </summary>
    /// <param name="ni">The NetworkInterface for the adapter</param>
    public virtual IWiredNetworkAdapter GetWiredNetworkAdapter(NetworkInterface ni)
    {
        return new WiredNetworkAdapter(ni);
    }

    /// <summary>
    /// Creates an IWiFiNetworkAdapter from a NetworkInterface 
    /// </summary>
    /// <param name="ni">The NetworkInterface for the adapter</param>
    public virtual IWiFiNetworkAdapter GetWiFiNetworkAdapter(NetworkInterface ni)
    {
        return new WiFiNetworkAdapter(ni);
    }
}
