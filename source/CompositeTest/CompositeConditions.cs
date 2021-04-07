using Meadow.Units;

namespace CompositeTest
{
    public class CompositeConditions<U1>
        where U1 : IUnitType
    {
    }

    public class CompositeConditions<U1, U2>
        where U1 : IUnitType
        where U2 : IUnitType
    {
    }

    public class CompositeConditions<U1, U2, U3>
        where U1 : IUnitType
        where U2 : IUnitType
        where U3 : IUnitType
    {
    }
}
