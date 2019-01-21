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
        InterruptNone = 0,
        /// <summary>
        /// The interrupt is triggered on the falling edge when changing from high
        /// to low.
        /// </summary>
        InterruptEdgeLow = 1,
        /// <summary>
        /// The interrupt is triggered on the rising edge when changing from 
        /// low to high.
        /// </summary>
        InterruptEdgeHigh = 2,
        /// <summary>
        /// The interrupt is triggered on any state change.
        /// </summary>
        InterruptEdgeBoth = 3,
        /// <summary>
        /// Interrupt is triggered when the signal goes high.
        /// </summary>
        InterruptEdgeLevelHigh = 4,
        /// <summary>
        /// Interrupt is triggered when the signal goes low.
        /// </summary>
        InterruptEdgeLevelLow = 5
    }
}
