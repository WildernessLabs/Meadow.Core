using System;
using System.Collections.Generic;
using Meadow.Hardware;

namespace Meadow
{
    /// <summary>
    /// TODO: internal?
    /// </summary>
    public abstract class DeviceBase : IDevice
    {
        public DeviceCapabilities Capabilities { get; protected set; }


        protected DeviceBase()
        {

        }
    }
}
