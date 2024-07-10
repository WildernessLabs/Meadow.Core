using Meadow.Hardware;

namespace Meadow;

/// <summary>
/// Represents digital channel information for a GPIO pin controlled by GPIO character device (gpiod).
/// </summary>
public class GpiodDigitalChannelInfo : DigitalChannelInfoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GpiodDigitalChannelInfo"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the GPIO digital channel.</param>
    public GpiodDigitalChannelInfo(string name)
        : base(name, true, true, true, true, true, false, null)
    {
    }
}
