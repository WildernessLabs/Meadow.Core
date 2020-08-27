using System;
namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Describes teh type of WiFi network.
    /// </summary>
    public enum NetworkType
    {
        /// <summary>
        /// Any type of network.
        /// </summary>
        Any = 0,
        
        /// <summary>
        /// Infrastructure network (all devices communicate through an adapter / router.
        /// </summary>
        Infrastructure = 1,
        
        /// <summary>
        /// Ad-hoc (peer-to-peer) network.
        /// </summary>
        AdHoc = 2
    }
}
