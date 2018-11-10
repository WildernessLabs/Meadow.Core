using System;
using Meadow.Hardware;

namespace Meadow
{
    public abstract class DeviceBase : IDevice
    {
        public DeviceCapabilities Capabilities { get; protected set; }
    }
}
