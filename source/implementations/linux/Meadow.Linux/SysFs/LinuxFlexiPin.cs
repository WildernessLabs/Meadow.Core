using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    public class LinuxFlexiPin : Pin
    {
        public int SysFsGpio { get; }
        public string GpiodChip { get; }
        public int GpiodOffset { get; }

        public LinuxFlexiPin(IPinController controller, string name, object key, int sysfsGpio, string gpiodChip, int gpiodOffset, IList<IChannelInfo>? supportedChannels = null)
            : base(controller, name, key, supportedChannels)
        {
            SysFsGpio = sysfsGpio;
            GpiodChip = gpiodChip;
            GpiodOffset = gpiodOffset;
        }
    }
}
