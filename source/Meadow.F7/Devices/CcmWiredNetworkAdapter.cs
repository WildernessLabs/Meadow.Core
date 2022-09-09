using System.Net.NetworkInformation;

namespace Meadow.Devices
{
    public class CcmWiredNetworkAdapter : NetworkAdapterBase
    {
        internal CcmWiredNetworkAdapter()
            : base(NetworkInterfaceType.Ethernet)
        {
        }

        public override bool IsConnected => false;
    }
}
