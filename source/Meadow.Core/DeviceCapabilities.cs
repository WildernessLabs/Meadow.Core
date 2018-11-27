using System;
namespace Meadow
{
    public class DeviceCapabilities
    {
        public NetworkCapabilities Network
        {
            get; protected set;
        }

        public DeviceCapabilities()
        {
        }


    }

    public class NetworkCapabilities
    {
        public bool HasWiFi { get; protected set; }
        public bool HasEthernet { get; protected set; }
    }
}

