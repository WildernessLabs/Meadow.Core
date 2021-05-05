using System;
namespace Meadow
{
    /// <summary>
    /// Contains event data for events that represent a change in float value.
    /// </summary>
    public class FloatChangeResult : NumericChangeResultBase<float>
    {
        /// <summary>
        /// Change in value between `New` and `Old`.
        /// </summary>
        public override float? Delta {
            get => New - Old;
        }
        /// <summary>
        /// Change in value, as a percentage, between `New` and `Old`.
        /// </summary>
        public override float? DeltaPercent {
            get => (Delta / Old) * 100;
        }

        /// <summary>
        /// Creates a new FloatChangeResult.
        /// </summary>
        /// <param name="newValue">Current event value.</param>
        /// <param name="oldValue">Previous event value.</param>
        public FloatChangeResult(float newValue, float oldValue)
            : base(newValue, oldValue)
        {
        }
    }
}