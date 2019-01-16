using System;
namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Represents the security setting information of a WiFi network.
    /// </summary>
    public class NetworkSecuritySettings
    {
        public NetworkAuthenticationType AuthenticationType { get; protected set; }
        public NetworkEncryptionType EncryptionType { get; protected set; }

        public NetworkSecuritySettings(NetworkAuthenticationType authType, NetworkEncryptionType encryptionType)
        {
            this.AuthenticationType = authType;
            this.EncryptionType = encryptionType;
        }
    }
}
