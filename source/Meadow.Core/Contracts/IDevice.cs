using System;

namespace Meadow
{
    /// <summary>
    /// Contract for Meadow devices.
    /// </summary>
    public interface IDevice
    {
        DeviceCapabilities Capabilities { get; }

        IPinDefinitions Pins { get; }

        IGPIOManager GPIOManager { get; }
    }
}
