using Meadow.Hardware;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Meadow.Networking;

/// <summary>
/// Provides wrapper around a Linux network device using ip command
/// </summary>
public class IpNetworkAdapter : INetworkAdapter
{
#pragma warning disable CS0067 // The event is never used
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
#pragma warning restore CS0067

    internal IpDevice IpDevice { get; }

    internal IpNetworkAdapter(IpDevice ipDevice)
    {
        IpDevice = ipDevice;
    }

    /// <inheritdoc/>
    public string Name => IpDevice.Name ?? string.Empty;

    /// <inheritdoc/>
    public IPAddress IpAddress
    {
        get
        {
            var addresses = IpDevice.GetIPAddresses().ToArray();
            if (addresses.Length == 0)
            {
                return IPAddress.None;
            }
            // Prefer IPv4 addresses
            return addresses.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ?? addresses[0];
        }
    }

    /// <inheritdoc/>
    public PhysicalAddress MacAddress
    {
        get
        {
            if (string.IsNullOrWhiteSpace(IpDevice.HardwareAddress))
            {
                return PhysicalAddress.None;
            }
            try
            {
                return PhysicalAddress.Parse(IpDevice.HardwareAddress.Replace(":", ""));
            }
            catch
            {
                return PhysicalAddress.None;
            }
        }
    }

    /// <inheritdoc/>
    public IPAddress Gateway
    {
        get
        {
            var gateway = IpDevice.GetGateway();
            return gateway ?? IPAddress.None;
        }
    }

    /// <inheritdoc/>
    public bool IsConnected => IpDevice.IsConnected();

    /// <inheritdoc/>
    public IPAddress SubnetMask
    {
        get
        {
            var mask = IpDevice.GetSubnetMask();
            return mask ?? IPAddress.None;
        }
    }

    /// <inheritdoc/>
    public IPAddressCollection DnsAddresses
    {
        get
        {
            var dnsServers = IpDevice.GetDnsServers().ToArray();
            // IPAddressCollection is abstract, need to create a concrete implementation
            // For now, return empty or use a workaround
            throw new NotImplementedException("DnsAddresses property requires IPAddressCollection implementation");
        }
    }
}
