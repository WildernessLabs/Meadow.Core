using Meadow.Hardware;
using System;

namespace Meadow.Networking;

internal static class LinuxNetworkAdapterManager
{
    public static INetworkAdapterCollection GetNetworkAdapters()
    {
        // First, check for NetworkManager (nmcli)
        var nmcliResult = Linux.ExecuteBashCommandLine("nmcli --help");
        if (!string.IsNullOrWhiteSpace(nmcliResult.StdOut))
        {
            return new NmCliNetworkAdapterCollection();
        }

        // Fallback to ip command from iproute2
        // dev note: oddly `ip --help` sends back the result in stderr
        var ipResult = Linux.ExecuteBashCommandLine("ip address");
        if (!string.IsNullOrWhiteSpace(ipResult.StdOut))
        {
            return new IpNetworkAdapterCollection();
        }

        throw new NotSupportedException("No supported network manager found. Please install either NetworkManager (nmcli) or iproute2 (ip).");
    }
}
