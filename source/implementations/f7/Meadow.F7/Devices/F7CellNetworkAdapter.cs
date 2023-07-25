using Meadow.Hardware;
using System;
using System.Net.NetworkInformation;

namespace Meadow.Devices;

/// <summary>
/// Represents a Cell network adapter
/// </summary>
public class F7CellNetworkAdapter : NetworkAdapterBase, ICellNetworkAdapter
{
    /// <summary>
    /// Creates an instance of a F7CellNetworkAdapter
    /// </summary>
    /// <param name="ni">The associated native interface</param>
    internal F7CellNetworkAdapter(NetworkInterface ni)
        : base(ni)
    {
        if (ni.NetworkInterfaceType != NetworkInterfaceType.Ppp)
        {
            throw new ArgumentException();
        }
    }

    /// <summary>
    /// Creates an instance of a F7CellNetworkAdapter
    /// </summary>
    internal F7CellNetworkAdapter()
        : base(NetworkInterfaceType.Ppp)
    {
    }

    /// <summary>
    /// Returns <b>true</b> if the adapter is connected, otherwise <b>false</b>
    /// </summary>
    public override bool IsConnected => Core.Interop.Nuttx.meadow_cell_is_connected();
}
