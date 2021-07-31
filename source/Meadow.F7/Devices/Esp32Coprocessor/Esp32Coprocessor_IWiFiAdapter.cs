using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Gateways;
using Meadow.Gateway.WiFi;
using Meadow.Gateways.Exceptions;

namespace Meadow.Devices
{
    /// <summary>
	/// This file holds the WiFi specific methods, properties etc for the IWiFiAdapter interface.
	/// </summary>
    public partial class Esp32Coprocessor : IWiFiAdapter
    {
        #region Events

        /// <summary>
        /// Raised when the device connects to WiFi.
        /// </summary>
        public event EventHandler WiFiConnected = delegate { };

        /// <summary>
        /// Raised when the device disconnects from WiFi.
        /// </summary>
        public event EventHandler WiFiDisconnected = delegate { };

        /// <summary>
        /// Raised when the WiFi interface starts.
        /// </summary>
        public event EventHandler WiFiInterfaceStarted = delegate { };

        /// <summary>
        /// Raised when the WiFi interface stops.
        /// </summary>
        public event EventHandler WiFiInterfaceStopped = delegate { };

        #endregion Events


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
        /// IP Address of the network adapter.
        /// </summary>
        public IPAddress IpAddress { get; private set; }

        /// <summary>
        /// Subnet mask of the adapter.
        /// </summary>
        public IPAddress SubnetMask { get; private set; }

        /// <summary>
        /// Default gateway for the adapter.
        /// </summary>
        public IPAddress Gateway { get; private set; }

        /// <summary>
        /// Record if the WiFi ESP32 is connected to an access point.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Current onboard antenna in use.
        /// </summary>
        public AntennaType Antenna
        {
            get
            {
                CheckStatus();
                return (_antenna);
            }
        }
        protected AntennaType _antenna;

        /// <summary>
        /// Should the system automatically get the time when the board is connected to an access point?
        /// </summary>
        public bool GetNetworkTimeAtStartup
        {
            get
            {
                CheckStatus();
                return (_getNetworkTimeAtStartup);
            }
        }
        protected bool _getNetworkTimeAtStartup;

        /// <summary>
        /// Name of the NTP server to use for time retrieval.
        /// </summary>
        public string NtpServer
        {
            get
            {
                CheckStatus();
                return (_ntpServer);
            }
        }
        protected string _ntpServer = string.Empty;

        /// <summary>
        /// Get the device name.
        /// </summary>
        /// <remarks>
        /// This value should be changed through the meadow.cfg file.
        /// </remarks>
        public string DeviceName
        {
            get
            {
                CheckStatus();
                return (_deviceName);
            }
        }
        protected string _deviceName = string.Empty;

        /// <summary>
        /// MAC address as used by the ESP32 when acting as a client.
        /// </summary>
        public byte[] MacAddress
        {
            get
            {
                CheckStatus();
                return (_macAddress);
            }

        }
        protected byte[] _macAddress = new byte[0];

        /// <summary>
        /// MAC address as used by the ESP32 when acting as an access point.
        /// </summary>
        public byte[] ApMacAddress
        {
            get
            {
                CheckStatus();
                return (_apMacAddress);
            }
        }
        protected byte[] _apMacAddress = new byte[0];

        /// <summary>
        /// Automatically start the network interface when the board reboots?
        /// </summary>
        /// <remarks>
        /// This will automatically connect to any preconfigured access points if they are available.
        /// </remarks>
        public bool AutomaticallyStartNetwork
        {
            get
            {
                CheckStatus();
                return (_automaticallyStartNetwork);
            }
        }
        protected bool _automaticallyStartNetwork;

        /// <summary>
        /// Automatically try to reconnect to an access point if there is a problem / disconnection?
        /// </summary>
        public bool AutomaticallyReconnect
        {
            get
            {
                CheckStatus();
                return (_automaticallyReconect);
            }
        }
        protected bool _automaticallyReconect;

        /// <summary>
        /// Default access point to try to connect to if the network interface is started and the board
        /// is configured to automatically reconnect.
        /// </summary>
        public string DefaultAcessPoint
        {
            get
            {
                CheckStatus();
                return (_defaultAccessPoint);
            }
        }
        protected string _defaultAccessPoint = string.Empty;

