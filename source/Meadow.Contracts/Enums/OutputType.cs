namespace Meadow.Hardware
{
    /// <summary>
    /// Describes the output configuration for a GPIO.
    /// </summary>
    public enum OutputType
    {
        /// <summary>
        /// GPIO Output is configured as Push Pull. When in _Push-Pull_ mode the
        /// MCU can actively drive the port either `HIGH` (`3.3V`) or `LOW`
        /// (`0V`), by utilizing two switches internally, requiring no
        /// external components.
        /// </summary>
        PushPull = 0,
        /// <summary>
        /// GPIO Output is configured as Open Drain. When in _Open-Drain_ mode,
        /// the MCU port utilizes only one switch internally, and can only
        /// actively drive the port `LOW`. So an external pull-up resistor
        /// connected to the `3V3` rail is required to be able to set a logical
        /// `HIGH` level. `OpenDrain` is provided largely as a legacy feature
        /// and is hardly used anymore.
        /// </summary>
        OpenDrain = 1
    }
}