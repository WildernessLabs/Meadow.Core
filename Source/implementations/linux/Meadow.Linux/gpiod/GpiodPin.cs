using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    /// <summary>
    /// An IPin implementation using the gpiod driver
    /// </summary>
    public class GpiodPin : Pin
    {
        public string Chip { get; }
        public int Offset { get; }

        /// <summary>
        /// Creates an instance of a GpiodPin
        /// </summary>
        /// <param name="controller">The owner of this pin</param>
        /// <param name="name">The friendly pin name</param>
        /// <param name="key">The pin's internal key</param>
        /// <param name="chip">The chip name for the gpiod driver</param>
        /// <param name="offset">The pin offset for the gpiod driver</param>
        /// <param name="supportedChannels">A list of supported channels</param>
        public GpiodPin(IPinController controller, string name, object key, string chip, int offset, IList<IChannelInfo>? supportedChannels = null)
            : base(controller, name, key, supportedChannels)
        {
            Chip = chip;
            Offset = offset;
        }
    }
}
