using Meadow.Hardware;

namespace Meadow;

/// <summary>
/// Represents digital channel information specific to SysFs-based GPIO operations.
/// </summary>
public class SysFsDigitalChannelInfo : DigitalChannelInfoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SysFsDigitalChannelInfo"/> class
    /// with the specified channel name.
    /// </summary>
    /// <param name="name">The name of the digital channel.</param>
    public SysFsDigitalChannelInfo(string name)
        : base(name, true, true, true, false, false, false, null)
    {
    }
}
