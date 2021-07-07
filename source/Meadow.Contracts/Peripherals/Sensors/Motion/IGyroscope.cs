using Meadow.Units;
using System;

namespace Meadow.Peripherals.Sensors.Motion
{
    /// <summary>
    /// Represents a generic gyroscopic sensor that measures angular velocity.
    /// </summary>
    public interface IGyroscope : ISensor
    {
        /// <summary>
        /// Last value read from the Temperature sensor.
        /// </summary>
        AngularVelocity3D? AngularVelocity3D { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<IChangeResult<AngularVelocity3D>> AngularVelocity3DUpdated;
    }
}