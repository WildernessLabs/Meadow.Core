using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    public class GpiodPin : Pin
    {
        public int Gpio { get; }

        public GpiodPin(string name, object key, int gpio, IList<IChannelInfo>? supportedChannels = null)
            : base(name, key, supportedChannels)
        {
            Gpio = gpio;
        }
    }
}
