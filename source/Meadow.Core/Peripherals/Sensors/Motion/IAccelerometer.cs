using Meadow.Units;
using System;

namespace Meadow.Peripherals.Sensors.Motion
{
    /// <summary>
    /// Represents a generic accelerometer sensor.
    /// </summary>
    public interface IAccelerometer :  ISensor
    {
        /// <summary>
        /// Last value read from the Temperature sensor.
        /// </summary>
        Acceleration3D Acceleration3D { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<IChangeResult<Acceleration3D>> Acceleration3DUpdated;
    }
}