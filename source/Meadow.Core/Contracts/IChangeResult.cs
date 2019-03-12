using System;
namespace Meadow
{
    public interface IChangeResult<T>
    {
        T New { get; set; }
        T Old { get; set; }
        T Delta { get; }
    }

    public interface INumericChangeResult<T> : IChangeResult<T>
    {
        T DeltaPercent { get; }
    }
}
