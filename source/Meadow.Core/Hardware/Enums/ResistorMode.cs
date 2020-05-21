namespace Meadow.Hardware
{
    // Internal note: The values for PullUp and PullDown are backwards from
    // what STM32F7 expects.
    /// <summary>
    /// Describes the internal IO resistor state.
    /// </summary>
    public enum ResistorMode
    {
        /// <summary>
        /// Internal pull-up/pull-down resistor is disabled.
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// Pulled down to `0V` by default.
        /// </summary>
        PullDown = 1,
        /// <summary>
        /// Pulled up to `VCC` by default.
        /// </summary>
        PullUp = 2
    }
}