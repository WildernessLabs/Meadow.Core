using System;

namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Describes the WiFi network reconnection behavior.
    /// </summary>
    public enum ReconnectionType
    {
        /// <summary>
        /// Reconnect automatically.
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// Allow user to reconnect manually.
        /// </summary>
        Manual = 1,

        /// <summary>
        /// Connect to an access point at startup if access point data is stored on the device.
        ///
        /// Note: This implies Automatic reconnection should a network drop out occur.
        /// </summary>
        ConnectAtStartup = 2
    }
}
