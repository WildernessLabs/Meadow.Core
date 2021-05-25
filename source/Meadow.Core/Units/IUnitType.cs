using System;

namespace Meadow.Units
{
    // TODO: I'm not sure we need this anymore. IChangeResult can now use
    // primitive types like float, etc.

    public interface IUnitType
    {
        public static IUnitType operator -(IUnitType lvalue, IUnitType rvalue)
        {
            throw new Exception("Must override IUnitType subtraction operator");
        }

        public static IUnitType operator +(IUnitType lvalue, IUnitType rvalue)
        {
            throw new Exception("Must override IUnitType addition operator");
        }

        public static IUnitType operator /(IUnitType lvalue, IUnitType rvalue)
        {
            throw new Exception("Must override IUnitType addition operator");
        }

        public static IUnitType operator *(IUnitType lvalue, IUnitType rvalue)
        {
            throw new Exception("Must override IUnitType addition operator");
        }

    }
}