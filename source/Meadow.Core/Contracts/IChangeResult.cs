using System;
namespace Meadow
{
    public interface IChangeResult<T>
    {
        T Current { get; set; }
        T Previous { get; set; }
        T Delta { get; }
    }

    public interface INumericChangeResult<T> : IChangeResult<T>
    {
        T DeltaPercent { get; }
    }
}
