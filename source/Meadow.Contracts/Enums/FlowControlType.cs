using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Describes the type of flow control. See 
    /// <a href='https://en.wikipedia.org/wiki/Flow_control_(data)">Flow Control</a>.
    /// </summary>
    public enum FlowControlType
    {
        /// <summary>
        /// No flow control.
        /// </summary>
        None = 0,
        /// <summary>
        /// Hardware flow control.
        /// </summary>
        RequestToSend = 6,
        /// <summary>
        /// Software flow control.
        /// </summary>
        XOnXOff = 24
    }
}
