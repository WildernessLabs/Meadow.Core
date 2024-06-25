using Meadow.Hardware;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using SRI = System.Runtime.InteropServices;

namespace Meadow.Simulation;

internal class SimulationInformation : IDeviceInformation
{
    public string DeviceName { get; set; }
    public string Model { get; set; }
    public MeadowPlatform Platform => MeadowPlatform.MeadowSimulation;
    public string ProcessorType { get; private set; } = "Unknown";
    public string ProcessorSerialNumber { get; private set; } = "SIMULATOR";
    public string ChipID => "SIM";
    public string CoprocessorType => "None";
    public string? CoprocessorOSVersion => null;
    public string UniqueID { get; private set; } = "SIM";
    public string OSVersion => "SIM";

    public SimulationInformation()
    {
        Model = $"{System.Environment.OSVersion.Platform} {System.Environment.OSVersion.Version.ToString(2)}";
        DeviceName = Environment.MachineName;

        if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0\"))
            {
                ProcessorType = (key?.GetValue("ProcessorNameString")?.ToString() ?? "Unknown").Trim();
            }
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography\"))
            {
                ProcessorSerialNumber = UniqueID = (key?.GetValue("MachineGuid")?.ToString() ?? "Unknown").Trim();
            }
        }
        else if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            ProcessorSerialNumber = UniqueID = File.ReadAllText("/var/lib/dbus/machine-id").Trim();
        }
        else if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var mac_id = ExecuteBashCommandLine("ioreg -l | grep IOPlatformSerialNumber | sed 's/.*= //' | sed 's/\\\"//g'");
            ProcessorSerialNumber = UniqueID = mac_id.Trim();
        }
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