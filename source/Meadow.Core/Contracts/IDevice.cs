using System;
using Meadow.Hardware;

namespace Meadow
{
    public interface IDevice
    {
        DeviceCapabilities Capabilities { get; }
    }
}
