using System;
namespace Meadow.Hardware
{
    public interface IDeviceInformation
    {
        /// <summary>
        /// The current device name.
        /// </summary>
        /// <returns>Name of the device.</returns>
        string DeviceName { get; set; }

        /// <summary>
        /// Get the current model name.
        /// </summary>
        /// <returns>Model name.</returns>
        string Model { get; }

        /// <summary>
        /// The hardware version.
        /// </summary>
        string HardwareRevision { get; }

        /// <summary>
        /// Get the processor type.
        /// </summary>
        /// <returns>Processor type.</returns>
        string ProcessorType { get; }

        /// <summary>
        /// Get the serial number of the device.
        /// </summary>
        /// <returns>Serial number of the device.</returns>
        string ProcessorSerialNumber { get; }

        /// <summary>
        /// Get the unique ID of the micrcontroller.
        /// </summary>
        /// <returns>Unique ID of the microcontroller.</returns>
        string ChipID { get; }

        /// <summary>
        /// Get the coprocessor type.
        /// </summary>
        /// <returns>Coprocessor type.</returns>
        string CoprocessorType { get; }

        /// <summary>
        /// Get the version of the firmware flashed to the coprocessor.
        /// </summary>
        /// <returns>Coprocessor firmware version..</returns>
        string CoprocessorOSVersion { get; }
    }
}