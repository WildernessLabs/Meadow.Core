using System.Collections.Generic;
using System.Net;

namespace Meadow.Networking;

internal record NmCliDevice
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? HardwareAddress { get; set; }
    public string? Mtu { get; set; }
    public string? State { get; set; }
    public string? Connection { get; set; }
    public string? ConnectionPath { get; set; }
    public List<IPAddress> IPAddresses { get; set; } = new();
}
