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
        public UNIT New { get; set; }
        public UNIT? Old { get; set; }

        public ChangeResult(UNIT newValue, UNIT? oldValue)
        {
            New = newValue;
            Old = oldValue;
        }
    }
}