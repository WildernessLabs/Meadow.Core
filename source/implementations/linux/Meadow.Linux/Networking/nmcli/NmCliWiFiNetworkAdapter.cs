using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Networking;

/// <summary>
/// Represents a WiFi network adapter managed by NetworkManager using nmcli.
/// </summary>
public class NmCliWiFiNetworkAdapter : NmCliNetworkAdapter, IWiFiNetworkAdapter
{
    internal NmCliWiFiNetworkAdapter(NmCliDevice nmCliDevice)
        : base(nmCliDevice)
    {
    }

    /// <inheritdoc/>
    public Task<IList<WifiNetwork>> Scan(CancellationToken token)
    {
        var result = NmCli.GetWirelessNetworksInfo();
        return Task.FromResult(result as IList<WifiNetwork>);
    }

    /// <inheritdoc/>
    public Task<IList<WifiNetwork>> Scan(TimeSpan timeout)
    {
        var result = NmCli.GetWirelessNetworksInfo();
        return Task.FromResult(result as IList<WifiNetwork>);
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
    public Task Connect(string ssid, string password, TimeSpan timeout, CancellationToken token, ReconnectionType reconnection = ReconnectionType.Automatic)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task ConnectToDefaultAccessPoint(TimeSpan timeout, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task Disconnect(bool turnOffWiFiInterface)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetAntenna(AntennaType antenna, bool persist = true)
    {
        throw new NotImplementedException();
    }
}
