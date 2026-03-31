using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Meadow.Networking;

internal class IpDevice
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? HardwareAddress { get; set; }
    public string? Mtu { get; set; }
    public string? OperState { get; set; }
    public string? LinkType { get; set; }
    public string? Gateway { get; set; }

    public List<(string Address, int PrefixLength)> Addresses { get; } = new();

    public IEnumerable<IPAddress> GetIPAddresses()
    {
        var list = new List<IPAddress>();

        foreach (var addr in Addresses)
        {
            if (IPAddress.TryParse(addr.Address, out IPAddress? ipAddr))
            {
                if (ipAddr != null)
                {
                    list.Add(ipAddr);
                }
            }
        }

        return list;
    }

    public IEnumerable<IPAddress> GetDnsServers()
    {
        // DNS servers are system-wide, not per-interface
        var dnsServers = IpCommand.GetDnsServers();
        var list = new List<IPAddress>();

        foreach (var dns in dnsServers)
        {
            if (IPAddress.TryParse(dns, out IPAddress? addr))
            {
                if (addr != null)
                {
                    list.Add(addr);
                }
            }
        }

        return list;
    }

    public IPAddress? GetGateway()
    {
        if (string.IsNullOrWhiteSpace(Gateway))
        {
            return null;
        }

        if (IPAddress.TryParse(Gateway, out IPAddress? addr))
        {
            return addr;
        }

        return null;
    }

    public IPAddress? GetSubnetMask()
    {
        // Calculate subnet mask from prefix length of first IPv4 address
        var firstV4Addr = Addresses.FirstOrDefault(a => a.Address.Contains('.'));
        if (firstV4Addr.Address == null)
        {
            return null;
        }

        var prefixLen = firstV4Addr.PrefixLength;
        if (prefixLen == 0)
        {
            return null;
        }

        // Convert prefix length to subnet mask
        uint mask = 0xFFFFFFFF << (32 - prefixLen);
        Span<byte> bytes = stackalloc byte[4];
        bytes[0] = (byte)(mask >> 24);
        bytes[1] = (byte)(mask >> 16);
        bytes[2] = (byte)(mask >> 8);
        bytes[3] = (byte)mask;

        return new IPAddress(bytes);
    }

    public bool IsConnected()
    {
        // Consider device connected if OperState is UP and it has at least one IP address
        return OperState?.ToUpper() == "UP" && Addresses.Any();
    }
}
