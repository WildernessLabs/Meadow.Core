using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for ports that implement an analog input channel.
    /// </summary>
    public interface IAnalogInputPort : IAnalogPort
    {
        /// <summary>
        /// Gets the current voltage level of the port represented by a number
        /// from 0 through 1023.
        /// </summary>
        /// <value>The raw value, in.</value>
        float RawValue { get; }

        /// <summary>
        /// Gets the current voltage level of the port in Volts.
        /// </summary>
        /// <value>The current voltage level.</value>
        float Voltage { get; }
    }
}
