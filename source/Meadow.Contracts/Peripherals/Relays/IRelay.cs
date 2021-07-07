namespace Meadow.Peripherals.Relays
{
    /// <summary>
    /// Electrical switch (usually mechanical) that switches on an isolated circuit
    /// </summary>
    public interface IRelay
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Relay is on.
        /// </summary>
        /// <value><c>true</c> if is on; otherwise, <c>false</c>.</value>
        bool IsOn { get; set; }

        /// <summary>
        /// Returns relay type.
        /// </summary>
        RelayType Type { get; }

        /// <summary>
        /// Toggles the relay on or off.
        /// </summary>
        void Toggle();
    }
}
