using System;
using System.Runtime.InteropServices;
using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Gateways;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    public partial class Esp32Coprocessor : IBluetoothAdapter
    {
        internal string GetDefaultName()
        {
            // TODO: query this
            return "Meadow BLE";
        }

        public bool StartBluetoothStack(string configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration))
            {
                throw new ArgumentException("Invalid deviceName");
            }

            // TODO: sanity checking of the config

            var payloadGcHandle = default(GCHandle);

            try
            {
                var req = new BTStackConfig
                {
                    Config = configuration
                };

                var requestBytes = Encoders.EncodeBTStackConfig(req);

                // TODO: do we expect a result?  If so create a buffer and pin it.

                payloadGcHandle = GCHandle.Alloc(requestBytes, GCHandleType.Pinned);

                var command = new Nuttx.UpdEsp32Command()
                {
                    Interface = (byte)Esp32Interfaces.BlueTooth,
                    Function = (int)BluetoothFunction.Start,
                    StatusCode = (int)StatusCodes.CompletedOk,
                    Payload = payloadGcHandle.AddrOfPinnedObject(),
                    PayloadLength = (UInt32)requestBytes.Length,
                    Result = IntPtr.Zero,
                    ResultLength = 0,
                    Block = 1
                };

                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.Esp32Command, ref command);

                if ((result == 0) && (command.StatusCode == (UInt32)StatusCodes.CompletedOk))
                {
                    return true;
                }
                else
                {
                    if (command.StatusCode == (UInt32)StatusCodes.CoprocessorNotResponding)
                    {
                        throw new Exception("ESP32 coprocessor is not responding.");
                    }

                    // TODO: if we have a response, we'd decode that here

                    return false;
                }
            }
            finally
            {
                if (payloadGcHandle.IsAllocated)
                {
                    payloadGcHandle.Free();
                }
            }
        }

        /// <summary>
        /// Use the event data to work out which event to invoke and create any event args that will be consumed.
        /// </summary>
        /// <param name="eventId">Event ID.</param>
        /// <param name="statusCode">Status of the event.</param>
        /// <param name="payload">Optional payload containing data specific to the result of the event.</param>
        protected void InvokeEvent(BluetoothFunction eventId, StatusCodes statusCode, byte[] payload)
        {
            Output.WriteIf(_debugLevel.HasFlag(DebugOptions.EventHandling), $"event {eventId} of status {statusCode} with {payload.Length} bytes : {BitConverter.ToString(payload)}");

            //  Placeholder
            switch (eventId)
            {
                case BluetoothFunction.WriteRequestEvent:
                    break;
                case BluetoothFunction.ReadRequestEvent:
                    break;
            }
        }
    }
}