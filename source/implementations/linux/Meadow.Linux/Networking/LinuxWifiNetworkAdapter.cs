using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

/// <summary>
/// Encapsulates WiFi functionality specific to Linux platforms
/// </summary>
public class LinuxWifiNetworkAdapter : NetworkAdapterBase, IWirelessNetworkAdapter, IWiFiNetworkAdapter
{
    private NetworkInterface _networkInterface;

    /// <summary>
    /// Creates a WindowsWifiNetworkAdapter
    /// </summary>
    /// <param name="ni">The managed NetworkInterface describing the adapter</param>
    public LinuxWifiNetworkAdapter(NetworkInterface ni)
        : base(NetworkInterfaceType.Wireless80211)
    {
        _networkInterface = ni;
    }
    /// <inheritdoc/>
    public Task<IList<WifiNetwork>> Scan(CancellationToken token)
    {
        return Task.FromResult(Scan() as IList<WifiNetwork>);
    }

    /// <inheritdoc/>
    public Task<IList<WifiNetwork>> Scan(TimeSpan timeout)
    {
        return Task.FromResult(Scan() as IList<WifiNetwork>);
    }

    private List<WifiNetwork> Scan()
    {
        return Array.Empty<WifiNetwork>().ToList();
    }

    /// <inheritdoc/>
    public Task Connect(string ssid, string password, TimeSpan timeout, CancellationToken token, ReconnectionType reconnection = ReconnectionType.Manual)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task Disconnect(bool turnOffWiFiInterface)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public string? Ssid => throw new NotImplementedException();

    /// <inheritdoc/>
    public PhysicalAddress Bssid => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool AutoConnect => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool AutoReconnect => throw new NotImplementedException();

    /// <inheritdoc/>
    public string DefaultSsid => throw new NotImplementedException();

    /// <inheritdoc/>
    public int Channel => throw new NotImplementedException();

    /// <inheritdoc/>
    public AntennaType CurrentAntenna => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task ClearStoredAccessPointInformation()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task ConnectToDefaultAccessPoint(TimeSpan timeout, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetAntenna(AntennaType antenna, bool persist = true)
    {
        throw new NotImplementedException();
    }
}
