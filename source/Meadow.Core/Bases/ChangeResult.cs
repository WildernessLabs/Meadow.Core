using Meadow.Units;

namespace Meadow
{
    /// <summary>
    /// Represents a change result from an event. Contains a `New` and an optional
    /// `Old` value which will likely be null on the first result within an event
    /// series.
    /// </summary>
    /// <typeparam name="UNIT">A unit type that carries the result data. Must be
    /// a `struct`. Will most often be a unit such as `Temperature` or `Mass`,
    /// but can also be a primitive datatype such as `int`, `float`, or even
    /// `DateTime`.</typeparam>
    public struct ChangeResult<UNIT> : IChangeResult<UNIT>
        where UNIT : struct
    {
        /// <summary>
        /// The value at the time of event or notification.
        /// </summary>
        public UNIT New { get; set; }
        /// <summary>
        /// The previous value evented or notified.
        /// </summary>
        public UNIT? Old { get; set; }

        /// <summary>
        /// Creates a new ChangeResult.
        /// </summary>
        /// <param name="newValue">The value at the time of event or notification.</param>
        /// <param name="oldValue">The previous value evented or notified.</param>
        public ChangeResult(UNIT newValue, UNIT? oldValue)
        {
            New = newValue;
            Old = oldValue;
        }
    }
}