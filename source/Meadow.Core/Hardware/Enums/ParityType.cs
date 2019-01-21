using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Describes whether or not (and what type, if applicable) there is a bit
    /// within each serial message frame that serves an error detection mechanism.
    /// See: <a href="https://en.wikipedia.org/wiki/Parity_bit">Parity Bit</a>.
    /// </summary>
    public enum ParityType
    {
        None = 0,
        Odd = 1,
        Even = 2,
        Mark = 3,
        Space = 4
    }
}
