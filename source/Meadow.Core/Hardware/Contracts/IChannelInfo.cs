using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Minimum contract to describes a GPIO channel type or protocol on a 
    /// Meadow device such as Analog, Digital, or I2C.
    /// </summary>
    public interface IChannelInfo
    {
        /// <summary>
        /// Gets the name of the channel.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
    }
}
