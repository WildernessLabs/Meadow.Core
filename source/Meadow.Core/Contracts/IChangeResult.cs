using System;
namespace Meadow
{
    /// <summary>
    /// Contract for change notifications.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IChangeResult<T>
        //where T : notnull //struct
        //where T: notnull//, IComparable //System.Collections.IStructuralEquatable, System.Collections.IStructuralComparable,
        where T: struct
    {
        /// <summary>
        /// Current/new event value.
        /// </summary>
        T New { get; set; }
        /// <summary>
        /// Previous value.
        /// </summary>
        T? Old { get; set; }
        ///// <summary>
        ///// Change in value between `New` and `Old`.
        ///// </summary>
        //object Delta { get; }
    }

    /// <summary>
    /// Contract for change notifications, where the values are numeric.
    /// </summary>
    /// <typeparam name="T">The type, such as a float or int.</typeparam>
    public interface INumericChangeResult<T> : IChangeResult<T>
        where T : struct
        //where T : notnull//, IComparable// System.Collections.IStructuralEquatable, System.Collections.IStructuralComparable, 
    {
        /// <summary>
        /// Change in value, as a percentage, between `New` and `Old`.
        /// </summary>
        T? DeltaPercent { get; }

        T? Delta {get;}
    }

    public interface ITimeChangeResult : IChangeResult<DateTime>
    {
        TimeSpan? Delta { get; }
    }
}
