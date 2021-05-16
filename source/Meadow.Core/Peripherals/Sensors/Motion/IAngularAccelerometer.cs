using Meadow.Units;
using System;

namespace Meadow.Peripherals.Sensors.Motion
{
    /// <summary>
    /// Represents a generic accelerometer sensor.
    /// </summary>
    public interface IAngularAccelerometer : ISensor
    {
        /// <summary>
        /// Last value read from the Temperature sensor.
        /// </summary>
        AngularAcceleration3D? AngularAcceleration3D { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<IChangeResult<AngularAcceleration3D>> AngularAcceleration3DUpdated;
    }
}