        /// <summary>
        /// Does the access point the WiFi adapter is currently connected to have internet access?
        /// </summary>
        public bool HasInternetAccess { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Check to if the coprocessor is ready and throw an exception if it is not.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the coprocessor has not completed setup or if it is sleeping.</exception>
        private void CheckStatus()
        {
            if (Status != ICoprocessor.CoprocessorState.Ready)
            {
                throw new InvalidOperationException("Coprocessor is not ready or is sleeping.");
            }
        }

        /// <summary>
        /// Delay (in milliseconds) between network scans.
        /// </summary>
        /// <remarks>
        /// This will default to the <see cref="DEFAULT_SCAN_FREQUENCY"/> value.
        ///
        /// The ScanFrequency should be between <see cref="MINIMUM_SCAN_FREQUENCY"/> and
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
        /// Use the event data to work out which event to invoke and create any event args that will be consumed.
        /// </summary>
        /// <param name="eventId">Event ID.</param>
        /// <param name="statusCode">Status of the event.</param>
        /// <param name="payload">Optional payload containing data specific to the result of the event.</param>
        protected void InvokeEvent(WiFiFunction eventId, StatusCodes statusCode, byte[] payload)
        {
            switch (eventId)
            {
                case WiFiFunction.ConnectToAccessPointEvent:
                    RaiseWiFiConnected(statusCode, payload);
                    break;
                case WiFiFunction.DisconnectFromAccessPointEvent:
                    RaiseWiFiDisconnected(statusCode, payload);
                    break;
                case WiFiFunction.StartWiFiInterfaceEvent:
                    RaiseWiFiInterfaceStarted(statusCode, payload);
                    break;
                case WiFiFunction.StopWiFiInterfaceEvent:
                    RaiseWiFiInterfaceStopped(statusCode, payload);
                    break;
                default:
                    throw new NotImplementedException($"WiFi event not implemented ({eventId}).");
            }
        }

        /// <summary>
        /// Clear the IP address, subnet mask and gateway details.
        /// </summary>
        private void ClearIpDetails()
        {
            byte[] addressBytes = new byte[4];
            Array.Clear(addressBytes, 0, addressBytes.Length);
            IpAddress = new IPAddress(addressBytes);
            SubnetMask = new IPAddress(addressBytes);
            Gateway = new IPAddress(addressBytes);
        }

        // TODO: this really should probably not return the networks, but rather
        // populate the collection that it used to do.
        //
        //  MS: Or should it?  The list of available networks is only valid at the point it is collected.
        //      Having the list of networks in this class suggests that they are availalbe now and in say 2 hours time.
        //      This might not be the case.
        //  BC: Yeah, you're right. But it should return just an `IList<Network>` then. And be async.
        /// <summary>
        /// Scan for networks.
        /// </summary>
        /// <remarks>
        /// The network must be started before this method can be called.
        /// </remarks>
        public ObservableCollection<WifiNetwork> Scan()
        {
            var networks = new ObservableCollection<WifiNetwork>();
            byte[] resultBuffer = new byte[MAXIMUM_SPI_BUFFER_LENGTH];
            StatusCodes result = SendCommand((byte) Esp32Interfaces.WiFi, (UInt32) WiFiFunction.GetAccessPoints, true, resultBuffer);
            if (result == StatusCodes.CompletedOk)
            {
                var accessPointList = Encoders.ExtractAccessPointList(resultBuffer, 0);
                var accessPoints = new AccessPoint[accessPointList.NumberOfAccessPoints];

                if (accessPointList.NumberOfAccessPoints > 0)
                {
                    int accessPointOffset = 0;
                    for (int count = 0; count < accessPointList.NumberOfAccessPoints; count++)
                    {
                        var accessPoint = Encoders.ExtractAccessPoint(accessPointList.AccessPoints, accessPointOffset);
                        accessPointOffset += Encoders.EncodedAccessPointBufferSize(accessPoint);
                        string bssid = "";
                        for (int index = 0; index < accessPoint.Bssid.Length; index++)
                        {
                            bssid += accessPoint.Bssid[index].ToString("x2");
                            if (index != accessPoint.Bssid.Length - 1)
                            {
                                bssid += ":";
                            }
                        }
                        var network = new WifiNetwork(accessPoint.Ssid, bssid, NetworkType.Infrastructure, PhyType.Unknown,
                            new NetworkSecuritySettings((NetworkAuthenticationType)accessPoint.AuthenticationMode, NetworkEncryptionType.Unknown),
                            accessPoint.PrimaryChannel, (NetworkProtocol)accessPoint.Protocols, accessPoint.Rssi);
                        networks.Add(network);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error getting access points: {result}");
            }
            return (networks);
        }

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
        /// Stored Configuration Present NOTE NOT IMPLEMENTED IN THIS RELEASE
        /// If a default access point (and optional password) are stored in the adapter then the network
        /// interface and the system is set to connect at startup then the system will then attempt to
        /// connect to the specified access point.
        ///
        /// In this case, the return result indicates if the interface was started successfully and a
        /// connection to the access point was made.
        /// </remarks>
        /// <returns>true if the adapter was started successfully, false if there was an error.</returns>
        public bool StartWiFiInterface()
        {
            StatusCodes result = SendCommand((byte) Esp32Interfaces.WiFi, (UInt32) WiFiFunction.StartWiFiInterface, true, null);
            return (result == StatusCodes.CompletedOk);
        }

        /// <summary>
        /// Stop the WiFi interface,
        /// </summary>
        /// <remarks>
        /// Stopping the WiFi interface will release all resources associated with the WiFi running on the ESP32.
        ///
        /// Errors could occur if the adapter was not started.
        /// </remarks>
        /// <returns>true if the adapter was successfully turned off, false if there was a problem.</returns>
        public bool StopWiFiInterface()
        {
            StatusCodes result = SendCommand((byte) Esp32Interfaces.WiFi, (UInt32) WiFiFunction.StopWiFiInterface, true, null);
            return (result == StatusCodes.CompletedOk);
        }

        /// <summary>
        /// Connect to a WiFi network.
        /// </summary>
        /// <param name="ssid">SSID of the network to connect to</param>
        /// <param name="password">Password for the WiFi access point.</param>
        /// <param name="reconnection">Determine if the adapter should automatically attempt to reconnect (see <see cref="ReconnectionType"/>) to the access point if it becomes disconnected for any reason.</param>
        /// <returns></returns>
        //TODO: we should probably be using some sort of password credential and secure storage see: https://docs.microsoft.com/en-us/uwp/api/windows.security.credentials.passwordcredential
        public async Task<ConnectionResult> Connect(string ssid, string password, ReconnectionType reconnection = ReconnectionType.Automatic)
        {
            var t = await Task.Run<ConnectionResult>(() => {
                ConnectionResult connectionResult;
                StatusCodes result = ConnectToAccessPoint(ssid, password, reconnection);
                switch (result)
                {
                    case StatusCodes.CompletedOk:
                        connectionResult = new ConnectionResult(ConnectionStatus.Success);
                        break;
                    case StatusCodes.AuthenticationFailed:
                    case StatusCodes.CannotConnectToAccessPoint:
                        connectionResult = new ConnectionResult(ConnectionStatus.ConnectionRefused);
                        break;
                    case StatusCodes.CannotStartNetworkInterface:
                        connectionResult = new ConnectionResult(ConnectionStatus.NetworkInterfaceCannotBeStarted);
                        break;
                    case StatusCodes.AccessPointNotFound:
                    case StatusCodes.EspWiFiInvalidSsid:
                        connectionResult = new ConnectionResult(ConnectionStatus.NetworkNotAvailable);
                        break;
                    case StatusCodes.Timeout:
                        connectionResult = new ConnectionResult(ConnectionStatus.Timeout);
                        break;
                    case StatusCodes.ConnectionFailed:
                        connectionResult = new ConnectionResult(ConnectionStatus.ConnectionRefused);
                        break;
                    case StatusCodes.UnmappedErrorCode:
                    default:
                        connectionResult = new ConnectionResult(ConnectionStatus.UnspecifiedFailure);
                        break;
                }
                HasInternetAccess = (result == StatusCodes.CompletedOk);
                return connectionResult;
            });
            return t;
        }

        /// <summary>
        /// Request the ESP32 to connect to the specified network.
        /// </summary>
        /// <param name="ssid">Name of the network to connect to.</param>
        /// <param name="password">Password for the network.</param>
        /// <param name="reconnection">Should the adapter reconnect automatically?</param>
        /// <exception cref="ArgumentNullException">Thrown if the ssid is null or empty or the password is null.</exception>
        /// <returns>true if the connection was successfully made.</returns>
        private StatusCodes ConnectToAccessPoint(string ssid, string password, ReconnectionType reconnection)
        {
            if (string.IsNullOrEmpty(ssid))
            {
                throw new ArgumentNullException("Invalid SSID.");
            }
            if (password == null)
            {
                throw new ArgumentNullException($"{nameof(password)} cannot be null.");
            }

            WiFiCredentials request = new WiFiCredentials()
            {
                NetworkName = ssid,
                Password = password
            };
            byte[] encodedPayload = Encoders.EncodeWiFiCredentials(request);
            byte[] resultBuffer = new byte[MAXIMUM_SPI_BUFFER_LENGTH];

            // TODO: should be async and awaited
            StatusCodes result = SendCommand((byte) Esp32Interfaces.WiFi, (UInt32) WiFiFunction.ConnectToAccessPoint, true, encodedPayload, resultBuffer);

            ClearIpDetails();
            IsConnected = false;
            ConnectEventData data;
            switch (result)
            {
                case StatusCodes.CompletedOk:
                    data = Encoders.ExtractConnectEventData(resultBuffer, 0);
                    IpAddress = new IPAddress(data.IpAddress);
                    SubnetMask = new IPAddress(data.SubnetMask);
                    Gateway = new IPAddress(data.Gateway);
                    IsConnected = true;
                    break;
                case StatusCodes.WiFiDisconnected:
                    data = Encoders.ExtractConnectEventData(resultBuffer, 0);
                    switch ((WiFiReasons) data.Reason)
                    {
                        case WiFiReasons.AuthenticationFailed:
                            result = StatusCodes.AuthenticationFailed;
                            break;
                        case WiFiReasons.NoAccessPointFound:
                            result = StatusCodes.AccessPointNotFound;
                            break;
                        case WiFiReasons.FourWayHandshakeTimeout:
                        case WiFiReasons.ConnectionFailed:
                            result = StatusCodes.ConnectionFailed;
                            break;
                        default:
                            result = StatusCodes.ConnectionFailed;
                            break;
                    }
                    break;
                case StatusCodes.CoprocessorNotResponding:
                    throw new InvalidNetworkOperationException("ESP32 coprocessor is not responding.");
            }
            return (result);
        }

        /// <summary>
        /// Disconnect from the current access point.
        /// </summary>
        /// <param name="turnOffWiFiInterface">Stop the WiFi interface.</param>
        /// <returns></returns>
        public async Task<ConnectionResult> Disconnect(bool turnOffWiFiInterface)
        {
            throw new NotImplementedException();

            //var t = await Task.Run<ConnectionResult>(() => {
            //    ConnectionResult connectionResult;
            //    StatusCodes result = DisconnectFromAccessPoint(turnOffWiFiInterface);
            //    connectionResult = new ConnectionResult(ConnectionStatus.Success);
            //    return (connectionResult);
            //});
            //return (t);
        }

        /// <summary>
        /// Disconnect from the the currently active access point.
        /// </summary>
        /// <remarks>
        /// Setting turnOffWiFiInterface to true will call <cref="StopWiFiInterface" /> following
        /// the disconnection from the current access point.
        /// </remarks>
        /// <param name="turnOffWiFiInterface">Should the WiFi interface be turned off?</param>
        private StatusCodes DisconnectFromAccessPoint(bool turnOffWiFiInterface)
        {
            throw new NotImplementedException();
            //return (StatusCodes.CompletedOk);
        }

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
        public void SetAntenna(AntennaType antenna, bool persist = true)
        {
            if (antenna == AntennaType.NotKnown)
            {
                throw new ArgumentException("Setting the antenna type NotKnown is not allowed.");
            }

            CheckStatus();
            SetAntennaRequest request = new SetAntennaRequest();
            if (persist)
            {
                request.Persist = 1;
            }
            else
            {
                request.Persist = 0;
            }
            if (antenna == AntennaType.OnBoard)
            {
                request.Antenna = (byte) AntennaTypes.OnBoard;
            }
            else
            {
                request.Antenna = (byte) AntennaTypes.External;
            }
            byte[] encodedPayload = Encoders.EncodeSetAntennaRequest(request);
            byte[] encodedResult = new byte[4000];
            StatusCodes result = SendCommand((byte) Esp32Interfaces.WiFi, (UInt32) WiFiFunction.SetAntenna, true, encodedPayload, encodedResult);
            if (result == StatusCodes.CompletedOk)
            {
                CheckStatus();
                _antenna = antenna;
            }
            else
            {
                throw new Exception("Failed to change the antenna in use.");
            }
        }

        public void SetGetNetworkTimeAtStartup(bool getNetworkTimeAtStartup)
        {
            throw new NotImplementedException($"Method {nameof(SetGetNetworkTimeAtStartup)} is not currently implemented.");
        }

        public void SetNtpServer(string ntpServer)
        {
            throw new NotImplementedException($"Method {nameof(SetNtpServer)} is not currently implemented.");
        }

        public void SetDeviceName(string deviceName)
        {
            throw new NotImplementedException($"Method {nameof(SetDeviceName)} is not currently implemented.");
        }

        public void SetMacAddress(byte[] macAddress)
        {
            throw new NotImplementedException($"Method {nameof(SetMacAddress)} is not currently implemented.");
        }

        public void SetApMacAddress(byte[] apMacAddress)
        {
            throw new NotImplementedException($"Method {nameof(SetApMacAddress)} is not currently implemented.");
        }

        public void SetAutomaticallyStartNetwork(bool automaticallyStartNetwork)
        {
            throw new NotImplementedException($"Method {nameof(SetAutomaticallyStartNetwork)} is not currently implemented.");
        }

        public void SetAutomatiacallyReconnect(bool automaticallyReconnect)
        {
            throw new NotImplementedException($"Method {nameof(SetAutomatiacallyReconnect)} is not currently implemented.");
        }

        public void SetDefaultAccessPoint(string defaultAccessPoint)
        {
            throw new NotImplementedException($"Method {nameof(SetDefaultAccessPoint)} is not currently implemented.");
        }

        #endregion Methods

        #region Event raising methods

        /// <summary>
        /// Process the ConnectionCompleted event extracing any event data from the
        /// payload and create an EventArg object if necessary
        /// </summary>
        /// <param name="statusCode">Status code for the WiFi connection</param>
        /// <param name="payload">Event data encoded in the payload.</param>
        protected void RaiseWiFiConnected(StatusCodes statusCode, byte[] payload)
        {
            ConnectEventData connectEventData = Encoders.ExtractConnectEventData(payload, 0);
            IPAddress ip = new IPAddress(connectEventData.IpAddress);
            IPAddress subnet = new IPAddress(connectEventData.SubnetMask);
            IPAddress gateway = new IPAddress(connectEventData.Gateway);
            string ssid = connectEventData.Ssid;
            string bssid = BitConverter.ToString(connectEventData.Bssid).Replace("-", ":");
            byte channel = connectEventData.Channel;
            NetworkAuthenticationType authenticationType = (NetworkAuthenticationType) connectEventData.AuthenticationMode;
            WiFiConnectEventArgs ea = new WiFiConnectEventArgs(ip, subnet, gateway, ssid, bssid, channel, authenticationType, statusCode);
            WiFiConnected?.Invoke(this, ea);
        }

        /// <summary>
        /// Process the Disconnected event extracing any event data from the
        /// payload and create an EventArg object if necessary
        /// </summary>
        /// <param name="statusCode">Status code for the WiFi disconnection request.</param>
        /// <param name="payload">Event data encoded in the payload.</param>
        protected void RaiseWiFiDisconnected(StatusCodes statusCode, byte[] payload)
        {
            EventArgs e = EventArgs.Empty;
            WiFiDisconnected?.Invoke(this, e);
        }

        /// <summary>
        /// Process the InterfaceStarted event extracing any event data from the
        /// payload and create an EventArg object if necessary
        /// </summary>
        /// <param name="statusCode">Status code for the WiFi interface start event (should be CompletedOK).</param>
        /// <param name="payload">Event data encoded in the payload.</param>
        protected void RaiseWiFiInterfaceStarted(StatusCodes statusCode, byte[] payload)
        {
            WiFiInterfaceStartedEventArgs e = new WiFiInterfaceStartedEventArgs(statusCode);
            WiFiInterfaceStarted?.Invoke(this, e);
        }

        /// <summary>
        /// Process the InterfaceStopped event extracing any event data from the
        /// payload and create an EventArg object if necessary
        /// </summary>
        /// <param name="statusCode">Status code for the WiFi interface stop event (should be CompletedOK).</param>
        /// <param name="payload">Event data encoded in the payload.</param>
        protected void RaiseWiFiInterfaceStopped(StatusCodes statusCode, byte[] payload)
        {
            WiFiInterfaceStoppedEventArgs e = new WiFiInterfaceStoppedEventArgs(statusCode);
            WiFiInterfaceStopped?.Invoke(this, e);
        }

        #endregion Event raising methods
    }
}
