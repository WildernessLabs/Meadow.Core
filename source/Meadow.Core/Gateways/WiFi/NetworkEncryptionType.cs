using System;

namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Describes the type of encryption employed by a WiFi network.
    /// </summary>
    public enum NetworkEncryptionType
    {
        /// <summary>
        /// No authentication enabled.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Wired Equivalent Privacy (WEP) 40-bit encryption.
        /// </summary>
        Wep40 = 1,
        
        /// <summary>
        /// Wired Equivalent Privacy (WEP) 104-bit encryption.
        /// </summary>
        Wep104 = 2,
        
        /// <summary>
        /// Temporal Key Integrity Protocol (TKIP).
        /// </summary>
        Tkip = 3,
        
        /// <summary>
        /// CCM Mode Protocol.
        /// </summary>
        Ccmp = 4,
        
        /// <summary>
        /// TKIP or CCMP.
        /// </summary>
        TkipCcmp = 5,

        /// <summary>
        /// Unknown authentication method.
        /// </summary>
        Unknown = 6,
    }
}
