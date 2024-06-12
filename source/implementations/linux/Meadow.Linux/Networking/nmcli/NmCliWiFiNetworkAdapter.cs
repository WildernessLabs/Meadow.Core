using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Networking;

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

    public string? Ssid => throw new NotImplementedException();

    public PhysicalAddress Bssid => throw new NotImplementedException();

    public bool AutoConnect => throw new NotImplementedException();

    public bool AutoReconnect => throw new NotImplementedException();

    public string DefaultSsid => throw new NotImplementedException();

    public int Channel => throw new NotImplementedException();

    public AntennaType CurrentAntenna => throw new NotImplementedException();

    public Task ClearStoredAccessPointInformation()
    {
        throw new NotImplementedException();
    }

    public Task Connect(string ssid, string password, TimeSpan timeout, CancellationToken token, ReconnectionType reconnection = ReconnectionType.Automatic)
    {
        throw new NotImplementedException();
    }

    public Task ConnectToDefaultAccessPoint(TimeSpan timeout, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task Disconnect(bool turnOffWiFiInterface)
    {
        throw new NotImplementedException();
    }

    public void SetAntenna(AntennaType antenna, bool persist = true)
    {
        throw new NotImplementedException();
    }
}
