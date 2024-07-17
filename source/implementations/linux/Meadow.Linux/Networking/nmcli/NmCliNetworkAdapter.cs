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
#pragma warning disable CS0067 // The event 'NmCliNetworkAdapter.NetworkConnecting' is never used
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
#pragma warning restore CS0067 // The event 'NmCliNetworkAdapter.NetworkConnecting' is never used

    internal NmCliDevice NmCliDevice { get; }

    internal NmCliNetworkAdapter(NmCliDevice nmCliDevice)
    {
        NmCliDevice = nmCliDevice;
    }

    /// <inheritdoc/>
    public string Name => NmCliDevice.Name ?? string.Empty;
    /// <inheritdoc/>
    public IPAddress IpAddress => NmCliDevice.GetIPAddresses().First();
    /// <inheritdoc/>
    public PhysicalAddress MacAddress => PhysicalAddress.Parse(NmCliDevice.HardwareAddress);
    /// <inheritdoc/>
    public IPAddress Gateway => NmCliDevice.GetGateways().First();
    /// <inheritdoc/>
    public bool IsConnected => NmCliDevice.State?.Contains("connected") ?? false;

    /// <inheritdoc/>
    public IPAddress SubnetMask => throw new NotImplementedException();

    /// <inheritdoc/>
    public IPAddressCollection DnsAddresses => throw new NotImplementedException();
}
