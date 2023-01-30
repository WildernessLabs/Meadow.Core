using Meadow.Hardware;
using Microsoft.Win32;
using System;

namespace Meadow
{
    public class WindowsDeviceInformation : IDeviceInformation
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

        internal WindowsDeviceInformation()
        {
            DeviceName = Environment.MachineName;
            Model = $"{Environment.OSVersion.Platform} {Environment.OSVersion.Version.ToString(2)}";

            using (var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0\"))
            {
                ProcessorType = (key?.GetValue("ProcessorNameString")?.ToString() ?? "Unknown").Trim();
            }
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography\"))
            {
                UniqueID = (key?.GetValue("MachineGuid")?.ToString() ?? "Unknown").Trim();
            }
        }
    }
}
