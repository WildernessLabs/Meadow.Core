using Meadow.Hardware;
using System.Net.NetworkInformation;

namespace Meadow.Devices
{
    public class WiredNetworkAdapter : NetworkAdapterBase, IWiredNetworkAdapter
    {
        public WiredNetworkAdapter()
            : base(NetworkInterfaceType.Ethernet)
        {
        }

        // TODO: determine a way to detect this
        public override bool IsConnected => true;
    }
}
