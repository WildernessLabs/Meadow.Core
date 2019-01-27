namespace Meadow.Peripherals.Sensors.Distance
{
    /// <summary>
    /// Interface for distance sensors classes.
    /// </summary>
    public interface IRangeFinder
    {
        /// <summary>
        /// Returns current distance detected.
        /// </summary>
        float CurrentDistance { get; }

        /// <summary>
        /// Minimum valid distance (CurrentDistance returns -1 if below).
        /// </summary>
        float MinimumDistance { get; }

        /// <summary>
        /// Maximum valid distance (CurrentDistance returns -1 if above).
        /// </summary>
        float MaximumDistance { get; }

        /// <summary>
        /// Raised when detecting an obstacle.
        /// </summary>
        event DistanceDetectedEventHandler DistanceDetected;
    }
}
