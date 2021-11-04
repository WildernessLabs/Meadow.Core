using System;

namespace Meadow
{
    public partial class F7PlatformOS : IPlatformOS
    {
        public F7PlatformOS()
        {
        }

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
