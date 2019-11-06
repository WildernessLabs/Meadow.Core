namespace Meadow.Peripherals.Sensors.Proximity
{
    /// <summary>
    /// Defines a generic proximity sensor
    /// </summary>
    public interface IRangeFinder : ISensor
    {
        /// <summary>
        /// Returns distance current value
        /// </summary>
        float DistanceOutput { get; }
    }
}
