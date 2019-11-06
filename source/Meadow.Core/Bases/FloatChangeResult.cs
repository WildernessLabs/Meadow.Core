using System;
namespace Meadow
{
    public class FloatChangeResult : NumericChangeResultBase<float>
    {
        public override float Delta {
            get => New - Old;
        }
        public override float DeltaPercent {
            get => (Delta / Old) * 100;
        }

        public FloatChangeResult(float newValue, float oldValue)
            : base(newValue, oldValue)
        {
        }
    }
}