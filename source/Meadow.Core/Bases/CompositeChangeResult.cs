using Meadow.Units;

namespace Meadow.Bases
{
    public class CompositeChangeResult<U1> : IChangeResult<U1>
        where U1 : IUnitType
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

            //set delta here
        }

        public CompositeChangeResult()
        {
        }
    }

    public class CompositeChangeResult<U1, U2> : IChangeResult<(U1 unit1, U2 unit2)>
        where U1 : IUnitType
        where U2 : IUnitType
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

        public CompositeChangeResult()
        {
        }
    }

    public class CompositeChangeResult<U1, U2, U3> : IChangeResult<(U1 unit1, U2 unit2, U3 unit3)>
        where U1 : IUnitType
        where U2 : IUnitType
        where U3 : IUnitType
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

        public CompositeChangeResult()
        {
        }
    }
}