using System;
namespace Meadow
{
    public class FloatChangeResult : NumericChangeResultBase<float>
    {
        public override float Delta {
            get => Current - Previous;
        }
        public override float DeltaPercent {
            get => (Delta / Previous) * 100;
        }
    }
}