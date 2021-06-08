namespace Meadow.Hardware
{
    /// <summary>
    /// Standard I2C Bus speeds
    /// </summary>
    public enum I2cBusSpeed
    {
        /// <summary>
        /// Standard 100 kHz clock frequency
        /// </summary>
        Standard = 100000,
        /// <summary>
        /// Fast 400 kHz clock frequency
        /// </summary>
        Fast = 400000,
        /// <summary>
        /// Fast-Plus 1 MHz clock frequency
        /// </summary>
        FastPlus = 1000000
    }
}
