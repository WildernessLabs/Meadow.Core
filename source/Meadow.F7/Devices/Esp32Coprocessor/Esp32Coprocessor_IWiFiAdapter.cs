﻿using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Gateways;
using Meadow.Gateway.WiFi;
using Meadow.Gateways.Exceptions;
using System.Collections.Generic;
using static Meadow.Devices.F7MicroBase;
using System.Threading;

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

        /// <summary>
        /// Raise the NTP time changed event.
        /// </summary>
        public event EventHandler NtpTimeChanged = delegate { };

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

        #region Member variables.

        /// <summary>
        /// Lock object to make sure the events and the methods do not try to access
        /// properties simultaneously.
        /// </summary>
        private object _lock = new object();

        /// <summary>
        /// 
        /// </summary>
        private Semaphore _connectionSemaphore;

        #endregion Member variables.

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
        public AntennaType Antenna => _antenna;
      
        protected AntennaType _antenna;

        /// <summary>
        /// Should the system automatically get the time when the board is connected to an access point?
        /// </summary>
        public bool GetNetworkTimeAtStartup
        {
            get => F7PlatformOS.GetBoolean(IPlatformOS.ConfigurationValues.GetTimeAtStartup);
            set => F7PlatformOS.SetBoolean(IPlatformOS.ConfigurationValues.GetTimeAtStartup, value);
        }

        /// <summary>
        /// MAC address as used by the ESP32 when acting as a client.
        /// </summary>
        public byte[] MacAddress
        {
            get
            {
                byte[] mac = new byte[6];
                F7PlatformOS.GetByteArray(IPlatformOS.ConfigurationValues.MacAddress, mac);
                return mac;
            }

        }

        /// <summary>
        /// MAC address as used by the ESP32 when acting as an access point.
        /// </summary>
        public byte[] ApMacAddress
        {
            get
            {
                byte[] mac = new byte[6];
                F7PlatformOS.GetByteArray(IPlatformOS.ConfigurationValues.SoftApMacAddress, mac);
                return mac;
            }
        }

        /// <summary>
        /// Gets or sets whether to automatically start the network interface when the board reboots.
        /// </summary>
        /// <remarks>
        /// This will automatically connect to any preconfigured access points if they are available.
        /// </remarks>
        public bool AutomaticallyStartNetwork
        {
            get => F7PlatformOS.GetBoolean(IPlatformOS.ConfigurationValues.AutomaticallyStartNetwork);
            set => F7PlatformOS.SetBoolean(IPlatformOS.ConfigurationValues.AutomaticallyStartNetwork, value);
        }

        /// <summary>
        /// Automatically try to reconnect to an access point if there is a problem / disconnection?
        /// </summary>
        public bool AutomaticallyReconnect
        {
            get => F7PlatformOS.GetBoolean(IPlatformOS.ConfigurationValues.AutomaticallyReconnect);
            set => F7PlatformOS.SetBoolean(IPlatformOS.ConfigurationValues.AutomaticallyReconnect, value);
        }

        /// <summary>
        /// Default access point to try to connect to if the network interface is started and the board
        /// is configured to automatically reconnect.
        /// </summary>
        public string DefaultAcessPoint => F7PlatformOS.GetString(IPlatformOS.ConfigurationValues.DefaultAccessPoint);

        /// <summary>
        /// Access point the ESP32 is currently connected to.
        /// </summary>
        public string Ssid { get; private set; }

        /// <summary>
        /// BSSID of the access point the ESP32 is currently connected to.
        /// </summary>
        public string Bssid { get; private set; }

        /// <summary>
        /// WiFi channel the ESP32 and the access point are using for communication.
        /// </summary>
        public uint Channel { get; private set; } 

        /// <summary>
        /// The maximum number of times the ESP32 will retry an operation before returning an error.
        /// </summary>
        /// <remarks>
        /// This property enforces a minimum value of 3.
        /// </remarks>

        public uint MaximumRetryCount
        {
            get => F7PlatformOS.GetUInt(IPlatformOS.ConfigurationValues.MaximumNetworkRetryCount);
            
            set
            {
                uint retryCount = value;
                if (retryCount < 3)
                {
                    retryCount = 3;
                }
                F7PlatformOS.SetUInt(IPlatformOS.ConfigurationValues.MaximumNetworkRetryCount, retryCount);
            }
        }

        /// <summary>
        /// Does the access point the WiFi adapter is currently connected to have internet access?
        /// </summary>
        public bool HasInternetAccess { get; protected set; }

        #endregion Properties

        #region Methods

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
                case WiFiFunction.NtpUpdateEvent:
                    RaiseNtpTimeChangedEvent();
                    break;
                default:
                    throw new NotImplementedException($"WiFi event not implemented ({eventId}).");
            }
        }

        /// <summary>
        /// Clear the IP address, subnet mask and gateway details.
        /// </summary>
        private void ClearNetworkDetails()
        {
            lock (_lock)
            {
                byte[] addressBytes = new byte[4];
                Array.Clear(addressBytes, 0, addressBytes.Length);
                IpAddress = new IPAddress(addressBytes);
                SubnetMask = new IPAddress(addressBytes);
                Gateway = new IPAddress(addressBytes);
                Ssid = string.Empty;
                Bssid = string.Empty;
                Channel = 0;
            }
        }

        // TODO: Mark, this should be async. But i think it requires the `SendCommand()` method to be async.
        /// <summary>
        /// Scan for networks.
        /// </summary>
        /// <remarks>
        /// The network must be started before this method can be called.
        /// </remarks>
        public IList<WifiNetwork> Scan()
        {
            var networks = new List<WifiNetwork>();
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
                            new NetworkSecuritySettings((NetworkAuthenticationType) accessPoint.AuthenticationMode, NetworkEncryptionType.Unknown),
                            accessPoint.PrimaryChannel, (NetworkProtocol) accessPoint.Protocols, accessPoint.Rssi);
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
            var t = await Task.Run<ConnectionResult>(() =>
            {
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
                    case StatusCodes.WiFiAlreadyStarted:
                        connectionResult = new ConnectionResult(ConnectionStatus.AlreadyConnected);
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

            ClearNetworkDetails();
            //
            //  The connection semaphore is used to synchronise the connection event
            //  and the return from this method.  This fixes the problem where the
            //  event could sometimes be fired before this method had the chance to set
            //  the various network properties.
            //
            _connectionSemaphore = new Semaphore(0, 1);

            // TODO: should be async and awaited
            StatusCodes result = SendCommand((byte) Esp32Interfaces.WiFi, (UInt32) WiFiFunction.ConnectToAccessPoint, true, encodedPayload, resultBuffer);
            _connectionSemaphore.WaitOne();

            ConnectEventData data;
            switch (result)
            {
                case StatusCodes.CompletedOk:
                    //
                    //  Nothing to do here as setting properties now dealt with by the connection event.
                    //
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
            var t = await Task.Run<ConnectionResult>(() =>
            {
                StatusCodes result = DisconnectFromAccessPoint(turnOffWiFiInterface);
                ConnectionResult connectionResult;
                switch (result)
                {
                    case StatusCodes.CompletedOk:
                        ClearNetworkDetails();
                        connectionResult = new ConnectionResult(ConnectionStatus.Success);
                        break;
                    case StatusCodes.Failure:
                        connectionResult = new ConnectionResult(ConnectionStatus.UnspecifiedFailure);
                        break;
                    case StatusCodes.EspWiFiNotStarted:
                        connectionResult = new ConnectionResult(ConnectionStatus.WiFiNotStarted);
                        break;
                    default:
                        connectionResult = new ConnectionResult(ConnectionStatus.UnspecifiedFailure);
                        break;
                }
                return (connectionResult);
            });
            return (t);
        }

        /// <summary>
        /// Disconnect from the the currently active access point.
        /// </summary>
        /// <remarks>
        /// Setting turnOffWiFiInterface to true will call StopWiFiInterface following
        /// the disconnection from the current access point.
        /// </remarks>
        /// <param name="turnOffWiFiInterface">Should the WiFi interface be turned off?</param>
        private StatusCodes DisconnectFromAccessPoint(bool turnOffWiFiInterface)
        {
            DisconnectFromAccessPointRequest request = new DisconnectFromAccessPointRequest()
            {
                TurnOffWiFiInterface = (byte) ((turnOffWiFiInterface ? 1 : 0) & 0xff)
            };
            byte[] encodedRequest = Encoders.EncodeDisconnectFromAccessPointRequest(request);

            StatusCodes result = SendCommand((byte) Esp32Interfaces.WiFi, (UInt32) WiFiFunction.DisconnectFromAccessPoint, true, encodedRequest, null);
            return (result);
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
                _antenna = antenna;
            }
            else
            {
                throw new Exception("Failed to change the antenna in use.");
            }
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
            byte channel = 0;

            ConnectEventData connectEventData = Encoders.ExtractConnectEventData(payload, 0);
            lock (_lock)
            {
                IpAddress = new IPAddress(connectEventData.IpAddress);
                SubnetMask = new IPAddress(connectEventData.SubnetMask);
                Gateway = new IPAddress(connectEventData.Gateway);
                Ssid = connectEventData.Ssid;
                Bssid = BitConverter.ToString(connectEventData.Bssid).Replace("-", ":");
                channel = connectEventData.Channel;
                Channel = channel;
                IsConnected = true;
                HasInternetAccess = true;
            }
            _connectionSemaphore.Release();
            NetworkAuthenticationType authenticationType = (NetworkAuthenticationType) connectEventData.AuthenticationMode;

            WiFiConnectEventArgs ea = new WiFiConnectEventArgs(IpAddress, SubnetMask, Gateway, Ssid, Bssid, channel, authenticationType, statusCode);
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
            ClearNetworkDetails();
            HasInternetAccess = false;
            IsConnected = false;

            WiFiDisconnectEventArgs e = new WiFiDisconnectEventArgs(statusCode);
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

        /// <summary>
        /// Process the NtpTimeChanged event.
        /// </summary>
        protected void RaiseNtpTimeChangedEvent()
        {
            NtpTimeChangedEventArgs e = new NtpTimeChangedEventArgs();
            NtpTimeChanged?.Invoke(this, e);
        }

        #endregion Event raising methods
    }
}
