using System;
namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Describes the type of WiFi network authentication.
    /// </summary>
    public enum NetworkAuthenticationType
    {
        /// <summary>
        /// No authentication enabled.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Open authentication over 802.11 wireless. Devices are authenticated and can connect to an
        /// access point, but communication with the network requires a matching Wired Equivalent Privacy (WEP) key.
        /// </summary>
        Wep = 1,
        
        /// <summary>
        /// Specifies a Wi-Fi Protected Access (WPA) algorithm that uses pre-shared keys (PSK). IEEE 802.1X port
        /// authorization is performed by the supplicant and authenticator. Cipher keys are dynamically derived
        /// through a pre-shared key that is used on both the supplicant and authenticator.
        /// </summary>
        WpaPsk = 2,
        
        /// <summary>
        /// Specifies a Wi-Fi Protected Access 2 (WPAs) algorithm that uses pre-shared keys (PSK). IEEE 802.1X port
        /// authorization is performed by the supplicant and authenticator. Cipher keys are dynamically derived
        /// through a pre-shared key that is used on both the supplicant and authenticator.
        /// </summary>
        Wpa2Psk = 3,
        
        /// <summary>
        /// WPA PSK or WPA2 PSk encryption.
        /// </summary>
        WpaWpa2Psk = 4,
        
        /// <summary>
        /// WPA Enterprise authentication using a RADIUS server.
        /// </summary>
        Wpa2Enterprise = 5,
        
        /// <summary>
        /// Open authentication over 802.11 wireless. Devices are authenticated and can connect to an
        /// access point, but communication with the network requires a matching Wired Equivalent Privacy (WEP) key.
        /// </summary>
        Open80211 = 6,

        /// <summary>
        /// Specifies an IEEE 802.11 Shared Key authentication algorithm that requires the use of a pre-shared
        /// Wired Equivalent Privacy (WEP) key for the 802.11 authentication.
        /// </summary>
        SharedKey80211 = 7,

        /// <summary>
        /// Specifies a Wi-Fi Protected Access (WPA) algorithm. IEEE 802.1X port authorization is performed by
        /// the supplicant, authenticator, and authentication server. Cipher keys are dynamically derived through
        /// the authentication process.
        /// </summary>
        Wpa = 8,

        /// <summary>
        /// Wi-Fi Protected Access.
        /// </summary>
        WpaNone = 9,

        /// <summary>
        /// Specifies an IEEE 802.11i Robust Security Network Association (RSNA) algorithm. IEEE 802.1X port
        /// authorization is performed by the supplicant, authenticator, and authentication server. Cipher keys
        /// are dynamically derived through the authentication process.
        /// </summary>
        Rsna = 10,

        /// <summary>
        /// Specifies an IEEE 802.11i RSNA algorithm that uses PSK. IEEE 802.1X port authorization is performed
        /// by the supplicant and authenticator. Cipher keys are dynamically derived through a pre-shared key
        /// that is used on both the supplicant and authenticator.
        /// </summary>
        RsnaPsk = 11,

        /// <summary>
        /// Specifies an authentication type defined by an independent hardware vendor (IHV).
        /// </summary>
        Ihv = 12,   
        /// <summary>
        /// Unknown authentication type.
        /// </summary>
        Unknown = 1

    }
}
