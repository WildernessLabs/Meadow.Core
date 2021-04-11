using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Gateways;
using Meadow.Gateways.Bluetooth;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    public partial class Esp32Coprocessor : IBluetoothAdapter
    {
        private Definition _definition;

        internal string GetDefaultName()
        {
            // TODO: query this
            return "Meadow BLE";
        }

        public bool StartBluetoothStack(Definition definition)
        {
            if (definition == null)
            {
                throw new ArgumentException("Invalid definition");
            }
            if(_definition != null)
            {
                throw new Exception("Stack already initialized");
            }

            _definition = definition;

            // wire up the server write events
            foreach(var s in _definition.Services)
            {
                foreach(var c in s.Characteristics)
                {
                    c.ServerValueSet += OnServerValueSet;
                }
            }

            var configuration = definition.ToJson();

            Console.WriteLine("======================");
            Console.WriteLine(configuration);
            Console.WriteLine("======================");
            // TODO: sanity checking of the config

            var payloadGcHandle = default(GCHandle);

            try
            {
                var req = new BTStackConfig
                {
                    Config = configuration
                };

                var requestBytes = Encoders.EncodeBTStackConfig(req);

                var result = SendBluetoothCommand(BluetoothFunction.Start, true, requestBytes, null);
                return result == StatusCodes.CompletedOk;
            }
            finally
            {
                Task.Run(async () =>
                {
                    // yes. this is ugly.
                    // We wait for the BT stack to get created and then go get the handles
                    await Task.Delay(100);
                    UpdateDefinitionHandles();
                });

                if (payloadGcHandle.IsAllocated)
                {
                    payloadGcHandle.Free();
                }
            }
        }

        private void OnServerValueSet(ICharacteristic c, byte[] valueBytes)
        {
            if(c.ValueHandle == 0)
            {
                // we don't yet have a handle for this, can't set a value
                Output.WriteLineIf(_debugLevel.HasFlag(DebugOptions.EventHandling), $"Cannot write characteristic value without a handle.");
                return;
            }

            var msg = new BTServerDataSet
            {
                Handle = c.ValueHandle,
                SetData = valueBytes,
                SetDataLength = (uint)valueBytes.Length
            };

            var bytes = Encoders.EncodeBTServerDataSet(msg);

            Output.WriteLineIf(_debugLevel.HasFlag(DebugOptions.EventHandling), $"Writing {valueBytes.Length} bytes of data to 0x{c.ValueHandle:x4}...");

            try
            {
                var test = new byte[4];
                var result = SendBluetoothCommand(BluetoothFunction.ServerDataSet, false, bytes, test);

                Output.WriteLineIf(_debugLevel.HasFlag(DebugOptions.EventHandling), $"Result: {result}");
            }
            catch(Exception ex)
            {
                Output.WriteLine($"Result: {ex.Message}");
            }
        }

        private void UpdateDefinitionHandles()
        {
            var handles = GetGraphHandles();

            var index = 0;

            foreach(var service in _definition.Services)
            {
                service.Handle = handles[index++];
                foreach(var characteristic in service.Characteristics)
                {
                    characteristic.DefinitionHandle = handles[index++];
                    characteristic.ValueHandle = handles[index++];

                    _handleToCharacteristicMap.Add(characteristic.ValueHandle, characteristic);

                    foreach (var descriptor in characteristic.Descriptors)
                    {
                        descriptor.Handle = handles[index++];
                    }
                }
            }
            // TODO: assign handles
            // TODO: wire events
        }

        private Dictionary<ushort, ICharacteristic> _handleToCharacteristicMap = new Dictionary<ushort, ICharacteristic>();

        private ushort[] GetGraphHandles()
        {
            // TODO: maybe this needs to be a dictionary where we set ID's in the graph config and we get an ID->handle dictionary back?

            var resultBuffer = new byte[MAXIMUM_SPI_BUFFER_LENGTH];

            Output.WriteLineIf(_debugLevel.HasFlag(DebugOptions.EventHandling), $"Requesting graph handles...");

            var result = SendBluetoothCommand(BluetoothFunction.GetHandles, true, null, resultBuffer);

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
                case BluetoothFunction.ClientWriteRequestEvent:
                    var handle = BitConverter.ToUInt16(payload, 0);
                    var value = new byte[BitConverter.ToInt32(payload, 2)];
                    Output.WriteLineIf(_debugLevel.HasFlag(DebugOptions.EventHandling), $"BLE Data says it is {value.Length} bytes");
                    Array.Copy(payload, 6, value, 0, value.Length);
                    Output.WriteLineIf(_debugLevel.HasFlag(DebugOptions.EventHandling), $"BLE Data Write to Handle 0x{handle:x4}: {BitConverter.ToString(value)}");

                    if(_handleToCharacteristicMap.ContainsKey(handle))
                    {
                        _handleToCharacteristicMap[handle].HandleDataWrite(value);
                    }
                    break;
            }
        }
    }
}