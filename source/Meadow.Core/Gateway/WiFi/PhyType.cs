using System;
namespace Meadow.Gateway.WiFi
{
    public enum PhyType
    {
        /// <summary>
        /// Unspecified PHY type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Frequency-hopping, spread-spectrum (FHSS) PHY.
        /// </summary>
        Fhss = 1,

        /// <summary>
        /// Direct sequence, spread-spectrum (DSSS) PHY.
        /// </summary>
        Dsss = 2,

        /// <summary>
        /// Infrared (IR) baseband PHY.
        /// </summary>
        IRBaseband = 3,

        /// <summary>
        /// Orthogonal frequency division multiplex (OFDM) PHY.
        /// </summary>
        Ofdm = 4,

        /// <summary>
        /// High-rated DSSS (HRDSSS) PHY.
        /// </summary>
        Hrdsss = 5,

        /// <summary>
        /// Extended Rate (ERP) PHY.
        /// </summary>
        Erp = 6,

        /// <summary>
        /// High Throughput (HT) PHY for 802.11n PHY.
        /// </summary>
        HT = 7,

        /// <summary>
        /// Very High Throughput (VHT) PHY for 802.11ac PHY.
        /// </summary>
        Vht = 8,

        /// <summary>
        /// Directional multi-gigabit (DMG) PHY for 802.11ad.
        /// </summary>
        Dmg = 9,

        /// <summary>
        /// High-Efficiency Wireless (HEW) PHY for 802.11ax.
        /// </summary>
        HE = 10
    }
}
