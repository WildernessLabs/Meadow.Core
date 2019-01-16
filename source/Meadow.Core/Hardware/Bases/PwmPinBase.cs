using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for pins that have a Pulse-Width-Modulation
    /// (PWM) channel.
    /// </summary>
    public abstract class PwmPinBase : DigitalPinBase, IPwmPin
    {
        /// <summary>
        /// Describes the minimum frequency the PWM channel can generate.
        /// </summary>
        /// <value>The minimum frequency.</value>
        public double MinimumFrequency { get; protected set; }
        /// <summary>
        /// Describes the maximum frequency the PWM channel can generate.
        /// </summary>
        /// <value>The maximum frequency.</value>
        public double MaximumFrequency { get; protected set; }

        protected PwmPinBase(string name, object key, 
                             double minimumFrequency, double maximumFrequency) 
            : base(name, key)
        {
            this.MinimumFrequency = minimumFrequency;
            this.MaximumFrequency = maximumFrequency;
        }
    }
}
