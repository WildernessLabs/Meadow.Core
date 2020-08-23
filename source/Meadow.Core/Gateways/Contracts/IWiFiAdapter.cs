using System.Net;
using Meadow.Gateway.WiFi;

namespace Meadow.Gateway
{
    public interface IWiFiAdapter
    {
        /// <summary>
        /// Start a WiFi network.
        /// </summary>
        /// <param name="ssid">Name of the network to connect to.</param>
        /// <param name="password">Password for the network.</param>
        /// <param name="reconnection">Should the adapter reconnect automatically?</param>
        /// <exception cref="ArgumentNullException">Thrown if the ssid is null or empty or the password is null.</exception>
        /// <returns>true if the connection was successfully made.</returns>
        bool StartNetwork(string ssid, string password, ReconnectionType reconnection);
    }
}