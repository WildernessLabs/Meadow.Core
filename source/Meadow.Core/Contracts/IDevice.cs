namespace Meadow
{
    /// <summary>
    /// Contract for Meadow devices.
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Gets the device capabilities.
        /// </summary>
        DeviceCapabilities Capabilities { get; }

        /// <summary>
        /// Gets the pins.
        /// </summary>
        IPinDefinitions Pins { get; }

        /// <summary>
        /// Gets the GPIO Manager.
        /// </summary>
        IGPIOManager GPIOManager { get; }
    }
}
