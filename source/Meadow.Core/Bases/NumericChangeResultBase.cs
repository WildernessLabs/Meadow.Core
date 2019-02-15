using System;
namespace Meadow
{
    public abstract class NumericChangeResultBase<T> : INumericChangeResult<T>
    {
        public T Current { get; set; }
        public T Previous { get; set; }

        public abstract T Delta { get; }
        public abstract T DeltaPercent { get; }
    }
}
