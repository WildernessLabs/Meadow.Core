using System;

namespace Meadow.Peripherals.Sensors.Rotary
{
    /// <summary>
    /// Defines the event args for the RotaryTurned event
    /// </summary>
    public struct RotaryChangeResult : IChangeResult<RotationDirection>
    {
        /// <summary>
        /// Get or Sets the rotary's direction
        /// </summary>
        [Obsolete("Please use the `New` property.")]
        public RotationDirection Direction {
            get { return New; }
        }

        public RotationDirection New { get; set; }
        public RotationDirection? Old { get; set; }

        /// <summary>
        /// Creates a new `RotaryChangeResult` with the new direction of rotation
        /// and, optionally, the previous.
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        public RotaryChangeResult(RotationDirection newValue, RotationDirection? oldValue)
        {
            New = newValue;
            Old = oldValue;
        }
    }
}
