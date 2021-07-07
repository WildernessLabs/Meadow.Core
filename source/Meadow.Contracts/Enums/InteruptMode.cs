namespace Meadow.Hardware
{
    /// <summary>
    /// Describes if and when an interrupt is triggered in response to signal
    /// changes.
    /// </summary>
    public enum InterruptMode
    {
        /// <summary>
        /// No interrupt is enabled.
        /// </summary>
        None = 0,
        /// <summary>
        /// The interrupt is triggered on the falling edge when changing from `HIGH`
        /// to `LOW`.
        /// </summary>
        EdgeFalling = 1,
        /// <summary>
        /// The interrupt is triggered on the rising edge when changing from 
        /// `LOW` to `HIGH`.
        /// </summary>
        EdgeRising = 2,
        /// <summary>
        /// The interrupt is triggered on any state change.
        /// </summary>
        EdgeBoth = 3
    }
}
