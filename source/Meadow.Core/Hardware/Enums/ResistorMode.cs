namespace Meadow.Hardware
{
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
        PullUp = 1,
        /// <summary>
        /// Pulled down to `0V` by default.
        /// </summary>
        PullDown = 2
    }
}