using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow;

public partial class RaspberryPi
{
    public class LedPin : Pin
    {
        internal LedPin(IPinController? controller, string name, object key, IList<IChannelInfo>? supportedChannels)
            : base(controller, name, key, supportedChannels)
        {
        }
    }
}
