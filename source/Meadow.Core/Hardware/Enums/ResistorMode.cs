namespace Meadow.Hardware
{
    /// <summary>
    /// Describes the internal IO resistor state.
    /// </summary>
    public enum ResistorMode
    {
        // TODO: what about NONE?

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