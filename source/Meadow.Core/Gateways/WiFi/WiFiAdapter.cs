using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Meadow.Gateways.Exceptions;
using Meadow.Hardware;

namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Represents a WiFi network adapter.
    /// </summary>
    public class WiFiAdapter
    {
        #region Constants

        /// <summary>
        /// Default delay (in milliseconds) between WiFi network scans <see cref="ScanFrequency"/>.
        /// </summary>
        public const ushort DEFAULT_SCAN_FREQUENCY = 5000;

        /// <summary>
        /// Minimum delay (in milliseconds) that can be used for the <see cref="ScanFrequency"/>.
        /// </summary>
        public const ushort MINIMUM_SCAN_FREQUENCY = 1000;

        /// <summary>
        /// Maximum delay (in milliseconds) that can be used for the <see cref="ScanFrequency"/>.
        /// </summary>
        public const ushort MAXIMUM_SCAN_FREQUENCY = 60000;

        #endregion Constants

        #region Properties

        /// <summary>
        /// Observable collection of <see cref="WifiNetwork"/>s that were detected on the last scan.
        /// </summary>
        public ObservableCollection<WifiNetwork> Networks { get; private set; }

        /// <summary>
        /// Is the WiFi adapter currently connected to an access point?
        /// </summary>
        public bool IsConnected { get { return(NetworkAdapter.IsConnected); } }

        /// <summary>
        /// Does the access point the WiFi adapter is currently connected to have internet access?
        /// </summary>
        public bool HasInternetAccess { get; protected set; }

        /// <summary>
        /// Delay (in milliseconds) between network scans.
        /// </summary>
        /// <remarks>
        /// This will default to the <see cref="DEFAULT_SCAN_FREQUENCY"/> value.
        ///
        /// the ScanFrequency should be between <see cref="MINIMUM_SCAN_FREQUENCY"/> and
        /// <see cref="MAXIMUM_SCAN_FREQUENCY"/> (inclusive).
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Exception is thrown if value is less than <see cref="MINIMUM_SCAN_FREQUENCY"/> or greater than <see cref="MAXIMUM_SCAN_FREQUENCY"/>.</exception>
        private ushort _scanFrequency = DEFAULT_SCAN_FREQUENCY;
        public ushort ScanFrequency
        {
            get { return (_scanFrequency); }
            set
            {
                if ((value < MINIMUM_SCAN_FREQUENCY) || (value > MAXIMUM_SCAN_FREQUENCY))
                {
                    throw new ArgumentOutOfRangeException($"{nameof(ScanFrequency)} should be between {MINIMUM_SCAN_FREQUENCY} and {MAXIMUM_SCAN_FREQUENCY} (inclusive).");
                }
                _scanFrequency = value;
            }
        }

        /// <summary>
        /// Network adapter.
        /// </summary>
        private IWiFiAdapter NetworkAdapter { get; set; }

        /// <summary>
        /// IP Address of the network adapter.
        /// </summary>
        public IPAddress IpAddress { get { return (NetworkAdapter.IpAddress); } }

        /// <summary>
        /// Subnet mask of the adapter.
        /// </summary>
        public IPAddress SubnetMask { get { return (NetworkAdapter.SubnetMask); } }

        /// <summary>
        /// Default gateway for the adapter.
        /// </summary>
        public IPAddress Gateway { get { return (NetworkAdapter.Gateway); } }

        #endregion Properties

        #region Constructor(s)

        /// <summary>
        /// Default constructor private to prevent it from being called.
        /// </summary>
        private WiFiAdapter()
        {
        }

        /// <summary>
        /// Create a new WiFi adapter that implements the <see cref="IWifiAdapter"/> interface.
        /// </summary>
        /// <param name="networkAdapter">Network adapter interface.</param>
        internal WiFiAdapter(IWiFiAdapter networkAdapter)
        {
            NetworkAdapter = networkAdapter;
            Networks = new ObservableCollection<WifiNetwork>();
            HasInternetAccess = false;
        }

        #endregion Constructor(s)

        #region Methods

        /// <summary>
        /// Scan for networks.
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        public void Scan()
        {
            Networks = NetworkAdapter.GetAccessPoints();
            // Task.Run(() => {
            //     Networks.Clear();
            //     //
            //     //  TODO: Make the adapter scan for networks and then populate the Networks property.
            //     //
            //     Thread.Sleep(ScanFrequency);
            // });
        }

        /// <summary>
        /// Connect to a WiFi network.
        /// </summary>
        /// <param name="network"><see cref="WifiNetwork"/> access point to connect to.</param>
        /// <param name="reconnection">Determine if the adapter should automatically attempt to reconnect (see <see cref="ReconnectionType"/>) to the access point if it becomes disconnected for any reason.</param>
        /// <returns></returns>
        public Task<ConnectionResult> Connect(WifiNetwork network, ReconnectionType reconnection = ReconnectionType.Automatic)
        {
            return new Task<ConnectionResult>(() =>
            {
                return new ConnectionResult(ConnectionStatus.Timeout);
            });
        }

        /// <summary>
        /// Connect to a WiFi network.
        /// </summary>
        /// <param name="network"><see cref="WifiNetwork"/> access point to connect to.</param>
        /// <param name="password">Password for the WiFi access point.</param>
        /// <param name="reconnection">Determine if the adapter should automatically attempt to reconnect (see <see cref="ReconnectionType"/>) to the access point if it becomes disconnected for any reason.</param>
        /// <returns></returns>
        //TODO: we should probably be using some sort of password credential and secure storage see: https://docs.microsoft.com/en-us/uwp/api/windows.security.credentials.passwordcredential
        public Task<ConnectionResult> Connect(WifiNetwork network, string password, ReconnectionType reconnection = ReconnectionType.Automatic)
        {
            return new Task<ConnectionResult>(() =>
            {
                return new ConnectionResult(ConnectionStatus.Timeout);
            });
        }

        /// <summary>
        /// Connect to a WiFi network.
        /// </summary>
        /// <param name="ssid">SSID of the network to connect to</param>
        /// <param name="password">Password for the WiFi access point.</param>
        /// <param name="reconnection">Determine if the adapter should automatically attempt to reconnect (see <see cref="ReconnectionType"/>) to the access point if it becomes disconnected for any reason.</param>
        /// <returns></returns>
        //TODO: we should probably be using some sort of password credential and secure storage see: https://docs.microsoft.com/en-us/uwp/api/windows.security.credentials.passwordcredential
        // public Task<ConnectionResult> Connect(string ssid, string password, ReconnectionType reconnection = ReconnectionType.Automatic)
        public ConnectionResult Connect(string ssid, string password, ReconnectionType reconnection = ReconnectionType.Automatic)
        {
            // return new Task<ConnectionResult>(() =>
            // {
                ConnectionResult result;
                if (NetworkAdapter.ConnectToAccessPoint(ssid, password, reconnection))
                {
                    HasInternetAccess = true;
                    result = new ConnectionResult(ConnectionStatus.Success);
                }
                else
                {
                    HasInternetAccess = false;
                    result = new ConnectionResult(ConnectionStatus.UnspecifiedFailure);
                }
                return result;
            // });
        }

        /// <summary>
        /// Connect to a WiFi network.
        /// </summary>
        /// <param name="network"><see cref="WifiNetwork"/> access point to connect to.</param>
        /// <param name="password">Password for the WiFi access point.</param>
        /// <param name="ssid">SSID (Service Set Identifier) for the access point.</param>
        /// <param name="reconnection">Determine if the adapter should automatically attempt to reconnect (see <see cref="ReconnectionType"/>) to the access point if it becomes disconnected for any reason.</param>
        /// <returns></returns>
        public Task<ConnectionResult> Connect(WifiNetwork network, string password, string ssid, ReconnectionType reconnection = ReconnectionType.Automatic)
        {
            return new Task<ConnectionResult>(() =>
            {
                return new ConnectionResult(ConnectionStatus.Timeout);
            });
        }

        /// <summary>
        /// Connect to a WiFi network.
        /// </summary>
        /// <param name="network"><see cref="WifiNetwork"/> access point to connect to.</param>
        /// <param name="password">Password for the WiFi access point.</param>
        /// <param name="ssid">SSID (Service Set Identifier) for the access point.</param>
        /// <param name="connectionMethodType">WiFi connection method (see <see cref="ConnectionMethodType"/>).</param>
        /// <param name="reconnection">Determine if the adapter should automatically attempt to reconnect (see <see cref="ReconnectionType"/>) to the access point if it becomes disconnected for any reason.</param>
        /// <returns></returns>
        public Task<ConnectionResult> Connect(WifiNetwork network, string password, string ssid, ConnectionMethodType connectionMethodType, ReconnectionType reconnection = ReconnectionType.Automatic)
        {
            return new Task<ConnectionResult>(() =>
            {
                return new ConnectionResult(ConnectionStatus.Timeout);
            });
        }

        /// <summary>
        /// Disconnect from the current network.
        /// </summary>
        public void Disconect()
        {
            //  TODO: Determine if we should automatically reconnect and also should we use a retry count?
        }

        #endregion Methods
    }
}
