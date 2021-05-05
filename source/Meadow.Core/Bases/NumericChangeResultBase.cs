using System;
namespace Meadow
{
    /// <summary>
    /// Base class for event data that is numeric based and contains new and old
    /// value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class NumericChangeResultBase<T> : INumericChangeResult<T>
        where T : struct
    {
        /// <summary>
        /// Current/new event value.
        /// </summary>
        public T New { get; set; }
        /// <summary>
        /// Previous value.
        /// </summary>
        public T? Old { get; set; }

        /// <summary>
        /// Change in value between `New` and `Old`.
        /// </summary>
        public abstract T? Delta { get; }
        /// <summary>
        /// Change in value, as a percentage, between `New` and `Old`.
        /// </summary>
        public abstract T? DeltaPercent { get; }

        /// <summary>
        /// Creates a new `NumericChangeResultBase`.
        /// </summary>
        /// <param name="newValue">Current event value.</param>
        /// <param name="oldValue">Previous event value.</param>
        public NumericChangeResultBase(T newValue, T? oldValue)
        {
            New = newValue;
            Old = oldValue;
        }
    }
}
