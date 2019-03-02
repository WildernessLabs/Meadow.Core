using System;
using System.Collections.Generic;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a physical pin on the Meadow device.
    /// </summary>
    public class Pin : PinBase
    {
        public Pin(string name, object key, IList<IChannel> supportedChannels = null) 
            : base (name, key, supportedChannels)
        {
        }
    }
}
