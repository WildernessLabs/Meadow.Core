using Meadow.Gateway.WiFi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace Meadow.Networking;

/// <summary>
/// Wrapper around the Linux ip command from iproute2 package
/// </summary>
internal static class IpCommand
{
    public static WifiNetwork[] GetWirelessNetworksInfo(string deviceName)
    {
        // Use iw for WiFi scanning: iw dev wlan0 scan
        var output = Linux.ExecuteBashCommandLine($"iw dev {deviceName} scan");

        if (!string.IsNullOrWhiteSpace(output.StdErr))
        {
            return Array.Empty<WifiNetwork>();
        }

        var list = new List<WifiNetwork>();
        var lines = output.StdOut.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        string? currentSsid = null;
        PhysicalAddress? currentBssid = null;
        sbyte currentSignal = 0;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if (trimmed.StartsWith("BSS "))
            {
                // Save previous network if we have one
                if (currentSsid != null && currentBssid != null)
                {
                    list.Add(new WifiNetwork(
                        currentSsid,
                        currentBssid,
                        NetworkType.Infrastructure,
                        PhyType.Unknown,
                        new NetworkSecuritySettings(NetworkAuthenticationType.Wpa2Psk, NetworkEncryptionType.Unknown),
                        0,
                        NetworkProtocol.Protocol11G,
                        currentSignal));
                }

                // Parse BSSID from line like "BSS aa:bb:cc:dd:ee:ff(on wlan0)"
                var bssidStr = trimmed.Split('(')[0].Replace("BSS ", "").Trim();
                currentBssid = PhysicalAddress.Parse(bssidStr.Replace(":", ""));
                currentSsid = null;
                currentSignal = 0;
            }
            else if (trimmed.StartsWith("SSID: "))
            {
                currentSsid = trimmed.Substring(6);
            }
            else if (trimmed.StartsWith("signal: "))
            {
                // Parse signal like "signal: -72.00 dBm"
                var signalStr = trimmed.Substring(8).Replace(" dBm", "").Trim();
                if (double.TryParse(signalStr, out double signalValue))
                {
                    currentSignal = (sbyte)Math.Round(signalValue);
                }
            }
        }

        // Add last network
        if (currentSsid != null && currentBssid != null)
        {
            list.Add(new WifiNetwork(
                currentSsid,
                currentBssid,
                NetworkType.Infrastructure,
                PhyType.Unknown,
                new NetworkSecuritySettings(NetworkAuthenticationType.Wpa2Psk, NetworkEncryptionType.Unknown),
                0,
                NetworkProtocol.Protocol11G,
                currentSignal));
        }

        return list.ToArray();
    }

    public static IEnumerable<IpDevice> GetDevices()
    {
        // Use ip -json to get machine-readable output
        var addrOutput = Linux.ExecuteBashCommandLine("ip -json addr show");
        var routeOutput = Linux.ExecuteBashCommandLine("ip -json route show");

        if (string.IsNullOrWhiteSpace(addrOutput.StdOut))
        {
            return Enumerable.Empty<IpDevice>();
        }

        var list = new List<IpDevice>();

        try
        {
            using var doc = JsonDocument.Parse(addrOutput.StdOut);
            var interfaces = doc.RootElement;

            foreach (var iface in interfaces.EnumerateArray())
            {
                var device = ParseInterfaceJson(iface);
                if (device != null)
                {
                    // Enhance with route information
                    EnhanceWithRouteInfo(device, routeOutput.StdOut);
                    list.Add(device);
                }
            }
        }
        catch (JsonException)
        {
            // Fallback to non-JSON parsing if needed
            return Enumerable.Empty<IpDevice>();
        }

        return list;
    }

    private static IpDevice? ParseInterfaceJson(JsonElement iface)
    {
        if (!iface.TryGetProperty("ifname", out var ifnameElement))
        {
            return null;
        }

        var device = new IpDevice
        {
            Name = ifnameElement.GetString() ?? string.Empty
        };

        if (iface.TryGetProperty("address", out var addressElement))
        {
            device.HardwareAddress = addressElement.GetString();
        }

        if (iface.TryGetProperty("mtu", out var mtuElement))
        {
            device.Mtu = mtuElement.GetInt32().ToString();
        }

        if (iface.TryGetProperty("operstate", out var operstateElement))
        {
            device.OperState = operstateElement.GetString();
        }

        if (iface.TryGetProperty("link_type", out var linkTypeElement))
        {
            device.LinkType = linkTypeElement.GetString();
        }

        // Parse addresses
        if (iface.TryGetProperty("addr_info", out var addrInfoElement))
        {
            foreach (var addr in addrInfoElement.EnumerateArray())
            {
                if (addr.TryGetProperty("local", out var localElement))
                {
                    var ipStr = localElement.GetString();
                    var prefixLen = 0;
                    if (addr.TryGetProperty("prefixlen", out var prefixElement))
                    {
                        prefixLen = prefixElement.GetInt32();
                    }

                    device.Addresses.Add((ipStr ?? string.Empty, prefixLen));
                }
            }
        }

        // Determine if this is a WiFi device
        var deviceName = device.Name?.ToLower() ?? string.Empty;
        if (deviceName.StartsWith("wlan") || deviceName.StartsWith("wlp"))
        {
            device.Type = "wifi";
        }
        else if (deviceName.StartsWith("eth") || deviceName.StartsWith("enp") || deviceName == "end0")
        {
            device.Type = "ethernet";
        }
        else if (deviceName == "lo")
        {
            device.Type = "loopback";
        }
        else
        {
            device.Type = "unknown";
        }

        return device;
    }

    private static void EnhanceWithRouteInfo(IpDevice device, string routeJson)
    {
        if (string.IsNullOrWhiteSpace(routeJson))
        {
            return;
        }

        try
        {
            using var doc = JsonDocument.Parse(routeJson);
            var routes = doc.RootElement;

            foreach (var route in routes.EnumerateArray())
            {
                if (route.TryGetProperty("dev", out var devElement))
                {
                    var dev = devElement.GetString();
                    if (dev == device.Name)
                    {
                        // This route belongs to our device
                        if (route.TryGetProperty("gateway", out var gatewayElement))
                        {
                            var gateway = gatewayElement.GetString();
                            if (!string.IsNullOrWhiteSpace(gateway))
                            {
                                device.Gateway = gateway;
                            }
                        }
                    }
                }
            }
        }
        catch (JsonException)
        {
            // Ignore parse errors
        }
    }

    public static string[] GetDnsServers()
    {
        // Read DNS servers from /etc/resolv.conf
        var output = Linux.ExecuteBashCommandLine("cat /etc/resolv.conf");

        if (string.IsNullOrWhiteSpace(output.StdOut))
        {
            return Array.Empty<string>();
        }

        var list = new List<string>();
        var lines = output.StdOut.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("nameserver "))
            {
                var dns = trimmed.Substring(11).Trim();
                if (!string.IsNullOrWhiteSpace(dns))
                {
                    list.Add(dns);
                }
            }
        }

        return list.ToArray();
    }
}
