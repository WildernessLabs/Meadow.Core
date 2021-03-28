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

        public ushort[] GetGraphHandles()
        {
            // TODO: maybe this needs to be a dictionary where we set ID's in the graph config and we get an ID->handle dictionary back?

            var resultBuffer = new byte[MAXIMUM_SPI_BUFFER_LENGTH];

            Output.WriteLineIf(_debugLevel.HasFlag(DebugOptions.EventHandling), $"Requesting graph handles...");

            var result = SendCommand((byte)Esp32Interfaces.BlueTooth, (int)BluetoothFunction.GetHandles, true, resultBuffer);

            var count = BitConverter.ToInt16(resultBuffer, 0);

            Output.WriteLineIf(_debugLevel.HasFlag(DebugOptions.EventHandling), $"Received {count} graph handles");
            Output.BufferIf(_debugLevel.HasFlag(DebugOptions.EventHandling), resultBuffer, 2, count * 2);

            var handles = new ushort[count];
            var index = 6; // skip over the count *and* the uint length

            for(int i = 0; i < count; i++)
            {
                handles[i] = BitConverter.ToUInt16(resultBuffer, index);
                index += 2;
            }


            return handles;
        }

        /// <summary>
        /// Use the event data to work out which event to invoke and create any event args that will be consumed.
        /// </summary>
        /// <param name="eventId">Event ID.</param>
        /// <param name="statusCode">Status of the event.</param>
        /// <param name="payload">Optional payload containing data specific to the result of the event.</param>
        protected void InvokeEvent(BluetoothFunction eventId, StatusCodes statusCode, byte[] payload)
        {
            Output.WriteLineIf(_debugLevel.HasFlag(DebugOptions.EventHandling), $"event {eventId} of status {statusCode} with {payload.Length} bytes : {BitConverter.ToString(payload)}");

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