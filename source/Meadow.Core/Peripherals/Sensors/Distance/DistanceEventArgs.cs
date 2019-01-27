namespace Meadow.Peripherals.Sensors.Distance
{
    /// <summary>
    /// Distance event arguments for distance sensors
    /// </summary>
    public class DistanceEventArgs
    {
        /// <summary>
        /// Gets or sets distance
        /// </summary>
        public float Distance { get; set; }

        /// <summary>
        /// Creates a distance event argument
        /// </summary>
        /// <param name="distance"></param>
        public DistanceEventArgs(float distance)
        {
            Distance = distance;
        }
    }

    /// <summary>
    /// Raised when an received a rebound trigger signal
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DistanceDetectedEventHandler(object sender, DistanceEventArgs e);
}
