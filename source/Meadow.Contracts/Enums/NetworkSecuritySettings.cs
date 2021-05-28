using System;
namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Represents the security setting information of a WiFi network.
    /// </summary>
    public class NetworkSecuritySettings
    {
        #region Properties

        /// <summary>
        /// Type of authentication used by the network, see <see cref="NetworkAuthenticationType"/> for more information.
        /// </summary>
        public NetworkAuthenticationType AuthenticationType { get; protected set; }

        /// <summary>
        /// Type of encryption used by the network, see <see cref="NetworkEncryptionType"/> for more information.
        /// </summary>
        public NetworkEncryptionType EncryptionType { get; protected set; }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Default constructor is private to prevent it from being used.
        /// </summary>
        private NetworkSecuritySettings()
        {
        }

        /// <summary>
        /// Create a new NetworkSecuritySettings object setting the properties accordingly.
        /// </summary>
        /// <param name="authenticationType">Type of authentication used by the network, see <see cref="NetworkAuthenticationType"/> for more information.</param>
        /// <param name="encryptionType">Type of encryption used by the network, see <see cref="NetworkEncryptionType"/> for more information.</param>
        public NetworkSecuritySettings(NetworkAuthenticationType authenticationType, NetworkEncryptionType encryptionType)
        {
            AuthenticationType = authenticationType;
            EncryptionType = encryptionType;
        }
        
        #endregion Constructors
    }
}
