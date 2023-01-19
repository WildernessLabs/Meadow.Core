using Meadow.Hardware;
using System;

namespace Meadow
{
    public class WindowsDeviceInformation : IDeviceInformation
    {
        public string DeviceName { get; set; }
        public string Model => "Unknown";
        public MeadowPlatform Platform => MeadowPlatform.Windows;
        public string ProcessorType => "Unknown";
        public string ProcessorSerialNumber => "Unknown";
        public string UniqueID { get; private set; }
        public string CoprocessorType => "None";
        public string? CoprocessorOSVersion => null;
        public string OSVersion => Environment.OSVersion.ToString();

        internal WindowsDeviceInformation()
        {
            DeviceName = "Meadow for Windows";

            // TODO: implement some form of unique ID
            UniqueID = "Meadow.Windows";
        }
    }
}
