using Meadow.Units;
using System;

namespace Meadow.Peripherals.Sensors
{
    /// <summary>
    /// Mass sensor interface requirements.
    /// </summary>
    public interface IMassSensor : ISensor
    {
        /// <summary>
        /// Last value read from the sensor.
        /// </summary>
        Mass? Mass { get; }
        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<IChangeResult<Mass>> MassUpdated;
    }
}