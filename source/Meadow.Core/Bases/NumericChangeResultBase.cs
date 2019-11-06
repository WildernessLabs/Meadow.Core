using System;
namespace Meadow
{
    public abstract class NumericChangeResultBase<T> : INumericChangeResult<T>
    {
        public T New { get; set; }
        public T Old { get; set; }

        public abstract T Delta { get; }
        public abstract T DeltaPercent { get; }

        public NumericChangeResultBase(T newValue, T oldValue)
        {
            New = newValue;
            Old = oldValue;
        }
    }
}
