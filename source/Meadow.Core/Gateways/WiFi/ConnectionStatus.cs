using System;

namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Describes the status of a WiFi network connection.
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// Connection failed for a reason other than those in this list.
        /// </summary>
        UnspecifiedFailure = 0,

        /// <summary>
        /// Connection succeeded.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Connection failed because access to the network has been revoked.
        /// </summary>
        AccessRevoked = 2,

        /// <summary>
        /// Connection failed because an invalid credential was presented.
        /// </summary>
        InvalidCredential = 3,

        /// <summary>
        /// Connection failed because the network is not available.
        /// </summary>
        NetworkNotAvailable = 4,

        /// <summary>
        /// Connection failed because the connection attempt timed out.
        /// </summary>
        Timeout = 5,

        /// <summary>
        /// Connection failed because the authentication protocol is not supported.
        /// </summary>
        UnsupportedAuthenticationProtocol = 6
    }
}
