namespace Meadow.Hardware
{
    /// <summary>
    /// Describes the output configuration for a GPIO.
    /// </summary>
    public enum OutputType
    {
        /// <summary>
        /// GPIO Output is configured as Push Pull.
        /// </summary>
        PushPull = 0,
        /// <summary>
        /// GPIO Output is configured as Open Drain.
        /// </summary>
        OpenDrain = 1
    }
}