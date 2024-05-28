using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Devices;

/// <summary>
/// Represents WiFi network adapter
/// </summary>
public class WiFiNetworkAdapter : NetworkAdapterBase, IWirelessNetworkAdapter, IWiFiNetworkAdapter
{
    /// <summary>
    /// Creates an instance of a WiFiNetworkAdapter
    /// </summary>
    /// <param name="ni">The associated native interface</param>
    public WiFiNetworkAdapter(NetworkInterface ni)
        : base(ni)
    {
        if (ni.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
        {
            throw new ArgumentException();
        }
    }

    // TODO: determine a way to detect this
    /// <summary>
    /// Returns <c>true</c> if the adapter is connected, otherwise <c>false</c>
    /// </summary>
    public override bool IsConnected => true;

    /// <summary>
    /// Returns the current Antenna type
    /// </summary>
    public AntennaType CurrentAntenna => throw new NotImplementedException();

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
    public Task ClearStoredAccessPointInformation()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task Connect(string ssid, string password, TimeSpan timeout, CancellationToken cancellationToken, ReconnectionType reconnection = ReconnectionType.Automatic)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task ConnectToDefaultAccessPoint(TimeSpan timeout, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task Disconnect(bool turnOffWiFiInterface)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<IList<WifiNetwork>> Scan(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<IList<WifiNetwork>> Scan(TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the current antenna type used by the adapter
    /// </summary>
    /// <param name="antenna">The antenna type to use</param>
    /// <param name="persist">Whether or not the type should persist across OS restarts</param>
    /// <exception cref="NotImplementedException"></exception>
    public void SetAntenna(AntennaType antenna, bool persist = true)
    {
        throw new NotImplementedException();
    }
}
