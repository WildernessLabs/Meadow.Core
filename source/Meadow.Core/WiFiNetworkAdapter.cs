using Meadow.Hardware;
using System;
using System.Net.NetworkInformation;

namespace Meadow.Devices;

/// <summary>
/// Represents WiFi network adapter
/// </summary>
public class WiFiNetworkAdapter : NetworkAdapterBase, IWirelessNetworkAdapter
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
    /// Returns <b>true</b> if the adapter is connected, otherwise <b>false</b>
    /// </summary>
    public override bool IsConnected => true;

    /// <summary>
    /// Returns the current Antenna type
    /// </summary>
    public AntennaType CurrentAntenna => throw new NotImplementedException();

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
