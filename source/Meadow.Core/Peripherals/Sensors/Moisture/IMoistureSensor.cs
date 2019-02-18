namespace Meadow.Peripherals.Sensors.Moisture
{
    public interface IMoistureSensor : ISensor
    {
        /// <summary>
        /// Last value read from the moisture sensor.
        /// </summary>
        float Moisture { get; }
    }
}
