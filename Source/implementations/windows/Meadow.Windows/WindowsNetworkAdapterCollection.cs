using Meadow.Devices;
using Meadow.Hardware;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Meadow;

/// <summary>
/// Represents a collection of network adapters for Windows.
/// </summary>
public class WindowsNetworkAdapterCollection : NativeNetworkAdapterCollection
{
    /// <inheritdoc/>
    public override IWiFiNetworkAdapter GetWiFiNetworkAdapter(NetworkInterface ni)
    {
        return new WindowsWifiNetworkAdapter(ni);
    }

    /// <inheritdoc/>
    public override Task Refresh()
    {
        return base.Refresh();
    }
}
