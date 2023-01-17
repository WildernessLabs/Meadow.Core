using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    public class LinuxFlexiPin : Pin
    {
        public int SysFsGpio { get; }
        public string GpiodChip { get; }
        public int GpiodOffset { get; }

        public LinuxFlexiPin(string name, object key, int sysfsGpio, string gpiodChip, int gpiodOffset, IList<IChannelInfo>? supportedChannels = null)
            : base(name, key, supportedChannels)
        {
            SysFsGpio = sysfsGpio;
            GpiodChip = gpiodChip;
            GpiodOffset = gpiodOffset;
        }
    }

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
