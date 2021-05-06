using System;
namespace Meadow
{
    /// <summary>
    /// Base class for event data that is numeric based and contains new and old
    /// value.
    /// </summary>
    /// <typeparam name="UNIT"></typeparam>
    public abstract class NumericChangeResultBase<UNIT> : INumericChangeResult<UNIT>
        where UNIT : struct
    {
        /// <summary>
        /// Current/new event value.
        /// </summary>
        public UNIT New { get; set; }
        /// <summary>
        /// Previous value.
        /// </summary>
        public UNIT? Old { get; set; }

        /// <summary>
        /// Change in value between `New` and `Old`.
        /// </summary>
        public abstract UNIT? Delta { get; }
        /// <summary>
        /// Change in value, as a percentage, between `New` and `Old`.
        /// </summary>
        public abstract UNIT? DeltaPercent { get; }

        /// <summary>
        /// Creates a new `NumericChangeResultBase`.
        /// </summary>
        /// <param name="newValue">Current event value.</param>
        /// <param name="oldValue">Previous event value.</param>
        public NumericChangeResultBase(UNIT newValue, UNIT? oldValue)
        {
            New = newValue;
            Old = oldValue;
        }
    }
}
