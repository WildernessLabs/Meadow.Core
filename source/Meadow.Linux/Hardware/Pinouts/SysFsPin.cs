using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    public class SysFsPin : Pin
    {
        public int Gpio { get; }

        public SysFsPin(string name, object key, int gpio, IList<IChannelInfo>? supportedChannels = null)
            : base(name, key, supportedChannels)
        {
            Gpio = gpio;
        }
    }
}
