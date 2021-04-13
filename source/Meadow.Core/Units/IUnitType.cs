using System;

namespace Meadow.Units
{
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