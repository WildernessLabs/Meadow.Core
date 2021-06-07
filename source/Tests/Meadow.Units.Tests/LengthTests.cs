using Xunit;
using System;
using Meadow.Units;
using LU = Meadow.Units.Length.UnitType;

namespace Meadow.Units.Tests
{
    public class LengthTests
    {
        [Fact()]
        public void LengthCtors()
        {
            Length t = new Length(10, LU.Meters);
            Assert.True(t.Meters == 10);

            Length t2 = new Length(40);
            Assert.True(t2.Meters == 40);
        }

        [Fact()]
        public void LengthConversions()
        {
            Length t = new Length(10, LU.Meters);
            Assert.True(t.Kilometers == 0.01);
            Assert.True(t.Centimeters == 1000);
            Assert.True(t.Decimeters == 100);
            Assert.True(t.Millimeters == 10000);
            Assert.True(t.Microns == 10000000);
            Assert.True(t.Nanometer == 10000000000);
            Assert.True(t.Miles.Equals4DigitPrecision(0.00621371));
            Assert.True(t.NauticalMiles.Equals4DigitPrecision(0.00539957));
            Assert.True(t.Yards.Equals4DigitPrecision(10.9361));
            Assert.True(t.Feet.Equals4DigitPrecision(32.8084));
        }

        [Fact()]
        public void LengthMathOps()
        {
            Length t1 = new Length(1);
            Length t2 = new Length(10);
            Length t3 = new Length(-3);
            Assert.True(t1 != t2);
            Assert.True((t1 + t2) == new Length(11));
            Assert.True((t2 - t1) == new Length(9));

            Assert.True(t3.Abs() == new Length(3));
        }

        [Fact()]
        public void LengthComparisons()
        {
            Length t1 = new Length(1);
            Length t2 = new Length(10);
            Length t3 = new Length(10);

            Assert.True(t1 < t2);
            Assert.True(t2 <= t3);
            Assert.True(t2 > t1);
            Assert.True(t3 >= t2);

            Assert.True(t2 == t3);

            Assert.True(t2 == new Length(10, LU.Meters));
            Assert.True(t2 > new Length(5, LU.Meters));

            Assert.True(t2.CompareTo(t3) == 0);
            Assert.True(t2.CompareTo(t1) > 0);
            Assert.True(t1.CompareTo(t2) < 0);

            Assert.True(t2.Equals(t3));
            Assert.True(!t2.Equals(t1));
        }

        [Fact()]
        public void RandomTests()
        {
            // assignment
            Length i1 = new Length(100, LU.Meters);
            i1 = new Length(25);
            Assert.True(i1 == new Length(25));

            // more assignment
            Length i2 = i1;
            Assert.True(i2 == new Length(25, LU.Meters));

            Length i3 = new Length(i2);
            Assert.True(i3 == i2);
        }

        //[Fact()]
        //public void LengthTypeConversions()
        //{
        //    Length t1 = new Length(50, LU.Meters);

        //    Assert.True(t1.ToInt32(FormatProi) == 50);
        //}
    }
}
