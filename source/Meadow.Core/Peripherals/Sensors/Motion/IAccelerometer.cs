namespace Meadow.Peripherals.Sensors.Motion
{
    /// <summary>
    /// Represents a generic accelerometer sensor.
    /// </summary>
    public interface IAccelerometer : ISensor
    {
        /// <summary>
        /// Returns acceleration value in the X axis
        /// </summary>
        float XAcceleration { get; }

        /// <summary>
        /// Returns acceleration value in the Y axis
        /// </summary>
        float YAcceleration { get; }

        /// <summary>
        /// Returns acceleration value in the Z axis
        /// </summary>
        float ZAcceleration { get; }
    }
}
