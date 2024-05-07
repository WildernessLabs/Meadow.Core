using ManagedNativeWifi;
using Meadow.Devices;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

internal static class Extensions
{
    public static Gateway.WiFi.PhyType ToMeadowPhyType(this ManagedNativeWifi.PhyType phyType)
    {
        return phyType switch
        {
            ManagedNativeWifi.PhyType.He => Gateway.WiFi.PhyType.HE,
            ManagedNativeWifi.PhyType.Vht => Gateway.WiFi.PhyType.Vht,
            ManagedNativeWifi.PhyType.Fhss => Gateway.WiFi.PhyType.Fhss,
            ManagedNativeWifi.PhyType.Dmg => Gateway.WiFi.PhyType.Dmg,
            ManagedNativeWifi.PhyType.Dsss => Gateway.WiFi.PhyType.Dsss,
            ManagedNativeWifi.PhyType.Erp => Gateway.WiFi.PhyType.Erp,
            ManagedNativeWifi.PhyType.HrDsss => Gateway.WiFi.PhyType.Hrdsss,
            ManagedNativeWifi.PhyType.Ht => Gateway.WiFi.PhyType.HT,
            ManagedNativeWifi.PhyType.IrBaseband => Gateway.WiFi.PhyType.IRBaseband,
            ManagedNativeWifi.PhyType.Ofdm => Gateway.WiFi.PhyType.Ofdm,
            _ => Gateway.WiFi.PhyType.Unknown

        };
    }

    public static NetworkType ToNetworkType(this BssType bssType)
    {
        return bssType switch
        {
            BssType.Infrastructure => NetworkType.Infrastructure,
            BssType.Independent => NetworkType.AdHoc,
            _ => NetworkType.Any
        };
    }

    public static NetworkSecuritySettings AsSecuritySettings(this AuthenticationAlgorithm algo, CipherAlgorithm cipher)
    {
        return algo switch
        {
            AuthenticationAlgorithm.Open => new NetworkSecuritySettings(NetworkAuthenticationType.None, NetworkEncryptionType.None),
            AuthenticationAlgorithm.Shared => new NetworkSecuritySettings(NetworkAuthenticationType.Wep, NetworkEncryptionType.Wep40),
            AuthenticationAlgorithm.WPA => new NetworkSecuritySettings(NetworkAuthenticationType.Wpa, NetworkEncryptionType.Unknown),
            AuthenticationAlgorithm.WPA_PSK => new NetworkSecuritySettings(NetworkAuthenticationType.WpaPsk, NetworkEncryptionType.Unknown),
            AuthenticationAlgorithm.WPA3 => new NetworkSecuritySettings(NetworkAuthenticationType.Wpa3Psk, NetworkEncryptionType.Unknown),
            _ => new NetworkSecuritySettings(NetworkAuthenticationType.Unknown, NetworkEncryptionType.Unknown)
        };
    }

    public static sbyte PercentageSignalToEstimatedDB(this int percent)
    {
        // 100 == -30,
        // 75 == -60
        // 50 == -70
        // 25 == -80
        // 0 == -90

        return percent switch
        {
            > 75 => (sbyte)((30 + 100 - percent) * -1),
            > 50 => (sbyte)((60 + (75 - percent) * 0.5) * -1),
            > 25 => (sbyte)((70 + (50 - percent) * 0.5) * -1),
            _ => (sbyte)((80 + (25 - percent) * 0.5) * -1),
        };
    }
}

public class WindowsWifiNetworkAdapter : NetworkAdapterBase, IWirelessNetworkAdapter, IWiFiNetworkAdapter
{
    private NetworkInterface _networkInterface;

    public WindowsWifiNetworkAdapter(NetworkInterface ni)
        : base(NetworkInterfaceType.Wireless80211)
    {
        _networkInterface = ni;
    }

    public Task<IList<WifiNetwork>> Scan(CancellationToken token)
    {
        return Task.FromResult(Scan() as IList<WifiNetwork>);
    }

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


    public string? Ssid => throw new NotImplementedException();

    public PhysicalAddress Bssid => throw new NotImplementedException();

    public bool AutoConnect => throw new NotImplementedException();

    public bool AutoReconnect => throw new NotImplementedException();

    public string DefaultSsid => throw new NotImplementedException();

    public int Channel => throw new NotImplementedException();

    public AntennaType CurrentAntenna => throw new NotImplementedException();

    public override bool IsConnected => throw new NotImplementedException();

    public Task ClearStoredAccessPointInformation()
    {
        throw new NotImplementedException();
    }

    public Task Connect(string ssid, string password, TimeSpan timeout, CancellationToken token, ReconnectionType reconnection = ReconnectionType.Automatic)
    {
        throw new NotImplementedException();
    }

    public void ConnectToDefaultAccessPoint()
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

public class WindowsNetworkAdapterCollection : NativeNetworkAdapterCollection
{
    public override IWiFiNetworkAdapter GetWiFiNetworkAdapter(NetworkInterface ni)
    {
        return new WindowsWifiNetworkAdapter(ni);
    }
}
