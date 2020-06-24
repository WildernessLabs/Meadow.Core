using Meadow.Devices;
using System;
using System.Runtime.InteropServices;
using Meadow.Hardware.Coprocessor;
using static Meadow.Core.Interop;
using Meadow.Hardware.Coprocessor.MessagePayloads;

namespace Meadow.Hardware
{
    /// <summary>
    ///
    /// </summary>
    internal static class Esp32Coprocessor
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

        #region Constructor(s)

        /// <summary>
        /// Default constructor of the Esp32Coprocessor class.
        /// </summary>
        static Esp32Coprocessor()
        {
            DebugLevel = DebugOptions.Full;
        }

        #endregion Constructor(s)

        #region Methods

        /// <summary>
        /// Request the ESP32 to connect to the specified network.
        /// </summary>
        /// <param name="ssid">Name of the network to connect to.</param>
        /// <param name="password">Password for the network.</param>
        /// <exception cref="ArgumentNullException">Thrown if the SSID is null or empty, password is null.</exception>
        public static void StartNetwork(string ssid, string password)
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
                    Function = (UInt32) WiFiFunction.Start,
                    StatusCode = 0,
                    Payload = payloadGcHandle.AddrOfPinnedObject(),
                    PayloadLength = (UInt32) encodedPayload.Length,
                    Result = resultGcHandle.AddrOfPinnedObject(),
                    ResultLength = (UInt32) resultBuffer.Length,
                    Block = 1
                };

                Output.WriteLineIf(DebugLevel.HasFlag(DebugOptions.Information), $"Connecting to {ssid}.");
                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.Esp32Command, ref command);

                if (result == 0)
                {
                    Output.WriteLineIf(DebugLevel.HasFlag(DebugOptions.Information), $"IP Address: {resultBuffer[0]}.{resultBuffer[1]}.{resultBuffer[2]}.{resultBuffer[3]}");
                    Output.WriteLineIf(DebugLevel.HasFlag(DebugOptions.Information), $"Subnet mask Address: {resultBuffer[4]}.{resultBuffer[5]}.{resultBuffer[6]}.{resultBuffer[7]}");
                    Output.WriteLineIf(DebugLevel.HasFlag(DebugOptions.Information), $"Gateway Address: {resultBuffer[8]}.{resultBuffer[9]}.{resultBuffer[10]}.{resultBuffer[11]}");
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
        }

        #endregion Methods
    }
}