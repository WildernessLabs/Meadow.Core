using Meadow.Hardware;
using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Meadow.Devices
{

    /// <summary>
    /// Represents a Cell network adapter
    /// </summary>
    public class CellNetworkAdapter : NetworkAdapterBase, ICellNetworkAdapter
    {
        public const string LIBRARY_NAME = "nuttx";

        [DllImport(LIBRARY_NAME, SetLastError = true)]
        private static extern bool meadow_cell_is_connected();

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

        /// <summary>
        /// Returns <b>true</b> if the adapter is connected, otherwise <b>false</b>
        /// </summary>
        public override bool IsConnected => meadow_cell_is_connected();
    }
}
