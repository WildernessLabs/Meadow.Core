using Meadow.Hardware;
using System;
using System.Diagnostics;

namespace Meadow
{
    public class MacDeviceInformation : IDeviceInformation
    {
        public string DeviceName { get; set; }
        public string Model { get; }
        public MeadowPlatform Platform => MeadowPlatform.Windows;
        public string ProcessorType { get; }
        public string ProcessorSerialNumber => "Unknown";
        public string UniqueID { get; }
        public string CoprocessorType => "None";
        public string? CoprocessorOSVersion => null;
        public string OSVersion => Environment.OSVersion.ToString();

        internal MacDeviceInformation()
        {
            DeviceName = Environment.MachineName;
            Model = $"{Environment.OSVersion.Platform} {Environment.OSVersion.Version.ToString(2)}";
            var cpu = ExecuteBashCommandLine("sysctl - n machdep.cpu.brand_string");
            ProcessorType = cpu.Trim();
            var mac_id = ExecuteBashCommandLine("ioreg -l | grep IOPlatformSerialNumber | sed 's/.*= //' | sed 's/\\\"//g'");
            UniqueID = mac_id.Trim();
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
}
