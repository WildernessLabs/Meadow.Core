using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    public class SysFsPin : Pin
    {
        public int Gpio { get; }

        public SysFsPin(IPinController controller, string name, object key, int gpio, IList<IChannelInfo>? supportedChannels = null)
            : base(controller, name, key, supportedChannels)
        {
            Gpio = gpio;
        }
    }
}
