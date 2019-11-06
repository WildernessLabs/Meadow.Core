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
        /// The PressureChanged event will be raised when the difference (absolute value)
        /// between the current Pressure reading and the last notified reading is greater
        /// than the PressureChangeNotificationThreshold.
        /// </summary>
        event EventHandler<FloatChangeResult> PressureChanged;
    }
}
