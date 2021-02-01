using Meadow.Devices;
using System;
using System.Runtime.InteropServices;
using static Meadow.Core.Interop;
using Meadow.Devices.Esp32.MessagePayloads;
using System.Net;
using Meadow.Gateway.WiFi;
using Meadow.Gateway;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Meadow.Gateways.Exceptions;

namespace Meadow.Devices
{
    /// <summary>
    ///
    /// </summary>
    public class Esp32Coprocessor : IWiFiAdapter
    {
        #region Constants

        /// <summary>
        /// Maximum length od the SPI buffer that can be used for communication with the ESP32.
        /// </summary>
        private const uint MAXIMUM_SPI_BUFFER_LENGTH = 4000;

        #endregion Constants

        #region Enums

        /// <summary>
        /// Possible debug levels.
        /// </summary>
        [Flags]
        private enum DebugOptions : UInt32 { None = 0x00, Information = 0x01, Errors = 0x02, Full = 0xffffffff }

        #endregion Enums

        #region Private fields / variables

        /// <summary>
        /// Current debug for this class.
        /// </summary>
        /// <remarks>
        /// The flags set in this variable determine the type and amount of output generated when
        /// debugging this class.
        /// </remarks>
        private static DebugOptions _debugLevel;

        /// <summary>
        /// Hold the ESP32 configuration.
        /// </summary>
        private SystemConfiguration? _config = null;

        #endregion Private fields / variables

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
        /// Should the system automatically get the time when the board is connected to an access point?
        /// </summary>
        public bool GetNetworkTimeAtStartup
        {
            get
            {
                if (GetConfiguration() != StatusCodes.CompletedOk)
                {
                    throw new InvalidNetworkOperationException("Cannot retrieve ESP32 configuration.");
                }
                return (_config.Value.GetTimeAtStartup == 1);
            }
            set { SetProperty(ConfigurationItems.GetTimeAtStartup, value); }
        }

