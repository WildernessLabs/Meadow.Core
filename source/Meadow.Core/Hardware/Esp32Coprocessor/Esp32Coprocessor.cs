using Meadow.Devices;
using System;
using System.Runtime.InteropServices;
using Meadow.Hardware.Coprocessor;
using static Meadow.Core.Interop;
using Meadow.Hardware.Coprocessor.MessagePayloads;
using System.Net;
using Meadow.Gateway.WiFi;
using Meadow.Gateway;

namespace Meadow.Hardware
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

        #endregion Properties

        #region Constructor(s)

        /// <summary>
        /// Default constructor of the Esp32Coprocessor class.
        /// </summary>
        public Esp32Coprocessor()
        {
            DebugLevel = DebugOptions.None;
        }

        #endregion Constructor(s)

        #region Methods

        /// <summary>
        /// Request the ESP32 to connect to the specified network.
        /// </summary>
        /// <param name="ssid">Name of the network to connect to.</param>
        /// <param name="password">Password for the network.</param>
        /// <param name="reconnection">Should the adapter reconnect automatically?</param>
        /// <exception cref="ArgumentNullException">Thrown if the ssid is null or empty or the password is null.</exception>
        /// <returns>true if the connection was successfully made.</returns>
        public bool StartNetwork(string ssid, string password, ReconnectionType reconnection)
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
            bool connected;

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
                    Function = (UInt32) WiFiFunction.Start,
                    StatusCode = 0,
                    Payload = payloadGcHandle.AddrOfPinnedObject(),
                    PayloadLength = (UInt32) encodedPayload.Length,
                    Result = resultGcHandle.AddrOfPinnedObject(),
                    ResultLength = (UInt32) resultBuffer.Length,
                    Block = 1
                };

                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.Esp32Command, ref command);

                if (result == 0)
                {
                    byte[] addressBytes = new byte[4];
                    Array.Copy(resultBuffer, addressBytes, addressBytes.Length);
                    IpAddress = new IPAddress(addressBytes);
                    Array.Copy(resultBuffer, 4, addressBytes, 0, addressBytes.Length);
                    SubnetMask = new IPAddress(addressBytes);
                    Array.Copy(resultBuffer, 8, addressBytes, 0, addressBytes.Length);
                    Gateway = new IPAddress(addressBytes);
                    connected = true;
                }
                else
                {
                    byte[] addressBytes = new byte[4];
                    Array.Clear(addressBytes, 0, addressBytes.Length);
                    IpAddress = new IPAddress(addressBytes);
                    SubnetMask = new IPAddress(addressBytes);
                    Gateway = new IPAddress(addressBytes);
                    connected = false;
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
            return (connected);
        }

        #endregion Methods
    }
}