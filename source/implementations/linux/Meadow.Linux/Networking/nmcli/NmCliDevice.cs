using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Meadow.Networking;

internal record NmCliDevice
{
    public Dictionary<string, string> Fields { get; } = new();

    public string? Name => Fields["GENERAL.DEVICE"];
    public string? Type => Fields["GENERAL.TYPE"];
    public string? HardwareAddress => Fields["GENERAL.HWADDR"];
    public string? Mtu => Fields["GENERAL.MTU"];
    public string? State => Fields["GENERAL.STATE"];
    public string? Connection => Fields["GENERAL.CONNECTION"];
    public string? ConnectionPath => Fields["GENERAL.CON-PATH"];

    public IEnumerable<IPAddress> GetIPAddresses()
    {
        var fields = Fields
            .Where(f => f.Key.StartsWith("IP4.ADDRESS") || f.Key.StartsWith("IP6.ADDRESS"))
            .Select(f => f.Value);

        var list = new List<IPAddress>();

        foreach (var f in fields)
        {
            if (IPNetwork.TryParse(f, out IPNetwork network))
            {
                list.Add(network.BaseAddress);
            }
        }

        return list;
    }

    public IEnumerable<IPAddress> GetDnsServers()
    {
        var fields = Fields
            .Where(f => f.Key.StartsWith("IP4.DNS") || f.Key.StartsWith("IP6.DNS"))
            .Select(f => f.Value);

        var list = new List<IPAddress>();

        foreach (var f in fields)
        {
            if (IPAddress.TryParse(f, out IPAddress? addr))
            {
                if (addr != null)
                {
                    list.Add(addr);
                }
            }
        }

        return list;
    }

    public IEnumerable<IPAddress> GetGateways()
    {
        var fields = Fields
            .Where(f => f.Key.StartsWith("IP4.GATEWAY") || f.Key.StartsWith("IP6.GATEWAY"))
            .Select(f => f.Value);

        var list = new List<IPAddress>();

        foreach (var f in fields)
        {
            if (IPAddress.TryParse(f, out IPAddress? addr))
            {
                if (addr != null)
                {
                    list.Add(addr);
                }
            }
        }

        return list;
    }
}
