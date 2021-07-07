using Meadow.Units;
using System;

namespace Meadow.Peripherals.Sensors
{
    /// <summary>
    /// Pressure sensor interface requirements.
    /// </summary>
    public interface IBarometricPressureSensor : ISensor
    {
        /// <summary>
        /// Last value read from the Pressure sensor.
        /// </summary>
        Pressure? Pressure { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<IChangeResult<Pressure>> PressureUpdated;
    }
}