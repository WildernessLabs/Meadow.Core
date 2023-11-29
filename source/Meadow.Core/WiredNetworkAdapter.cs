﻿using Meadow.Hardware;
using System;
using System.Net.NetworkInformation;

namespace Meadow.Devices;

/// <summary>
/// Represents a wired Ethernet network adapter
/// </summary>
public class WiredNetworkAdapter : NetworkAdapterBase, IWiredNetworkAdapter
{
    /// <summary>
    /// Creates an instance of a WiredNetworkAdapter
    /// </summary>
    /// <param name="ni">The associated native interface</param>
    public WiredNetworkAdapter(NetworkInterface ni)
        : base(ni)
    {
        if (ni.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
        {
            throw new ArgumentException();
        }
    }

    /// <summary>
    /// Creates an instance of a WiredNetworkAdapter
    /// </summary>
    public WiredNetworkAdapter()
        : base(NetworkInterfaceType.Ethernet)
    {
    }

    // TODO: determine a way to detect this
    /// <summary>
    /// Returns <c>true</c> if the adapter is connected, otherwise <c>false</c>
    /// </summary>
    public override bool IsConnected => true;
}
