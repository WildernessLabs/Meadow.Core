using System.Net;
using Meadow.Gateway.WiFi;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Meadow.Gateways
{
    public interface IWiFiAdapter
    {
        #region Properties

        /// <summary>
        /// Indicate if the network adapter is connected to an access point.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// IP Address of the network adapter.
        /// </summary>
        IPAddress IpAddress { get; }

        /// <summary>
        /// Subnet mask of the adapter.
        /// </summary>
        IPAddress SubnetMask { get; }

        /// <summary>
        /// Default gateway for the adapter.
        /// </summary>
        IPAddress Gateway { get; }

        /// <summary>
        /// Get the network time when the WiFi adapter starts?
        /// </summary>
        bool GetNetworkTimeAtStartup { get; set; }

        /// <summary>
        /// MAC address of the board when acting as a client.
        /// </summary>
        byte[] MacAddress { get; }

        /// <summary>
        /// MAC address of the board when acting as a sft access point.
        /// </summary>
        byte[] ApMacAddress { get; }

        /// <summary>
        /// Maximum number of times the ESP32 will retry a netowrk operation before returning an error.
        /// </summary>
        uint MaximumRetryCount { get; set; }

        /// <summary>
        /// Current antenna in use.
        /// </summary>
        AntennaType Antenna { get; }

        /// <summary>
        /// Automatically start the network interface when the board reboots?
        /// </summary>
        /// <remarks>
        /// This will automatically connect to any preconfigured access points if they are available.
        /// </remarks>
        bool AutomaticallyStartNetwork { get; set; }

        /// <summary>
        /// Automatically try to reconnect to an access point if there is a problem / disconnection?
        /// </summary>
        bool AutomaticallyReconnect { get; set; }

        /// <summary>
        /// Default access point to try to connect to if the network interface is started and the board
        /// is configured to automatically reconnect.
        /// </summary>
        string DefaultAcessPoint { get; }

        #endregion Properties

        #region Delegates and Events

        /// <summary>
        /// User code to process the ConnectionCompleted event.
        /// </summary>
        event EventHandler WiFiConnected;

        /// <summary>
        /// User code to process the Disconnected event.
        /// </summary>
        event EventHandler WiFiDisconnected;

        /// <summary>
        /// User code to process the InterfaceStarted event.
        /// </summary>
        event EventHandler WiFiInterfaceStarted;

        /// <summary>
        /// User code to process the InterfaceStopped event.
        /// </summary>
        event EventHandler WiFiInterfaceStopped;

        /// <summary>
        /// User code to process the time changed event.
        /// </summary>
        event EventHandler NtpTimeChanged;

        #endregion Delegates and Events

        #region Methods

        /// <summary>
        /// Start the network interface on the WiFi adapter.
        /// </summary>
        /// <remarks>
        /// This method starts the network interface hardware.  The result of this action depends upon the
        /// settings stored in the WiFi adapter memory.
        ///
        /// No Stored Configuration
        /// If no settings are stored in the adapter then the hardware will simply start.  IP addresses
        /// will not be obtained in this mode.
        ///
        /// In this case, the return result indicates if the hardware started successfully.
        ///
        /// Stored Configuration Present
        /// If a default access point (and optional password) are stored in the adapter then the network
        /// interface and the system is set to connect at startup then the system will then attempt to
        /// connect to the specified access point.
        ///
        /// In this case, the return result indicates if the interface was started successfully and a
        /// connection to the access point was made.
        /// </remarks>
        /// <returns>true if the adapter was started successfully, false if there was an error.</returns>
        bool StartWiFiInterface();

        /// <summary>
        /// Stop the WiFi interface,
        /// </summary>
        /// <remarks>
        /// Stopping the WiFi interface will release all resources associated with the WiFi running on the ESP32.
        /// 
        /// Errors could occur if the adapter was not started.
        /// </remarks>
        /// <returns>true if the adapter was successfully turned off, false if there was a problem.</returns>
        bool StopWiFiInterface();

        /// <summary>
        /// Start a WiFi network.
        /// </summary>
        /// <param name="ssid">Name of the network to connect to.</param>
        /// <param name="password">Password for the network.</param>
        /// <param name="reconnection">Should the adapter reconnect automatically?</param>
        /// <exception cref="ArgumentNullException">Thrown if the ssid is null or empty or the password is null.</exception>
        /// <returns>true if the connection was successfully made.</returns>
        Task<ConnectionResult> Connect(string ssid, string password, ReconnectionType reconnection = ReconnectionType.Automatic);

        /// <summary>
        /// Disconnect from the the currently active access point.
        /// </summary>
        /// <remarks>
        /// Setting turnOffWiFiInterface to true will call StopWiFiInterface following
        /// the disconnection from the current access point.
        /// </remarks>
        /// <param name="turnOffWiFiInterface">Should the WiFi interface be turned off?</param>
        Task<ConnectionResult> Disconnect(bool turnOffWiFiInterface);

        /// <summary>
        /// Get the list of access points.
        /// </summary>
        /// <remarks>
        /// The network must be started before this method can be called.
        /// </remarks>
        /// <returns>An `IList` (possibly empty) of access points.</returns>
        IList<WifiNetwork> Scan();

        /// <summary>
        /// Change the current WiFi antenna.
        /// </summary>
        /// <remarks>
        /// Allows the application to change the current antenna used by the WiFi adapter.  This
        /// can be made to persist between reboots / power cycles by setting the persist option
        /// to true.
        /// </remarks>
        /// <param name="antenna">New antenna to use.</param>
        /// <param name="persist">Make the antenna change persistent.</param>
        void SetAntenna(AntennaType antenna, bool persist = true);

        ///// <summary>
        ///// Set the property that indicates if the coporcessor should connect to the default access
        ///// point when the system starts.
        ///// </summary>
        ///// <param name="automaticallyStartNetwork">True for the connection to be tried, false otehrwise.</param>
        ///// <returns>True if the property was set, flase if there was a problem.</returns>
        //bool SetAutomaticallyStartNetowrk(bool automaticallyStartNetwork);

        ///// <summary>
        ///// Set the property that indicates if the system should automatically attempt to reconnect to an access
        ///// point should the system diconnect.
        ///// </summary>
        ///// <remarks>
        ///// This property will use the <seealso cref="MaximumRetryCount"/> property to determine how many times a reconnect should be attempted.
        ///// </remarks>
        ///// <param name="automaticallyReconnect">Automatically reconnect to an access point?</param>
        ///// <returns>True if the property was set, flase if there was a problem.</returns>
        //bool SetAutomaticallyReconnect(bool automaticallyReconnect);

        ///// <summary>
        ///// Should the system attempt to get the time from the configured NTP server.
        ///// </summary>
        ///// <param name="getTimeAtStartup">True to get the network time.</param>
        ///// <returns>True if the property was set, flase if there was a problem.</returns>
        //bool SetGetNetworkTimeAtStartup(bool getTimeAtStartup);

        ///// <summary>
        ///// Set the maximum number of times the coprocessor should retry netowrk operations (on error) before returning an error.
        ///// </summary>
        ///// <param name="maximumRetryCount">Maximum number retries.</param>
        ///// <returns>True if the property was set, flase if there was a problem.</returns>
        //bool SetMaximumRetryCount(uint maximumRetryCount);

        #endregion Methods
    }
}