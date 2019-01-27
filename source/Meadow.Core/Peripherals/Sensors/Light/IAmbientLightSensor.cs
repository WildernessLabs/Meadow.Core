namespace Meadow.Peripherals.Sensors.Light
{
    /// <summary>
    /// Generic ambient light sensor class
    /// </summary>
    public interface IAmbientLightSensor
    {
        /// <summary>
        /// Returns raw data from the sensor
        /// </summary>
        float Reading { get; }
    }
}
