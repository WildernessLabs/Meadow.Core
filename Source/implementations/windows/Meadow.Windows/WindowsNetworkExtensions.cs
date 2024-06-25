using ManagedNativeWifi;
using Meadow.Gateway.WiFi;

namespace Meadow;

internal static class WindowsNetworkExtensions
{
    public static Gateway.WiFi.PhyType ToMeadowPhyType(this ManagedNativeWifi.PhyType phyType)
    {
        return phyType switch
        {
            ManagedNativeWifi.PhyType.He => Gateway.WiFi.PhyType.HE,
            ManagedNativeWifi.PhyType.Vht => Gateway.WiFi.PhyType.Vht,
            ManagedNativeWifi.PhyType.Fhss => Gateway.WiFi.PhyType.Fhss,
            ManagedNativeWifi.PhyType.Dmg => Gateway.WiFi.PhyType.Dmg,
            ManagedNativeWifi.PhyType.Dsss => Gateway.WiFi.PhyType.Dsss,
            ManagedNativeWifi.PhyType.Erp => Gateway.WiFi.PhyType.Erp,
            ManagedNativeWifi.PhyType.HrDsss => Gateway.WiFi.PhyType.Hrdsss,
            ManagedNativeWifi.PhyType.Ht => Gateway.WiFi.PhyType.HT,
            ManagedNativeWifi.PhyType.IrBaseband => Gateway.WiFi.PhyType.IRBaseband,
            ManagedNativeWifi.PhyType.Ofdm => Gateway.WiFi.PhyType.Ofdm,
            _ => Gateway.WiFi.PhyType.Unknown

        };
    }

    public static NetworkType ToNetworkType(this BssType bssType)
    {
        return bssType switch
        {
            BssType.Infrastructure => NetworkType.Infrastructure,
            BssType.Independent => NetworkType.AdHoc,
            _ => NetworkType.Any
        };
    }

    public static NetworkSecuritySettings AsSecuritySettings(this AuthenticationAlgorithm algo, CipherAlgorithm cipher)
    {
        return algo switch
        {
            AuthenticationAlgorithm.Open => new NetworkSecuritySettings(NetworkAuthenticationType.None, NetworkEncryptionType.None),
            AuthenticationAlgorithm.Shared => new NetworkSecuritySettings(NetworkAuthenticationType.Wep, NetworkEncryptionType.Wep40),
            AuthenticationAlgorithm.WPA => new NetworkSecuritySettings(NetworkAuthenticationType.Wpa, NetworkEncryptionType.Unknown),
            AuthenticationAlgorithm.WPA_PSK => new NetworkSecuritySettings(NetworkAuthenticationType.WpaPsk, NetworkEncryptionType.Unknown),
            AuthenticationAlgorithm.WPA3 => new NetworkSecuritySettings(NetworkAuthenticationType.Wpa3Psk, NetworkEncryptionType.Unknown),
            _ => new NetworkSecuritySettings(NetworkAuthenticationType.Unknown, NetworkEncryptionType.Unknown)
        };
    }

    public static sbyte PercentageSignalToEstimatedDB(this int percent)
    {
        // 100 == -30,
        // 75 == -60
        // 50 == -70
        // 25 == -80
        // 0 == -90

        return percent switch
        {
            > 75 => (sbyte)((30 + 100 - percent) * -1),
            > 50 => (sbyte)((60 + (75 - percent) * 0.5) * -1),
            > 25 => (sbyte)((70 + (50 - percent) * 0.5) * -1),
            _ => (sbyte)((80 + (25 - percent) * 0.5) * -1),
        };
    }
}
