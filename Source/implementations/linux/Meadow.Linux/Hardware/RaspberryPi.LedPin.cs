using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow;

public partial class RaspberryPi
{
    /// <summary>
    /// Represents a LED pin on the Raspberry Pi.
    /// </summary>
    public class LedPin : Pin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LedPin"/> class.
        /// </summary>
        /// <param name="controller">The pin controller associated with this pin.</param>
        /// <param name="name">The name of the pin.</param>
        /// <param name="key">The key associated with this pin.</param>
        /// <param name="supportedChannels">The list of supported channels for this pin.</param>
        internal LedPin(IPinController? controller, string name, object key, IList<IChannelInfo>? supportedChannels)
            : base(controller, name, key, supportedChannels)
        {
        }
    }
}
