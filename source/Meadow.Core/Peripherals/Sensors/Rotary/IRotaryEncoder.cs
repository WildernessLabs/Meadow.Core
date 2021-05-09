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
        /// Raised when the rotary encoder is rotated and returns a RotaryTurnedEventArgs object which describes the direction of rotation.
        /// </summary>
        IDigitalInputPort APhasePort { get; }

        /// <summary>
        /// Returns the pin connected to the B-phase output on the rotary encoder.
        /// </summary>
        IDigitalInputPort BPhasePort { get; }

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
