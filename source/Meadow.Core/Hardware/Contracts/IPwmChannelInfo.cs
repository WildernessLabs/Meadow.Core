using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Describes the capabilities of a Pulse-Width-Modulation channel
    /// </summary>
    public interface IPwmChannelInfo : IDigitalChannelInfo
    {
        /// <summary>
        /// Gets the minimum frequency that the underlying PWM generator is 
        /// capable of.
        /// </summary>
        /// <value>The minimum frequency.</value>
        float MinimumFrequency { get; }

        /// <summary>
        /// Gets the maximum frequency that the underlying PWM generator is 
        /// capabel of
        /// </summary>
        /// <value>The maximum frequency.</value>
        float MaximumFrequency { get; }
    }
}
