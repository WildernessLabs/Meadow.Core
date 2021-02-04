namespace Meadow.Hardware
{
    // Internal note: The values for InternalPullUp and InternalPullDown are backwards from
    // what STM32F7 expects.
    /// <summary>
    /// Describes the internal\external IO resistor states.
    /// </summary>
    public enum ResistorMode
    {
        /// <summary>
        /// Internal pull-up/pull-down resistor is disabled.
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// Internal resistor pulled down to `0V` by default.
        /// </summary>
        InternalPullDown = 1,
        /// <summary>
        /// Internal resistor pulled up to `VCC` by default.
        /// </summary>
        InternalPullUp = 2,
        /// <summary>
        /// External pull down resistor.
        /// </summary>
        ExternalPullDown = 3,
        /// <summary>
        /// External Pull up ressistor.
        /// </summary>
        ExternalPullUp = 4
    }
}