using System;
using System.Runtime.InteropServices;
using Meadow.Core;
using System.Text;
using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Hardware;
using static Meadow.IPlatformOS;

namespace Meadow.Devices
{
    public abstract partial class F7MicroBase
    {
        // move up into Device?
        public static class DeviceInformation
        {
            /// <summary>
            /// The current device name.
            /// </summary>
            /// <returns>Name of the device.</returns>
            public static string DeviceName {
                get => F7PlatformOS.GetString(ConfigurationValues.DeviceName);
                set => F7PlatformOS.SetString(ConfigurationValues.DeviceName, value);
            }

            /// <summary>
            /// Get the current model name.
            /// </summary>
            /// <returns>Model name.</returns>
            public static string Model => F7PlatformOS.GetString(ConfigurationValues.Model);
            //public HardwareModel Model { get; }

            public static uint GetProductVersion => F7PlatformOS.GetUInt(ConfigurationValues.Product);

            /// <summary>
            /// Get the processor type.
            /// </summary>
            /// <returns>Processor type.</returns>
            public static string ProcessorType => F7PlatformOS.GetString(ConfigurationValues.ProcessorType);

            /// <summary>
            /// Get the serial number of the device.
            /// </summary>
            /// <returns>Serial number of the device.</returns>
            public static string ProcessorSerialNumber => F7PlatformOS.GetString(ConfigurationValues.SerialNumber);

            /// <summary>
            /// Get the unique ID of the micrcontroller.
            /// </summary>
            /// <returns>Unique ID of the microcontroller.</returns>
            public static string ChipID => F7PlatformOS.GetString(ConfigurationValues.UniqueId);

            /// <summary>
            /// Get the coprocessor type.
            /// </summary>
            /// <returns>Coprocessor type.</returns>
            public static string CoprocessorType => F7PlatformOS.GetString(ConfigurationValues.CoprocessorType);

            /// <summary>
            /// Get the version of the firmware flashed to the coprocessor.
            /// </summary>
            /// <returns>Coprocessor firmware version..</returns>
            public static string CoprocessorOSVersion => F7PlatformOS.GetString(ConfigurationValues.CoprocessorFirmwareVersion);

            //TODO: what about coprocessor build date
        }
    }
}
