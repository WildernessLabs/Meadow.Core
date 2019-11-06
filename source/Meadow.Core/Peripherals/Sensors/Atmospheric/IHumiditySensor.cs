using System;

namespace Meadow.Peripherals.Sensors.Atmospheric
{
    /// <summary>
    /// Humidity sensor interface requirements.
    /// </summary>
    public interface IHumiditySensor : ISensor
    {
        /// <summary>
        /// Last value read from the humidity sensor.
        /// </summary>
        float Humidity { get; }

        /// <summary>
        /// The Humidity changed event will be raised when the difference (absolute value)
        /// between the current humidity reading and the last notified reading is greater
        /// than the HumidityChangeNotificationThreshold.
        /// </summary>
        event EventHandler<FloatChangeResult> HumidityChanged;
    }
}
