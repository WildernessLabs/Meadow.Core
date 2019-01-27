namespace Meadow.Peripherals.Sensors.Rotary
{
    /// <summary>
    /// Defines the event args for the RotaryTurned event
    /// </summary>
    public class RotaryTurnedEventArgs
    {
        /// <summary>
        /// Get or Sets the rotary's direction
        /// </summary>
        public RotationDirection Direction { get; set; }

        /// <summary>
        /// Constructor for SensorVectorEventArgs objects.
        /// </summary>
        /// <param name="lastValue">Last sensor value sent through the eventing mechanism.</param>
        /// <param name="currentValue">Current sensor reading.</param>
        public RotaryTurnedEventArgs(RotationDirection direction)
        {
            Direction = direction;
        }
    }

    /// <summary>
    /// Delegate for the events that will return a RotaryTurnedEventHandler object.
    /// </summary>
    /// <param name="sender">Object sending the notification.</param>
    /// <param name="e">RotaryTurnedEventArgs object containing the data for the application.</param>
    public delegate void RotaryTurnedEventHandler(object sender, RotaryTurnedEventArgs e);
}
