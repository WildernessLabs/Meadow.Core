using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a GPIO channel that supports digital communications.
    /// </summary>
    public interface IDigitalChannelInfo : IChannelInfo
    {
        /// <summary>
        /// Whether or not the channel is capable of reading input.
        /// </summary>
        /// <value><c>true</c> if input capable; otherwise, <c>false</c>.</value>
        bool InputCapable { get; }
        bool OutputCapable { get; }
        bool InterrruptCapable { get; }
        bool PullDownCapable { get; }
        bool PullUpCapable { get; }
    }
}
