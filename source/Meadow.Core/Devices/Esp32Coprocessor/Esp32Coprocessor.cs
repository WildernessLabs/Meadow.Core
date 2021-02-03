using System;
using System.Runtime.InteropServices;
using static Meadow.Core.Interop;
using Meadow.Devices.Esp32.MessagePayloads;

namespace Meadow.Devices
{
    /// <summary>
    ///
    /// </summary>
    public class Esp32Coprocessor
    {
        #region Constants

        /// <summary>
        /// Maximum length od the SPI buffer that can be used for communication with the ESP32.
        /// </summary>
        public const uint MAXIMUM_SPI_BUFFER_LENGTH = 4000;

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
        protected SystemConfiguration? _config = null;

        #endregion Private fields / variables

        #region Properties

        #endregion Properties

        #region Constructor(s)

        /// <summary>
        /// Default constructor of the Esp32Coprocessor class.
        /// </summary>
        internal Esp32Coprocessor()
        {
            _debugLevel = DebugOptions.None;
        }

        #endregion Constructor(s)

        #region Methods


        protected void SetProperty(ConfigurationItems item, UInt32 value)
        {
            throw new NotImplementedException("SetProperty is not implemented.");
        }

        protected void SetProperty(ConfigurationItems item, byte value)
        {
            UInt32 v = value;
            SetProperty(item, v);
        }

        protected void SetProperty(ConfigurationItems item, bool value)
        {
            UInt32 v = 0;
            if (value)
            {
                v = 1;
            }
            SetProperty(item, v);
        }

        protected void SetProperty(ConfigurationItems item, string value)
        {
        }


        /// <summary>
        /// Send a parameterless command (i.e a command where no payload is required) to the ESP32.
        /// </summary>
        /// <param name="where">Interface the command is destined for.</param>
        /// <param name="function">Command to be sent.</param>
        /// <param name="block">Is this a blocking command?</param>
        /// <param name="encodedResult">4000 byte array to hold any data returned by the command.</param>
        /// <returns>StatusCodes enum indicating if the command was successful or if an error occurred.</returns>
        protected StatusCodes SendCommand(byte where, UInt32 function, bool block, byte[] encodedResult)
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
        protected StatusCodes SendCommand(byte where, UInt32 function, bool block, byte[] payload, byte[] encodedResult)
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
        protected StatusCodes GetConfiguration(bool force = false)
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

        #endregion Methods
    }
}