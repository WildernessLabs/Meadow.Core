﻿using Meadow.Units;

namespace Meadow
{
    /// <summary>
    /// Represents a change result from an event. Contains a `New` and an optional
    /// `Old` value which will likely be null on the first result within an event
    /// series.
    /// </summary>
    /// <typeparam name="UNIT">A unit type that carries the result data. Must be
    /// a `struct`. Will most often be a unit such as `Temperature` or `Mass`,
    /// but can also be a primitive datatype such as `int`, `float`, or even
    /// `DateTime`.</typeparam>
    public struct ChangeResult<UNIT> : IChangeResult<UNIT>
        where UNIT : struct
    {
        public UNIT New { get; set; }
        public UNIT? Old { get; set; }

        public UNIT? Delta {
            get //TODO: need maths here
                ;// return (new UnitType(), new UnitType()); // (New.Unit1 - Old.Unit1, New.Unit2 - Old.Unit2);
        }

        public ChangeResult(UNIT newValue, UNIT? oldValue)
        {
            New = newValue;
            Old = oldValue;
            Delta = null;
        }
    }

    //public struct CompositeChangeResult<U1> : IChangeResult<U1>
    //    where U1 : struct
    //    //where U1 : IUnitType
    //{
    //    public U1 New { get; set; }
    //    public U1? Old { get; set; }

    //    public U1? Delta
    //    {
    //        get //TODO: need maths here
    //            ;// return (new UnitType(), new UnitType()); // (New.Unit1 - Old.Unit1, New.Unit2 - Old.Unit2);
    //    }

    //    //public CompositeChangeResult() { }

    //    public CompositeChangeResult(U1 newValue, U1? oldValue)
    //    {
    //        New = newValue;
    //        Old = oldValue;
    //        Delta = null;
    //        //TODO: set delta here
    //    }
    //}

    //public struct CompositeChangeResult<U1, U2> : IChangeResult<(U1 Unit1, U2 Unit2)>
    //    where U1 : struct
    //    where U2 : struct
    //    //where U1 : IUnitType
    //    //where U2 : IUnitType
    //{
    //    public (U1 Unit1, U2 Unit2) New { get; set; }
    //    public (U1 Unit1, U2 Unit2)? Old { get; set; }
    //    public (U1 Unit1, U2 Unit2)? Delta
    //    {
    //        get //need maths here
    //            ;// return (new UnitType(), new UnitType()); // (New.Unit1 - Old.Unit1, New.Unit2 - Old.Unit2);
    //    }

    //    //public CompositeChangeResult() { }

    //    public CompositeChangeResult((U1 Unit1, U2 Unit2) newValue,
    //                                 (U1 Unit1, U2 Unit2) oldValue)
    //    {
    //        New = newValue;
    //        Old = oldValue;
    //        Delta = null;
    //    }
    //}

    //public struct CompositeChangeResult<U1, U2, U3> : IChangeResult<(U1 Unit1, U2 Unit2, U3 Unit3)>
    //    where U1 : struct
    //    where U2 : struct
    //    where U3 : struct
    //    //where U1 : IUnitType
    //    //where U2 : IUnitType
    //    //where U3 : IUnitType
    //{
    //    public (U1 Unit1, U2 Unit2, U3 Unit3) New { get; set; }
    //    public (U1 Unit1, U2 Unit2, U3 Unit3)? Old { get; set; }
    //    public (U1 Unit1, U2 Unit2, U3 Unit3)? Delta { get; }

    //    //public CompositeChangeResult() { }

    //    public CompositeChangeResult((U1 Unit1, U2 Unit2, U3 Unit3) newValue,
    //        (U1 Unit1, U2 Unit2, U3 Unit3) oldValue)
    //    {
    //        New = newValue;
    //        Old = oldValue;
    //        Delta = null;
    //    }
    //}

    //public struct CompositeChangeResult<U1, U2, U3, U4> : IChangeResult<(U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4)>
    //    where U1 : struct
    //    where U2 : struct
    //    where U3 : struct
    //    where U4 : struct
    //    //where U1 : IUnitType
    //    //where U2 : IUnitType
    //    //where U3 : IUnitType
    //    //where U4 : IUnitType
    //{
    //    public (U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4) New { get; set; }
    //    public (U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4)? Old { get; set; }
    //    public (U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4)? Delta { get; }

    //    //public CompositeChangeResult() { }

    //    public CompositeChangeResult((U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4) newValue,
    //        (U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4) oldValue)
    //    {
    //        New = newValue;
    //        Old = oldValue;
    //        Delta = null;
    //    }
    //}

    //public struct CompositeChangeResult<U1, U2, U3, U4, U5> : IChangeResult<(U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4, U5 Unit5)>
    //    where U1 : struct
    //    where U2 : struct
    //    where U3 : struct
    //    where U4 : struct
    //    where U5 : struct
    //    //where U1 : IUnitType
    //    //where U2 : IUnitType
    //    //where U3 : IUnitType
    //    //where U4 : IUnitType
    //    //where U5 : IUnitType
    //{
    //    public (U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4, U5 Unit5) New { get; set; }
    //    public (U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4, U5 Unit5)? Old { get; set; }
    //    public (U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4, U5 Unit5)? Delta { get; }

    //    //public CompositeChangeResult() { }

    //    public CompositeChangeResult((U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4, U5 Unit5) newValue,
    //        (U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4, U5 Unit5) oldValue)
    //    {
    //        New = newValue;
    //        Old = oldValue;
    //        Delta = null;
    //    }
    //}

    //public struct CompositeChangeResult<U1, U2, U3, U4, U5, U6> : IChangeResult<(U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4, U5 Unit5, U6 Unit6)>
    //    where U1 : struct
    //    where U2 : struct
    //    where U3 : struct
    //    where U4 : struct
    //    where U5 : struct
    //    where U6 : struct
    //    //where U1 : IUnitType
    //    //where U2 : IUnitType
    //    //where U3 : IUnitType
    //    //where U4 : IUnitType
    //    //where U5 : IUnitType
    //    //where U6 : IUnitType
    //{
    //    public (U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4, U5 Unit5, U6 Unit6) New { get; set; }
    //    public (U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4, U5 Unit5, U6 Unit6)? Old { get; set; }
    //    public (U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4, U5 Unit5, U6 Unit6)? Delta { get; }

    //    //public CompositeChangeResult() { }

    //    public CompositeChangeResult((U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4, U5 Unit5, U6 Unit6) newValue,
    //        (U1 Unit1, U2 Unit2, U3 Unit3, U4 Unit4, U5 Unit5, U6 Unit6) oldValue)
    //    {
    //        New = newValue;
    //        Old = oldValue;
    //        Delta = null;
    //    }
    //}
}