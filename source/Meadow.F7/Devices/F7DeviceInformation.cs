using System;
using Meadow.Hardware;
using static Meadow.IPlatformOS;

namespace Meadow.Devices
{
    public class F7DeviceInformation : IDeviceInformation
    {
        /// <summary>
        /// The current device name.
        /// </summary>
        /// <returns>Name of the device.</returns>
        public string DeviceName
        {
            get => F7PlatformOS.GetString(ConfigurationValues.DeviceName);
            set => F7PlatformOS.SetString(ConfigurationValues.DeviceName, value);
        }

        /// <summary>
        /// Get the current model name.
        /// </summary>
        /// <returns>Model name.</returns>
        public string Model => F7PlatformOS.GetString(ConfigurationValues.Model);
        //public HardwareModel Model { get; }

        // TODO: this should be a string, e.g. `2.c`, not a uint.
        public string HardwareRevision => F7PlatformOS.GetUInt(ConfigurationValues.Product).ToString();

        /// <summary>
        /// Get the processor type.
        /// </summary>
        /// <returns>Processor type.</returns>
        public string ProcessorType => F7PlatformOS.GetString(ConfigurationValues.ProcessorType);

        /// <summary>
        /// Get the serial number of the device.
        /// </summary>
        /// <returns>Serial number of the device.</returns>
        public string ProcessorSerialNumber => F7PlatformOS.GetString(ConfigurationValues.SerialNumber);

        /// <summary>
        /// Get the unique ID of the micrcontroller.
        /// </summary>
        /// <returns>Unique ID of the microcontroller.</returns>
        public string ChipID => F7PlatformOS.GetString(ConfigurationValues.UniqueId);

        /// <summary>
        /// Get the coprocessor type.
        /// </summary>
        /// <returns>Coprocessor type.</returns>
        public string CoprocessorType => F7PlatformOS.GetString(ConfigurationValues.CoprocessorType);

        /// <summary>
        /// Get the version of the firmware flashed to the coprocessor.
        /// </summary>
        /// <returns>Coprocessor firmware version..</returns>
        public string CoprocessorOSVersion => F7PlatformOS.GetString(ConfigurationValues.CoprocessorFirmwareVersion);


        //TODO: what about coprocessor build date
    }
}
