using System;

namespace Meadow
{
    public class LinuxPlatformOS : IPlatformOS
    {
        public virtual string OSVersion => "Not implemented";

        public virtual string OSBuildDate => "Not implemented";

        public virtual string MonoVersion => ".NET 5.0"; // TODO"

        public void Initialize()
        {
        }

        public T GetConfigurationValue<T>(IPlatformOS.ConfigurationValues item) where T : struct
        {
            throw new NotImplementedException();
        }

        public void SetConfigurationValue<T>(IPlatformOS.ConfigurationValues item, T value) where T : struct
        {
            throw new NotImplementedException();
        }
    }
}
