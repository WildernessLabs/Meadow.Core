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
        private static DebugOptions DebugLevel;

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

        #endregion Properties

        #region Constructor(s)

        /// <summary>
        /// Default constructor of the Esp32Coprocessor class.
        /// </summary>
        internal Esp32Coprocessor()
        {
            DebugLevel = DebugOptions.None;
            IsConnected = false;
        }

        #endregion Constructor(s)

        #region Methods

        /// <summary>
        /// Send a parameterless command (i.e a command where no payload is required) to the ESP32.
        /// </summary>
        /// <param name="where">Interface the command is destined for.</param>
        /// <param name="command">Command to be sent.</param>
        /// <param name="block">Is this a blocking command?</param>
        /// <param name="resultBuffer">4000 byte array to hold any data returned by the command.</param>
        /// <returns>StatusCodes enum indicating if the command was successful or if an error occurred.</returns>
        private StatusCodes SendParameterlessCommand(byte where, UInt32 function, bool block, byte[] resultBuffer)
        {
            byte[] encodedPayload = null;
            var payloadGcHandle = default(GCHandle);
            var resultGcHandle = default(GCHandle);
            StatusCodes result;
            try
            {
                payloadGcHandle = GCHandle.Alloc(encodedPayload, GCHandleType.Pinned);
                resultGcHandle = GCHandle.Alloc(resultBuffer, GCHandleType.Pinned);
                var command = new Nuttx.UpdEsp32Command()
                {
                    Interface = where,
                    Function = function,
                    StatusCode = (UInt32) StatusCodes.CompletedOk,
                    Payload = payloadGcHandle.AddrOfPinnedObject(),
                    PayloadLength = 0,
                    Result = resultGcHandle.AddrOfPinnedObject(),
                    ResultLength = (UInt32) resultBuffer.Length,
                    Block = (byte) (block ? 1 : 0)
                };

                if (UPD.Ioctl(Nuttx.UpdIoctlFn.Esp32Command, ref command) != 0)
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
            return(result);
        }

        /// <summary>
        /// Reset the ESP32.
        /// </summary>
        public void Reset()
        {
            SendParameterlessCommand((byte) Esp32Interfaces.Transport, (UInt32) TransportFunction.ResetEsp32, false, null);
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
        /// <returns>true if teh adapter was started successfully, false if there was an error.</returns>
        public bool StartNetwork()
        {
            StatusCodes result = SendParameterlessCommand((byte) Esp32Interfaces.WiFi, (UInt32) WiFiFunction.StartNetwork, true, null);
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

            var payloadGcHandle = default(GCHandle);
            var resultGcHandle = default(GCHandle);

            try
            {
                WiFiCredentials request = new WiFiCredentials()
                {
                    NetworkName = ssid,
                    Password = password
                };
                byte[] encodedPayload = Encoders.EncodeWiFiCredentials(request);
                byte[] resultBuffer = new byte[4000];

                payloadGcHandle = GCHandle.Alloc(encodedPayload, GCHandleType.Pinned);
                resultGcHandle = GCHandle.Alloc(resultBuffer, GCHandleType.Pinned);

                var command = new Nuttx.UpdEsp32Command()
                {
                    Interface = (byte) Esp32Interfaces.WiFi,
                    Function = (UInt32) WiFiFunction.ConnectToAccessPoint,
                    StatusCode = (UInt32) StatusCodes.CompletedOk,
                    Payload = payloadGcHandle.AddrOfPinnedObject(),
                    PayloadLength = (UInt32) encodedPayload.Length,
                    Result = resultGcHandle.AddrOfPinnedObject(),
                    ResultLength = (UInt32) resultBuffer.Length,
                    Block = 1
                };

                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.Esp32Command, ref command);

                if ((result == 0) && (command.StatusCode == (UInt32) StatusCodes.CompletedOk))
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
                    byte[] addressBytes = new byte[4];
                    Array.Clear(addressBytes, 0, addressBytes.Length);
                    IpAddress = new IPAddress(addressBytes);
                    SubnetMask = new IPAddress(addressBytes);
                    Gateway = new IPAddress(addressBytes);
                    IsConnected = false;
                    if (command.StatusCode == (UInt32) StatusCodes.CoprocessorNotResponding)
                    {
                        throw new InvalidNetworkOperationException("ESP32 coprocessor is not responding.");
                    }
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
            if (!IsConnected)
            {
                throw new InvalidOperationException("Device must be connected to a network before scanning for access points.");
            }

            var networks = new ObservableCollection<WifiNetwork>();
            byte[] resultBuffer = new byte[4000];
            if (SendParameterlessCommand((byte) Esp32Interfaces.WiFi, (UInt32) WiFiFunction.GetAccessPoints, true, resultBuffer) == StatusCodes.CompletedOk)
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
            return(networks);
        }

        #endregion Methods
    }
}