        /// <summary>
        /// Name of the NTP server to use for time retrieval.
        /// </summary>
        public string NtpServer
        {
            get
            {
                if (GetConfiguration() != StatusCodes.CompletedOk)
                {
                    throw new InvalidNetworkOperationException("Cannot retrieve ESP32 configuration.");
                }
                return (_config.Value.NtpServer);
            }
            set { SetProperty(ConfigurationItems.NtpServer, value); }
        }

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
                if (GetConfiguration() != StatusCodes.CompletedOk)
                {
                    throw new InvalidNetworkOperationException("Cannot retrieve ESP32 configuration.");
                }
                return (_config.Value.DeviceName);
            }
        }

        /// <summary>
        /// MAC address as used by the ESP32 when acting as a client.
        /// </summary>
        public byte[] MacAddress
        {
            get
            {
                if (GetConfiguration() != StatusCodes.CompletedOk)
                {
                    throw new InvalidNetworkOperationException("Cannot retrieve ESP32 configuration.");
                }
                return (_config.Value.BoardMacAddress);
            }

        }

        /// <summary>
        /// MAC address as used by the ESP32 when acting as an access point.
        /// </summary>
        public byte[] ApMacAddress
        {
            get
            {
                if (GetConfiguration() != StatusCodes.CompletedOk)
                {
                    throw new InvalidNetworkOperationException("Cannot retrieve ESP32 configuration.");
                }
                return (_config.Value.SoftApMacAddress);
            }

        }

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
                if (GetConfiguration() != StatusCodes.CompletedOk)
                {
                    throw new InvalidNetworkOperationException("Cannot retrieve ESP32 configuration.");
                }
                return (_config.Value.AutomaticallyStartNetwork == 1);
            }
            set { SetProperty(ConfigurationItems.NtpServer, value); }
        }

        /// <summary>
        /// Automatically try to reconnect to an access point if there is a problem / disconnection?
        /// </summary>
        public bool AutomaticallyReconnect
        {
            get
            {
                if (GetConfiguration() != StatusCodes.CompletedOk)
                {
                    throw new InvalidNetworkOperationException("Cannot retrieve ESP32 configuration.");
                }
                return (_config.Value.AutomaticallyReconnect == 1);
            }
            set { SetProperty(ConfigurationItems.NtpServer, value); }
        }

        /// <summary>
        /// Default access point to try to connect to if the network interface is started and the board
        /// is configured to automatically reconnect.
        /// </summary>
        public string DefaultAcessPoint
        {
            get
            {
                if (GetConfiguration() != StatusCodes.CompletedOk)
                {
                    throw new InvalidNetworkOperationException("Cannot retrieve ESP32 configuration.");
                }
                return (_config.Value.DefaultAccessPoint);
            }
            set { SetProperty(ConfigurationItems.NtpServer, value); }
        }

        #endregion Properties

        #region Constructor(s)

        /// <summary>
        /// Default constructor of the Esp32Coprocessor class.
        /// </summary>
        internal Esp32Coprocessor()
        {
            _debugLevel = DebugOptions.None;
            IsConnected = false;
            ClearIpDetails();
        }

        #endregion Constructor(s)

        #region Methods


        private void SetProperty(ConfigurationItems item, UInt32 value)
        {
            throw new NotImplementedException("SetProperty is not implemented.");
        }

        private void SetProperty(ConfigurationItems item, byte value)
        {
            UInt32 v = value;
            SetProperty(item, v);
        }

        private void SetProperty(ConfigurationItems item, bool value)
        {
            UInt32 v = 0;
            if (value)
            {
                v = 1;
            }
            SetProperty(item, v);
        }

        private void SetProperty(ConfigurationItems item, string value)
        {
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

        /// <summary>
        /// Send a parameterless command (i.e a command where no payload is required) to the ESP32.
        /// </summary>
        /// <param name="where">Interface the command is destined for.</param>
        /// <param name="function">Command to be sent.</param>
        /// <param name="block">Is this a blocking command?</param>
        /// <param name="encodedResult">4000 byte array to hold any data returned by the command.</param>
        /// <returns>StatusCodes enum indicating if the command was successful or if an error occurred.</returns>
        private StatusCodes SendCommand(byte where, UInt32 function, bool block, byte[] encodedResult)
        {
            return(SendCommand(where, function, block, null, encodedResult));
        }

        /// <summary>
        /// Send a command and its payload to the ESP32.
        /// </summary>
        /// <param name="where">Interface the command is destined for.</param>
        /// <param name="function">Command to be sent.</param>
        /// <param name="block">Is this a blocking command?</param>
        /// <param name="payload">Payload for the command to be executed by the ESP32.</param>
        /// <param name="encodedResult">4000 byte array to hold any data returned by the command.</param>
        /// <returns>StatusCodes enum indicating if the command was successful or if an error occurred.</returns>
        private StatusCodes SendCommand(byte where, UInt32 function, bool block, byte[] payload, byte[] encodedResult)
        {
            var payloadGcHandle = default(GCHandle);
            var resultGcHandle = default(GCHandle);
            StatusCodes result = StatusCodes.CompletedOk;
            try
            {
                payloadGcHandle = GCHandle.Alloc(payload, GCHandleType.Pinned);
                resultGcHandle = GCHandle.Alloc(encodedResult, GCHandleType.Pinned);
                UInt32 payloadLength = 0;
                if (!(payload is null))
                {
                    payloadLength = (UInt32) payload.Length;
                }
                var command = new Nuttx.UpdEsp32Command()
                {
                    Interface = where,
                    Function = function,
                    StatusCode = (UInt32) StatusCodes.CompletedOk,
                    Payload = payloadGcHandle.AddrOfPinnedObject(),
                    PayloadLength = payloadLength,
                    Result = resultGcHandle.AddrOfPinnedObject(),
                    ResultLength = (UInt32) encodedResult.Length,
                    Block = (byte) (block ? 1 : 0)
                };

                int updResult = UPD.Ioctl(Nuttx.UpdIoctlFn.Esp32Command, ref command);
                if (updResult == 0)
                {
                    result = (StatusCodes) command.StatusCode;
                }
                else
                {
                    result = StatusCodes.Failure;
                }
            }
            finally
            {
                if (payloadGcHandle.IsAllocated)
                {
                    payloadGcHandle.Free();
                }
                if (resultGcHandle.IsAllocated)
                {
                    resultGcHandle.Free();
                }
            }
            return (result);
        }

        /// <summary>
        /// Get the configuration data structure from the ESP32.
        /// </summary>
        /// <param name="force">Get the configuration from the ESP32 anyway?  Can be used to refresh the previously retrieved configuration.</param>
        /// <returns>Result of getting the configuration from the ESP32.</returns>
        private StatusCodes GetConfiguration(bool force = false)
        {
            StatusCodes result = StatusCodes.CompletedOk;
            if ((_config is null) || force)
            {
                byte[] encodedResult = new byte[MAXIMUM_SPI_BUFFER_LENGTH];
                result = SendCommand((byte) Esp32Interfaces.System, (UInt32) SystemFunction.GetConfiguration, true, encodedResult);
                if (result == StatusCodes.CompletedOk)
                {
                    _config = Encoders.ExtractSystemConfiguration(encodedResult, 0);
                }
            }
            return (result);
        }

        /// <summary>
        /// Reset the ESP32.
        /// </summary>
        public void Reset()
        {
            SendCommand((byte) Esp32Interfaces.Transport, (UInt32) TransportFunction.ResetEsp32, false, null);
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
        public bool StartNetwork()
        {
            StatusCodes result = SendCommand((byte) Esp32Interfaces.WiFi, (UInt32) WiFiFunction.StartNetwork, true, null);
            return (result == StatusCodes.CompletedOk);
        }

        /// <summary>
        /// Request the ESP32 to connect to the specified network.
        /// </summary>
        /// <param name="ssid">Name of the network to connect to.</param>
        /// <param name="password">Password for the network.</param>
        /// <param name="reconnection">Should the adapter reconnect automatically?</param>
        /// <exception cref="ArgumentNullException">Thrown if the ssid is null or empty or the password is null.</exception>
        /// <returns>true if the connection was successfully made.</returns>
        [Obsolete("StartNetwork(ssid, password, reconnection) is deprecated, please use ConnectToAccessPoint instead.")]
        public bool StartNetwork(string ssid, string password, ReconnectionType reconnection)
        {
            return (ConnectToAccessPoint(ssid, password, reconnection));
        }

        /// <summary>
        /// Request the ESP32 to connect to the specified network.
        /// </summary>
        /// <param name="ssid">Name of the network to connect to.</param>
        /// <param name="password">Password for the network.</param>
        /// <param name="reconnection">Should the adapter reconnect automatically?</param>
        /// <exception cref="ArgumentNullException">Thrown if the ssid is null or empty or the password is null.</exception>
        /// <returns>true if the connection was successfully made.</returns>
        public bool ConnectToAccessPoint(string ssid, string password, ReconnectionType reconnection)
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

            StatusCodes result = SendCommand((byte) Esp32Interfaces.WiFi, (UInt32) WiFiFunction.ConnectToAccessPoint, true, encodedPayload, resultBuffer);

            if ((result == StatusCodes.CompletedOk))
            {
                byte[] addressBytes = new byte[4];
                Array.Copy(resultBuffer, addressBytes, addressBytes.Length);
                IpAddress = new IPAddress(addressBytes);
                Array.Copy(resultBuffer, 4, addressBytes, 0, addressBytes.Length);
                SubnetMask = new IPAddress(addressBytes);
                Array.Copy(resultBuffer, 8, addressBytes, 0, addressBytes.Length);
                Gateway = new IPAddress(addressBytes);
                IsConnected = true;
            }
            else
            {
                ClearIpDetails();
                IsConnected = false;
                if (result == StatusCodes.CoprocessorNotResponding)
                {
                    throw new InvalidNetworkOperationException("ESP32 coprocessor is not responding.");
                }
            }
            return (IsConnected);
        }

        /// <summary>
        /// Get the list of access points.
        /// </summary>
        /// <remarks>
        /// The network must be started before this method can be called.
        /// </remarks>
        /// <returns>ObservableCollection (possibly empty) of access points.</returns>
        public ObservableCollection<WifiNetwork> GetAccessPoints()
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
            return(networks);
        }

        #endregion Methods
    }
}