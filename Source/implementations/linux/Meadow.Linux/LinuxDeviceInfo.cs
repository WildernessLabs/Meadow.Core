using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using RTI = System.Runtime.InteropServices.RuntimeInformation;

namespace Meadow;

/// <summary>
/// Represents device information specific to a Linux-based platform.
/// </summary>
public class LinuxDeviceInfo : IDeviceInformation
{
    private Dictionary<string, string> _osInfo = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public string DeviceName { get; set; }
    /// <inheritdoc/>
    public MeadowPlatform Platform { get; }
    /// <inheritdoc/>
    public string UniqueID { get; private set; }
    /// <inheritdoc/>
    public string Model => _osInfo["PRETTY_NAME"];
    /// <inheritdoc/>
    public string OSVersion => _osInfo["VERSION"];
    /// <inheritdoc/>
    public string ProcessorType { get; private set; } = "Unknown";
    /// <inheritdoc/>
    public string ProcessorSerialNumber => "Unknown";
    /// <inheritdoc/>
    public string CoprocessorType => "None";
    /// <inheritdoc/>
    public string? CoprocessorOSVersion => null;

    internal LinuxDeviceInfo()
    {
        Platform = RTI.ProcessArchitecture switch
        {
            Architecture.Arm => MeadowPlatform.EmbeddedLinux,
            Architecture.Arm64 => MeadowPlatform.EmbeddedLinux,
            _ => MeadowPlatform.DesktopLinux
        };

        // unique id is at /var/lib/dbus/machine-id
        UniqueID = File.ReadAllText("/var/lib/dbus/machine-id").Trim();
        DeviceName = File.ReadAllText("/etc/hostname").Trim();

        GetOsInfo();
        GetCpuInfo();
    }

    private void GetOsInfo()
    {
        var content = File.ReadAllText("/etc/os-release");
        var lines = content.Split('\n');
        foreach (var line in lines)
        {
            var items = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 2)
            {
                _osInfo.Add(
                    items[0].Trim(),
                    items[1].Replace("\"", string.Empty).Trim());
            }
        }
    }

    private void GetCpuInfo()
    {
        using var reader = File.OpenText("/proc/cpuinfo");

        var done = false;

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line != null)
            {
                var items = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (items.Length == 2)
                {
                    switch (items[0].Trim())
                    {
                        case "model name":
                            ProcessorType = items[1].Trim();
                            done = true;
                            break;
                    }
                }
            }
            if (done) break;
        }
    }
}
