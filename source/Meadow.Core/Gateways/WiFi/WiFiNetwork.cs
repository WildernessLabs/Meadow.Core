using System;
using Meadow.Hardware;

namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Represents a WiFi network.
    /// </summary>
    public class WifiNetwork
    {
        #region Properties

        /// <summary>
        /// Name of the network
        /// </summary>
        /// <value>The SSID of the WiFi network.</value>
        public string Ssid { get; protected set; }

        /// <summary>
        /// MAC address of the AP
        /// </summary>
        /// <value>The BSSID of the WiFi network.</value>
        public string Bssid { get; protected set; }

        /// <summary>
        /// Type of network (infrastructure, ad-hoc etc.).
        /// </summary>
        public NetworkType TypeOfNetwork {get; protected set; }

        /// <summary>
        /// Beacon interval.
        /// </summary>
        public TimeSpan BeaconInterval { get; protected set; }

        /// <summary>
        /// RSSI strength in dBmW (decibel milliwatts)
        /// </summary>
        /// <value>The signal strength in dBmW (decibel milliwatts).</value>
        public sbyte SignalDbStrength { get; protected set; }

        /// <summary>
        /// Physical network type.
        /// </summary>
        public PhyType Phy { get; protected set; }

        /// <summary>
        /// Security settings for the WiFi network.
        /// </summary>
        public NetworkSecuritySettings SecuritySettings { get; protected set; }

        /// <summary>
        /// Network protocol in use or supported.
        /// </summary>
        public NetworkProtocol Protocol { get; protected set; }

        /// <summary>
        /// TimeSpan value representing the value of the Timestamp field from the 802.11 Beacon or
        /// Probe Response frame received by the wireless LAN interface.
        /// </summary>
        /// <value>Up time.</value>
        public TimeSpan UpTime { get; set; }

        /// <summary>
        /// The channel center frequency of the band on which the 802.11 Beacon or Probe Response frame was
        /// received. The value of this property is in units of kilohertz (kHz). Note that this member is only
        /// valid for PHY types that are not frequency-hopping spread spectrum (FHSS). In all other cases the
        /// value returned is zero.
        /// </summary>
        /// <value>The channel center frequency.</value>
        public int ChannelCenterFrequency { get; protected set; }

        /// <summary>
        /// Gets the signal bar strength.
        /// </summary>
        /// <value>The signal bar strength.</value>
        public byte SignalBarStrength {
            get {
                // TODO: a function of RSSI
                return 0;
            }
        }

        #endregion Properties.

        #region Constructor(s)

        /// <summary>
        /// Default constructor is private to prevent it from being used.
        /// </summary>
        private WifiNetwork()
        {
        }

        /// <summary>
        /// Constructor for WiFi object.
        /// </summary>
        /// <param name="ssid">Name of the network (Service Set Identifier)</param>
        /// <param name="bssid">Section of the WiFi network (Basic Service Set Identifier)</param>
        /// <param name="typeOfNetworkType">Type of network, infrastructure, ad-hoc etc.  (see <see cref="NetworkType"/>).</param>
        /// <param name="phy">Physical network type <see cref="PhyType"/>.</param>
        /// <param name="securitySettings">Security settings for the network.</param>
        /// <param name="channelFreq">Center frequency of the channel is network is using for data transmission.</param>
        /// <param name="protocol">Network protocol is use / supported.</param>
        /// <param name="signalDbStrength">Signal strength for the WiFi network.</param>
        public WifiNetwork(string ssid, string bssid, NetworkType typeOfNetworkType, PhyType phy,
            NetworkSecuritySettings securitySettings, int channelFreq, NetworkProtocol protocol,
            sbyte signalDbStrength)
        {
            Ssid = ssid;
            Bssid = bssid;
            TypeOfNetwork = typeOfNetworkType;
            Phy = phy;
            SecuritySettings = securitySettings;
            ChannelCenterFrequency = channelFreq;
            Protocol = protocol;
            SignalDbStrength = signalDbStrength;
        }

        #endregion Constructor(s)

        #region Methods

        #endregion Methods
    }
}
