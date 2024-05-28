using Meadow.Devices;
using Meadow.Hardware;
using System.Net.NetworkInformation;

namespace Meadow;

public class WindowsNetworkAdapterCollection : NativeNetworkAdapterCollection
{
    /// <inheritdoc/>
    public override IWiFiNetworkAdapter GetWiFiNetworkAdapter(NetworkInterface ni)
    {
        return new WindowsWifiNetworkAdapter(ni);
    }
}
