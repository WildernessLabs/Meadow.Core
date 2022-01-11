using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Describes the type of flow control. See 
    /// https://en.wikipedia.org/wiki/Flow_control_(data)
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
