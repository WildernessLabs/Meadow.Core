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
        Manual = 1

    }
}
