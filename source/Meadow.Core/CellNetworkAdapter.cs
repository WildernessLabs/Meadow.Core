using Meadow.Hardware;
using System;
using System.Net.NetworkInformation;

namespace Meadow.Devices
{

    /// <summary>
    /// Represents a Cell network adapter
    /// </summary>
    public class CellNetworkAdapter : NetworkAdapterBase, IWiredNetworkAdapter
    {
        /// <summary>
        /// Creates an instance of a CellNetworkAdapter
        /// </summary>
        /// <param name="ni">The associated native interface</param>
        public CellNetworkAdapter(NetworkInterface ni)
            : base(ni)
        {
            if (ni.NetworkInterfaceType != NetworkInterfaceType.Ppp)
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Creates an instance of a CellNetworkAdapter
        /// </summary>
        public CellNetworkAdapter()
            : base(NetworkInterfaceType.Ppp)
        {
        }

        // TODO: determine a way to detect this
        /// <summary>
        /// Returns <b>true</b> if the adapter is connected, otherwise <b>false</b>
        /// </summary>
        public override bool IsConnected => true;
    }
}
