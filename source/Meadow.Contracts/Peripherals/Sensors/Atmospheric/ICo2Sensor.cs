using Meadow.Units;
using System;

namespace Meadow.Peripherals.Sensors
{
    /// <summary>
    /// CO2 sensor interface requirements.
    /// </summary>
    public interface ICo2Sensor : ISensor
    {
        /// <summary>
        /// Last value read from the CO2 sensor.
        /// </summary>
        Concentration? Co2 { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<ChangeResult<Concentration>> Co2Updated;
    }
}