using Meadow.Hardware;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Meadow.Networking;

/// <summary>
/// Provides wrapper around a Linux Network Manager device
/// </summary>
public class NmCliNetworkAdapter : INetworkAdapter
{
    /// <inheritdoc/>
    public event NetworkStateHandler? NetworkConnecting;
    /// <inheritdoc/>
    public event NetworkConnectionHandler? NetworkConnected;
    /// <inheritdoc/>
    public event NetworkDisconnectionHandler? NetworkDisconnected;
    /// <inheritdoc/>
    public event NetworkStateHandler? NetworkConnectFailed;
    /// <inheritdoc/>
    public event NetworkErrorHandler? NetworkError;

    internal NmCliDevice NmCliDevice { get; }

    internal NmCliNetworkAdapter(NmCliDevice nmCliDevice)
    {
        NmCliDevice = nmCliDevice;
    }

    /// <inheritdoc/>
    public string Name => NmCliDevice.Name;
    /// <inheritdoc/>
    public IPAddress IpAddress => NmCliDevice.IPAddresses.First();
    /// <inheritdoc/>
    public PhysicalAddress MacAddress => PhysicalAddress.Parse(NmCliDevice.HardwareAddress);




    public IPAddress SubnetMask => throw new NotImplementedException();
    public IPAddress Gateway => throw new NotImplementedException();
    public bool IsConnected => throw new NotImplementedException();


    public IPAddressCollection DnsAddresses => throw new NotImplementedException();
}
