namespace Meadow.Hardware;

/// <summary>
/// Represents a configuration for a hardware channel.
/// </summary>
public class ChannelConfig
{
    /// <summary>
    /// Gets or sets the state of the channel.
    /// </summary>
    public ChannelState State { get; set; }

    /// <summary>
    /// Gets or sets the configuration type of the channel.
    /// </summary>
    public ChannelConfigurationType Config { get; set; }
}
