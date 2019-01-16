using System;
namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Represents a WiFi network.
    /// </summary>
    public class WifiNetwork
    {
        /// <summary>
        /// Name of the network
        /// </summary>
        /// <value>The ssid.</value>
        public string Ssid { get; protected set; }

        /// <summary>
        /// MAC address of the AP
        /// </summary>
        /// <value>The bssid.</value>
        public string Bssid { get; protected set; }

        /// <summary>
        /// Whether or not it's a direct WiFi access point.
        /// </summary>
        /// <value><c>true</c> if is direct; otherwise, <c>false</c>.</value>
        public bool IsDirect { get; protected set; }

        public NetworkType NetworkKind {get; protected set; }

        public TimeSpan BeaconInterval { get; protected set; }

        /// <summary>
        /// RSSI strength in dBmW (decibel milliwatts)
        /// </summary>
        /// <value>The signal strength in dBmW (decibel milliwatts).</value>
        public float SignalDbStrength {
            get; set;
        }

        public PhyType Phy { get; protected set; }

        public NetworkSecuritySettings SecuritySettings { get; protected set; }

        /// <summary>
        /// TimeSpan value representing the value of the Timestamp field from the 802.11 Beacon or Probe Response frame received by the wireless LAN interface.
        /// </summary>
        /// <value>Up time.</value>
        public TimeSpan UpTime { get; set; }

        /// <summary>
        /// The channel center frequency of the band on which the 802.11 Beacon or Probe Response frame was received. The value of this property is in units of kilohertz (kHz). Note that this member is only valid for PHY types that are not frequency-hopping spread spectrum (FHSS). In all other cases the value returned is zero.
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


        public WifiNetwork(string ssid, string bssid, NetworkType kind, PhyType phy, NetworkSecuritySettings securitySettings, int channelFreq, bool isDirect = true)
        {
            this.Ssid = ssid;
            this.Bssid = bssid;
            this.IsDirect = isDirect;
            this.NetworkKind = kind;
            this.Phy = phy;
            this.SecuritySettings = securitySettings;
            this.ChannelCenterFrequency = channelFreq;
        }
    }
}
