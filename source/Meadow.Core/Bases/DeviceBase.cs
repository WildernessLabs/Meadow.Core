using System;
using System.Collections.Generic;
using Meadow.Hardware;

namespace Meadow
{
    /// <summary>
    /// TODO: make public at some point.
    /// </summary>
    internal abstract class DeviceBase : IDevice
    {
        public DeviceCapabilities Capabilities { get; protected set; }
        public IGPIOManager GPIOManager { get; protected set; }


        protected DeviceBase()
        {

        }
    }
}
