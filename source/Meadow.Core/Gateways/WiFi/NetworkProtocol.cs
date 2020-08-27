using System;

namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Supported WiFi Protocols.
    /// </summary>
    [Flags]
    public enum NetworkProtocol
    {
        /// <summary>
        /// 802.11B protocol.
        /// </summary>
        Protocol11B = 1,
        
        /// <summary>
        /// 802.11G protocol.
        /// </summary>
        Protocol11G = 2,
        
        /// <summary>
        /// 802.11N protocol.
        /// </summary>
        Protocol11N = 4,
        
        /// <summary>
        ///    Low rate supported. 
        /// </summary>
        ProtocolLow = 8
    }
}