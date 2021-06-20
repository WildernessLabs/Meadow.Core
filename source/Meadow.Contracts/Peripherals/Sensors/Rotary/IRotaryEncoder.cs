using System;
using Meadow.Hardware;

namespace Meadow.Peripherals.Sensors.Rotary
{
    /// <summary>
    /// Defines a generic rotaty encoder
    /// </summary>
    public interface IRotaryEncoder
    {
        /// <summary>
        /// Gets the last direction of rotation
        /// </summary>
        RotationDirection? LastDirectionOfRotation { get; }

        /// <summary>
        /// Raised when the encoder detects a rotation
        /// </summary>
        event EventHandler<RotaryChangeResult> Rotated;
    }
}
