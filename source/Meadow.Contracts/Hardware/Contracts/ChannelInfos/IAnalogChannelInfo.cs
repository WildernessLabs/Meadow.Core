using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for analog communication channels.
    /// </summary>
    public interface IAnalogChannelInfo : IChannelInfo
    {
        /// <summary>
        /// Whether or not the channel is capable of reading input (i.e. ADC).
        /// </summary>
        /// <value><c>true</c> if input capable; otherwise, <c>false</c>.</value>
        bool InputCapable { get; }
        /// <summary>
        /// Whether or not the channel is capable of writing output (i.e. DAC).
        /// </summary>
        /// <value><c>true</c> if output capable; otherwise, <c>false</c>.</value>
        bool OutputCapable { get; }
        /// <summary>
        /// Precision (in bits) of the channel
        /// </summary>
        byte Precision { get; }
    }
}
