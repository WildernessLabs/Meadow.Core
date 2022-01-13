using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Information about an analog channel
    /// </summary>
    public class AnalogChannelInfo : ChannelInfoBase, IAnalogChannelInfo
    {
        /// <summary>
        /// Whether or not the channel is capable of reading input (i.e. ADC).
        /// </summary>
        /// <value><c>true</c> if input capable; otherwise, <c>false</c>.</value>
        public bool InputCapable { get; protected set; }
        /// <summary>
        /// Whether or not the channel is capable of writing output (i.e. DAC).
        /// </summary>
        /// <value><c>true</c> if output capable; otherwise, <c>false</c>.</value>
        public bool OutputCapable { get; protected set; }
        /// <summary>
        /// Precision (in bits) of the channel
        /// </summary>
        public byte Precision { get; protected set; }

        /// <summary>
        /// Create an AnalogChannelInfo instance
        /// </summary>
        /// <param name="name">The channel name</param>
        /// <param name="precision">The precision (in bits) of the channel</param>
        /// <param name="inputCapable">Whether or not the channel is ADC capable</param>
        /// <param name="outputCapable">Whether or not the channel is DAC capable</param>
        public AnalogChannelInfo(string name, byte precision, bool inputCapable, bool outputCapable) 
            : base (name)
        {
            this.Precision = precision;
            this.InputCapable = inputCapable;
            this.OutputCapable = outputCapable;
        }

    }
}
