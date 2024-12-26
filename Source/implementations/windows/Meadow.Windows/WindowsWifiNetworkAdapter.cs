using ManagedNativeWifi;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

/// <summary>
/// Encapsulates WiFi functionality specific to Windows platforms
/// </summary>
public class WindowsWifiNetworkAdapter : NetworkAdapterBase, IWirelessNetworkAdapter, IWiFiNetworkAdapter
{
    private NetworkInterface _networkInterface;

    /// <summary>
    /// Creates a WindowsWifiNetworkAdapter
    /// </summary>
    /// <param name="ni">The managed NetworkInterface describing the adapter</param>
    public WindowsWifiNetworkAdapter(NetworkInterface ni)
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
        var list = new List<WifiNetwork>();
        var testID = Guid.Parse(_networkInterface.Id);

        foreach (var network in NativeWifi.EnumerateBssNetworks())
        {
            if (network.Interface.Id.Equals(testID))
            {
                var net = new WifiNetwork(
                    network.Ssid.ToString(),
                    new PhysicalAddress(network.Bssid.ToBytes()),
                    network.BssType.ToNetworkType(),
                    network.PhyType.ToMeadowPhyType(),
                    new NetworkSecuritySettings(NetworkAuthenticationType.Unknown, NetworkEncryptionType.Unknown),
                    (int)network.Band * 10,
                    NetworkProtocol.Protocol11B, // todo
                    (sbyte)network.SignalStrength
                );

                list.Add(net);
            }
        }

        return list;
    }

    /// <inheritdoc/>
    /*
    public override bool IsConnected
    {
        get
        {
            var testID = Guid.Parse(_networkInterface.Id);

            foreach (var connection in NativeWifi.EnumerateInterfaceConnections())
            {
                if (connection.Id.Equals(testID))
                {
                    return connection.IsConnected;
                }
            }

            return false;
        }
    }
    */
    /// <inheritdoc/>
    public async Task Connect(string ssid, string password, TimeSpan timeout, CancellationToken token, ReconnectionType reconnection = ReconnectionType.Manual)
    {
        await NativeWifi.ConnectNetworkAsync(
            Guid.Parse(_networkInterface.Id),
            ssid,
            BssType.Infrastructure,
            timeout,
            token);
    }

    /// <inheritdoc/>
    public Task Disconnect(bool turnOffWiFiInterface)
    {
        NativeWifi.DisconnectNetwork(Guid.Parse(_networkInterface.Id));

        return Task.CompletedTask;
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
