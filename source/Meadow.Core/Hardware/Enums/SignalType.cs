namespace Meadow.Hardware
{
    /// <summary>
    /// Describes the type of electrical signal used.
    /// </summary>
    public enum SignalType
    {
        /// <summary>
        /// The electrical signal can be any level within a given range of `LOW`
        /// to `HIGH`
        /// </summary>
        Analog,
        /// <summary>
        /// The electrical signal can only be a `LOW` or `HIGH` value.
        /// </summary>
        Digital
    }
}
