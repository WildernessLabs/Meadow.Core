using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Bases
{
    //public class CompositeChangeResult<(U1)> Result where U : SensorChangeResult<U>
    // {} 
    // public class CompositeChangeResult<(U1, U2)> where U1, : UnitType, U2 : UnitType

    //https://stackoverflow.com/questions/15578668/constraining-class-generic-type-to-a-tuple/15578908

    public class CompositeChangeResult<U1> 
        where U1 : UnitType
    {
        public U1 New { get; set; }

        public U1 Old { get; set; }

        public U1 Delta
        {
            get //need maths here
                ;// return (new UnitType(), new UnitType()); // (New.unit1 - Old.unit1, New.unit2 - Old.unit2);
        }

        public CompositeChangeResult(U1 newValue, U1 oldValue)
        {
            New = newValue;

            Old = oldValue;
        }
    }

    public class CompositeChangeResult<U1, U2>
        where U1 : UnitType
        where U2 : UnitType
    {
        public (U1 unit1, U2 unit2) New { get; set; }

        public (U1 unit1, U2 unit2) Old { get; set; }

        public (U1 unit1, U2 unit2) Delta
        {
            get //need maths here
                ;// return (new UnitType(), new UnitType()); // (New.unit1 - Old.unit1, New.unit2 - Old.unit2);
        }

        public CompositeChangeResult((U1 unit1, U2 unit2) newValue,
                                     (U1 unit1, U2 unit2) oldValue)
        {
            New = newValue;

            Old = oldValue;
        }
    }

    public class CompositeChangeResult<U1, U2, U3>
        where U1 : UnitType
        where U2 : UnitType
        where U3 : UnitType
    {
        public (U1 unit1, U2 unit2, U3 unit3) New { get; set; }

        public (U1 unit1, U2 unit2, U3 unit3) Old { get; set; }

        public (U1 unit1, U2 unit2, U3 unit3) Delta { get; }

        
        public CompositeChangeResult((U1 unit1, U2 unit2, U3 unit3) newValue,
            (U1 unit1, U2 unit2, U3 unit3) oldValue)
        {
            New = newValue;

            Old = oldValue;
        }
    }


    // public class CompositeChangeResult<(U1, U2, U3) > result 
    // { 
}
