using System;

namespace Meadow.Peripherals.Sensors.Atmospheric
{
    /// <summary>
    /// Pressure sensor interface requirements.
    /// </summary>
    public interface IBarometricPressure : ISensor, IObservable<FloatChangeResult>
    {
        /// <summary>
        /// Last value read from the Pressure sensor.
        /// </summary>
        float Pressure { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<FloatChangeResult> Updated;
    }
}
