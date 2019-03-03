namespace Meadow
{
    /// <summary>
    /// Contract for Meadow devices.
    /// </summary>
    public interface IDevice<P> where P : IPinDefinitions
    {
        /// <summary>
        /// Gets the device capabilities.
        /// </summary>
        DeviceCapabilities Capabilities { get; }

        P Pins { get; }

        /// <summary>
        /// Gets the GPIO Manager.
        /// </summary>
        //IGPIOManager GPIOManager { get; }
    }
}
