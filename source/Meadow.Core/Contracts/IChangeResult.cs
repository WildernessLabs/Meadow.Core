using System;
namespace Meadow
{
    /// <summary>
    /// Contract for change notifications.
    /// </summary>
    /// <typeparam name="UNIT"></typeparam>
    public interface IChangeResult<UNIT>
        //where T : notnull //struct
        //where T: notnull//, IComparable //System.Collections.IStructuralEquatable, System.Collections.IStructuralComparable,
        where UNIT: struct
    {
        /// <summary>
        /// Current/new event value.
        /// </summary>
        UNIT New { get; set; }
        /// <summary>
        /// Previous value.
        /// </summary>
        UNIT? Old { get; set; }
        ///// <summary>
        ///// Change in value between `New` and `Old`.
        ///// </summary>
        //object Delta { get; }
    }

    /// <summary>
    /// Contract for change notifications, where the values are numeric.
    /// </summary>
    /// <typeparam name="UNIT">The type, such as a float or int.</typeparam>
    public interface INumericChangeResult<UNIT> : IChangeResult<UNIT>
        where UNIT : struct
        //where T : notnull//, IComparable// System.Collections.IStructuralEquatable, System.Collections.IStructuralComparable, 
    {
        /// <summary>
        /// Change in value, as a percentage, between `New` and `Old`.
        /// </summary>
        UNIT? DeltaPercent { get; }

        UNIT? Delta {get;}
    }

    public interface ITimeChangeResult : IChangeResult<DateTime>
    {
        TimeSpan? Delta { get; }
    }
}
