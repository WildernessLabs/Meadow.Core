using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    /// <summary>
    /// An IPin implementation that allows for automatic use of either gpiod or sysfs, depending on the OS support
    /// </summary>
    public class LinuxFlexiPin : Pin
    {
        /// <summary>
        /// The pin's sysfs GPIO number
        /// </summary>
        public int SysFsGpio { get; internal set; }
        /// <summary>
        /// The chip name for the gpiod driver
        /// </summary>
        public string GpiodChip { get; internal set; }
        /// <summary>
        /// The pin offset for the gpiod driver
        /// </summary>
        public int GpiodOffset { get; }

        /// <summary>
        /// Creates a LinuxFlexiPin instance
        /// </summary>
        /// <param name="controller">The owner of this pin</param>
        /// <param name="name">The friendly pin name</param>
        /// <param name="key">The pin's internal key</param>
        /// <param name="sysfsGpio">The pin's sysfs GPIO number</param>
        /// <param name="gpiodChip">The chip name for the gpiod driver</param>
        /// <param name="gpiodOffset">The pin offset for the gpiod driver</param>
        /// <param name="supportedChannels">A list of supported channels</param>
        public LinuxFlexiPin(IPinController controller, string name, object key, int sysfsGpio, string gpiodChip, int gpiodOffset, IList<IChannelInfo>? supportedChannels = null)
            : base(controller, name, key, supportedChannels)
        {
            SysFsGpio = sysfsGpio;
            GpiodChip = gpiodChip;
            GpiodOffset = gpiodOffset;
        }
    }
}
