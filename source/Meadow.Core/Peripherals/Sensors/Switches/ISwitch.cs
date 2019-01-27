using System;

namespace Meadow.Peripherals.Switches
{
    /// <summary>
    /// Represents a generic switch
    /// </summary>
    public interface ISwitch
    {
        /// <summary>
        /// Describes whether or not the switch circuit is closed/connected (IsOn = true), or open (IsOn = false).
        /// </summary>
        bool IsOn { get; }

        /// <summary>
        /// Raised when the switch circuit circuit is opened or closed.
        /// </summary>
        event EventHandler Changed;
    }
}
