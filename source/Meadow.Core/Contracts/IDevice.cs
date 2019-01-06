using System;

namespace Meadow
{

    public interface IDevice
    {
        DeviceCapabilities Capabilities { get; }

        IGPIOManager GPIOManager { get; }
    }
}
