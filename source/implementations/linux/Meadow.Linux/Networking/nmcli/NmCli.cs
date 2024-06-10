using Meadow.Gateway.WiFi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Meadow.Networking;

/// <summary>
/// Wrapper around the Linux nmcli network manager CLI (doesn't require compiling libnm.so)
/// </summary>
internal static class NmCli
{
    private static string[]? ParseDeviceRow(string row)
    {
        var firstColon = row.IndexOf(':');
        if (firstColon < 0) { return null; }

        return new string[]
        {
            row.Substring(0, firstColon).Trim(),
            row.Substring(firstColon + 1).Trim(),
        };
    }

    public static WifiNetwork[] GetWirelessNetworksInfo()
    {
        // sudo nmcli -g all -c no d wifi

        // NAME   SSID         SSID-HEX                 BSSID       MODE CHAN FREQ   RATE   SIGNAL BARS SECURITY WPA-FLAGS  RSN-FLAGS      DEVICE ACTIVE IN-USE  DBUS-PATH
        //  0      1              2                       3           4    5  6        7         8   9   10    11            12                13   14  15  16                            
        // AP[5]:interwebs:696E74657277656273:78\:D6\:D6\:F0\:B6\:C5:Infra:6:2437 MHz:270 Mbit/s:72:▂▄▆_:WPA2:(none):pair_ccmp group_ccmp psk:wlan0:yes:*:/org/freedesktop/NetworkManager/AccessPoint/4
        var output = ExecuteBashCommandLine($"nmcli -g all -c no d wifi");

        var rows = output.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);

        //.Select(line => ParseDeviceRow(line));

        var list = new List<WifiNetwork>();

        foreach (var row in rows)
        {
            var fields = EscapedSplit(row, ':', '\\')
                .ToArray();

            var ssid = fields[1];
            var bssid = PhysicalAddress.Parse(fields[3]);
            var channel = fields[5];
            var inUse = fields[15];
            var signal = sbyte.Parse(fields[8]);

            list.Add(new WifiNetwork(
                ssid,
                bssid,
                NetworkType.Infrastructure,
                PhyType.Unknown,
                new NetworkSecuritySettings(NetworkAuthenticationType.Wpa2Psk, NetworkEncryptionType.Unknown),
                0,
                NetworkProtocol.Protocol11G,
                signal)
                );
        }

        return list.ToArray();
    }

    private static IEnumerable<string> EscapedSplit(this string input, char separator, char escapeCharacter)
    {
        int startOfSegment = 0;
        int index = 0;
        while (index < input.Length)
        {
            index = input.IndexOf(separator, index);
            if (index > 0 && input[index - 1] == escapeCharacter)
            {
                index++;
                continue;
            }
            if (index == -1)
            {
                break;
            }
            yield return input
                .Substring(startOfSegment, index - startOfSegment)
                .Replace($"{escapeCharacter}", string.Empty);
            index++;
            startOfSegment = index;
        }
        yield return input.Substring(startOfSegment);
    }

    public static string[][] GetDeviceDetail(string deviceName)
    {
        //  nmcli -f all d show wlan0
        var output = ExecuteBashCommandLine($"nmcli -c no -f all device show {deviceName}");

        return output
            .Split('\n', System.StringSplitOptions.RemoveEmptyEntries)
            .Select(line => ParseDeviceRow(line) ?? Array.Empty<string>())
            .ToArray();
    }

    public static IEnumerable<NmCliDevice> GetDevices()
    {
        var output = ExecuteBashCommandLine("nmcli -c no device show");

        var fields = output
            .Split('\n', System.StringSplitOptions.RemoveEmptyEntries)
            .Select(line => ParseDeviceRow(line));

        NmCliDevice? device = null;

        var list = new List<NmCliDevice>();

        foreach (var field in fields)
        {
            if (field[0] == "GENERAL.DEVICE")
            {
                if (device != null)
                {
                    list.Add(device);
                }

                device = new NmCliDevice()
                {
                    Name = field[1],
                };
            }

            if (device != null)
            {
                switch (field[0])
                {
                    case "GENERAL.TYPE":
                        device.Type = field[1];
                        break;
                    case "GENERAL.HWADDR":
                        device.HardwareAddress = field[1];
                        break;
                    case "GENERAL.MTU":
                        device.Mtu = field[1];
                        break;
                    case "GENERAL.STATE":
                        device.State = field[1];
                        break;
                    case "GENERAL.CONNECTION":
                        device.Connection = field[1];
                        break;
                    case "GENERAL.CON-PATH":
                        device.ConnectionPath = field[1];
                        break;
                    default:
                        if (field[0].StartsWith("IP4.ADDRESS") || field[0].StartsWith("IP6.ADDRESS"))
                        {
                            if (IPNetwork.TryParse(field[1], out IPNetwork network))
                            {
                                device.IPAddresses.Add(network.BaseAddress);
                            }
                        }
                        break;
                }
            }
        }

        return list;
    }

    private static string ExecuteBashCommandLine(string command)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);

        process?.WaitForExit();

        return process?.StandardOutput.ReadToEnd() ?? string.Empty;
    }
}
