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
        /// <summary>
        /// Whether or not the channel is capable of writing outputs.
        /// </summary>
        /// <value><c>true</c> if output capable; otherwise, <c>false</c>.</value>
        bool OutputCapable { get; }
        /// <summary>
        /// Whether or not the channel is capable of receiving interrupts.
        /// </summary>
        /// <value><c>true</c> if interrupt capable; otherwise, <c>false</c>.</value>
        bool InterruptCapable { get; }
        /// <summary>
        /// Whether or not the channel is capable of internal pull-down resistors.
        /// </summary>
        /// <value><c>true</c> if pull-down capable; otherwise, <c>false</c>.</value>
        bool PullDownCapable { get; }
        /// <summary>
        /// Whether or not the channel is capable of internal pull-up resistors.
        /// </summary>
        /// <value><c>true</c> if pull-up capable; otherwise, <c>false</c>.</value>
        bool PullUpCapable { get; }
        /// <summary>
        /// Whether or not the channel uses high-voltage as logic high or low.
        /// </summary>
        /// <value><c>true</c> if voltage high equals logic low; otherwise, <c>false</c>.</value>
        bool InverseLogic { get; }
        /// <summary>
        /// If the channel is interrupt capable and is grouped (i.e. only one channel per group is allowed) this will be non-zero.
        /// </summary>
        int InterruptGroup { get; }
    }
}
