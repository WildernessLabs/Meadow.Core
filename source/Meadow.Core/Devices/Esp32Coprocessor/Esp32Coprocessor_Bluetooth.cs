using System;
using System.Runtime.InteropServices;
using Meadow.Devices.Esp32.MessagePayloads;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    public partial class Esp32Coprocessor
    {
        internal string GetDefaultName()
        {
            // TODO: query this
            return "Meadow BLE";
        }

        public bool StartBluetoothStack(string deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
            {
                throw new ArgumentException("Invalid deviceName");
            }

            // TODO: filter bad characters or too-long names

            var payloadGcHandle = default(GCHandle);

            try
            {
                var req = new BTServicesDescription
                {
                    DeviceName = deviceName,
                    PrimaryService = "MeadowSvc" // TODO
                };

                var requestBytes = Encoders.EncodeBTServicesDescription(req);

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
    }
}