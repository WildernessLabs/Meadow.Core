using Meadow.Hardware;
using static Meadow.IPlatformOS;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents the device information for the Meadow F7
    /// </summary>
    public class F7DeviceInformation : IDeviceInformation
    {
        private MeadowPlatform? _platform;
        private string? _processor;
        private string? _model;
        private string? _processorSN;
        private string? _uniqueID;
        private string? _coprocesssorType;
        private string? _coprocessorVersion;
        private string? _osVersion;

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
        public string Model => _model ??= F7PlatformOS.GetString(ConfigurationValues.Model);

        /// <summary>
        /// Get the hardware revision.
        /// </summary>
        public MeadowPlatform Platform => _platform ??= (MeadowPlatform)F7PlatformOS.GetUInt(ConfigurationValues.Product);

        /// <summary>
        /// Get the processor type.
        /// </summary>
        /// <returns>Processor type.</returns>
        public string ProcessorType => _processor ??= F7PlatformOS.GetString(ConfigurationValues.ProcessorType);

        /// <summary>
        /// Get the serial number of the device.
        /// </summary>
        /// <returns>Serial number of the device.</returns>
        public string ProcessorSerialNumber => _processorSN ??= F7PlatformOS.GetString(ConfigurationValues.SerialNumber);

        /// <summary>
        /// Gets the unique ID of the F7 microcontroller.
        /// </summary>
        public string UniqueID => _uniqueID ??= F7PlatformOS.GetString(ConfigurationValues.UniqueId);

        /// <summary>
        /// Get the coprocessor type.
        /// </summary>
        /// <returns>Coprocessor type.</returns>
        public string CoprocessorType => _coprocesssorType ??= F7PlatformOS.GetString(ConfigurationValues.CoprocessorType);

        /// <summary>
        /// Get the version of the firmware flashed to the coprocessor.
        /// </summary>
        /// <returns>Coprocessor firmware version..</returns>
        public string CoprocessorOSVersion => _coprocessorVersion ??= F7PlatformOS.GetString(ConfigurationValues.CoprocessorFirmwareVersion);

        /// <summary>
        /// Get the version of the firmware flashed to the microcontroller.
        /// </summary>
        public string OSVersion => _osVersion ??= F7PlatformOS.GetString(ConfigurationValues.OsVersion);

        //TODO: what about coprocessor build date
    }
}
