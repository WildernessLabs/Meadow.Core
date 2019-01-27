using Meadow.Peripherals.Sensors;

namespace Meadow.Peripherals.Temperature
{
    /// <summary>
    /// Temperature sensor interface requirements.
    /// </summary>
    public interface ITemperatureSensor : ISensor
    {
        /// <summary>
        /// Last value read from the Temperature sensor.
        /// </summary>
        float Temperature { get; }

        /// <summary>
        /// Threshold value for the changed notification event.
        /// </summary>
        float TemperatureChangeNotificationThreshold { get; set; }

        /// <summary>
        /// The TemperatureChanged event will be raised when the difference (absolute value)
        /// between the current Temperature reading and the last notified reading is greater
        /// than the TemperatureChangeNotificationThreshold.
        /// </summary>
        event SensorFloatEventHandler TemperatureChanged;
    }
}
