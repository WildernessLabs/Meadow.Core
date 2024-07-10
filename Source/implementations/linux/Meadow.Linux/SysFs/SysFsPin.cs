using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow;

/// <summary>
/// An IPin implementation using the sysfs driver
/// </summary>
public class SysFsPin : Pin
{
    /// <summary>
    /// The pin's sysfs GPIO number
    /// </summary>
    public int Gpio { get; }

    /// <summary>
    /// Creates an instance of a SysFsPin
    /// </summary>
    /// <param name="controller">The owner of this pin</param>
    /// <param name="name">The friendly pin name</param>
    /// <param name="key">The pin's internal key</param>
    /// <param name="gpio">The pin's sysfs GPIO number</param>
    /// <param name="supportedChannels">A list of supported channels</param>
    public SysFsPin(IPinController? controller, string name, object key, int gpio, IList<IChannelInfo>? supportedChannels = null)
        : base(controller, name, key, supportedChannels)
    {
        Gpio = gpio;
    }
}
