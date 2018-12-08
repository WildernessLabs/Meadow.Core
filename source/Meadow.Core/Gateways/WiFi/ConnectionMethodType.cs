using System;
namespace Meadow.Gateway.WiFi
{
    public enum ConnectionMethodType
    {
        /// <summary>
        /// Default.
        /// </summary>
        Default = 0,

        /// <summary>
        /// WPS pin.
        /// </summary>
        WpsPin = 1,

        /// <summary>
        /// WPS protected setup.
        /// </summary>
        WpsPushButton = 2
    }
}
