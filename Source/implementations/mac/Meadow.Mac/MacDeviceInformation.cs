using Meadow.Hardware;
using System;
using System.Diagnostics;

namespace Meadow;

/// <summary>
/// Represents information about the device running the application.
/// </summary>
public class MacDeviceInformation : IDeviceInformation
{
    /// <inheritdoc/>
    public string DeviceName { get; set; }
    /// <inheritdoc/>
    public string Model { get; }
    /// <inheritdoc/>
    public MeadowPlatform Platform => MeadowPlatform.OSX;
    /// <inheritdoc/>
    public string ProcessorType { get; }
    /// <inheritdoc/>
    public string ProcessorSerialNumber => "Unknown";
    /// <inheritdoc/>
    public string UniqueID { get; }
    /// <inheritdoc/>
    public string CoprocessorType => "None";
    /// <inheritdoc/>
    public string? CoprocessorOSVersion => null;
    /// <inheritdoc/>
    public string OSVersion => Environment.OSVersion.ToString();

    internal MacDeviceInformation()
    {
        DeviceName = Environment.MachineName;
        Model = $"{Environment.OSVersion.Platform} {Environment.OSVersion.Version.ToString(2)}";
        var cpu = ExecuteBashCommandLine("sysctl -n machdep.cpu.brand_string");
        ProcessorType = cpu.Trim();
        var mac_id = ExecuteBashCommandLine("ioreg -l | grep IOPlatformSerialNumber | sed 's/.*= //' | sed 's/\\\"//g'");
        UniqueID = mac_id.Trim();
        // sysctl -n machdep.cpu.brand_string
    }

    private string ExecuteBashCommandLine(string command)
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
