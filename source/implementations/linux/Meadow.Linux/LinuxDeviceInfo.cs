using Meadow.Hardware;
using System;

namespace Meadow
{
    public class LinuxDeviceInfo : IDeviceInformation
    {
        internal LinuxDeviceInfo()
        {
        }

        public string DeviceName { get => "Meadow.Linux"; set { } }

        public string Model => "[TBD]";

        public MeadowPlatform Platform => MeadowPlatform.MeadowForLinux;

        public string ProcessorType => "[TBD]";

        public string ProcessorSerialNumber => "[TBD]";

        public string UniqueID => "[TBD]";

        public string CoprocessorType => "[TBD]";

        public string? CoprocessorOSVersion => "[TBD]";

        public string OSVersion => throw new NotImplementedException();
    }
}
