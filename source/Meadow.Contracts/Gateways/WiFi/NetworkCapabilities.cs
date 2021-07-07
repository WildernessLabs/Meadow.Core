namespace Meadow
{
    public class NetworkCapabilities
    {
        public bool HasWiFi { get; protected set; }
        public bool HasEthernet { get; protected set; }

        public NetworkCapabilities(
            bool hasWifi,
            bool hasEthernet) {
            this.HasWiFi = hasWifi;
            this.HasEthernet = hasEthernet;
        }
    }
}

