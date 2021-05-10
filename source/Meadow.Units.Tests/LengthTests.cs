using NUnit.Framework;
using System;
using Meadow.Units;

namespace Meadow.Units.Tests
{
    [TestFixture()]
    public class LengthTests
    {
        [Test()]
        public void LengthCtors()
        {
            Length t = new Length(10, Length.UnitType.Meters);
            Assert.That(t.Meters == 10);

            Length t2 = new Length(40);
            Assert.That(t2.Meters == 40);
        }

        [Test()]
        public void LengthConversions()
        {
            Length t = new Length(10, Length.UnitType.Meters);
            Assert.That(t.Kilometers == 0.01);
            Assert.That(t.Centimeters == 1000);
            Assert.That(t.Decimeters == 100);
            Assert.That(t.Millimeters == 10000);
            Assert.That(t.Microns == 10000000);
            Assert.That(t.Nanometer == 10000000000);
            Assert.That(t.Miles.Equals4DigitPrecision(0.00621371));
            Assert.That(t.NauticalMiles.Equals4DigitPrecision(0.00539957));
            Assert.That(t.Yards.Equals4DigitPrecision(10.9361));
            Assert.That(t.Feet.Equals4DigitPrecision(32.8084));
        }

        [Test()]
        public void LengthMathOps()
        {
            Length t1 = new Length(1);
            Length t2 = new Length(10);
            Length t3 = new Length(-3);
            Assert.That(t1 != t2);
            Assert.That((t1 + t2) == new Length(11));
            Assert.That((t2 - t1) == new Length(9));
            Assert.That((t1 * t2) == new Length(10));
            Assert.That(t3.Abs() == new Length(3));
        }

        [Test()]
        public void LengthComparisons()
        {
            Length t1 = new Length(1);
            Length t2 = new Length(10);
            Length t3 = new Length(10);

            Assert.That(t1 < t2);
            Assert.That(t2 <= t3);
            Assert.That(t2 > t1);
            Assert.That(t3 >= t2);

            Assert.That(t2 == t3);

            Assert.That(t2 == 10);
            Assert.That(t2 > 5);

            Assert.That(t2.CompareTo(t3) == 0);
            Assert.That(t2.CompareTo(t1) > 0);
            Assert.That(t1.CompareTo(t2) < 0);

            Assert.That(t2.Equals(t3));
            Assert.That(!t2.Equals(t1));
        }

        [Test()]
        public void ImplicitConversions()
        {
            Length i1 = (ushort)10;
            Assert.That(i1 == new Length(10));
            Length i2 = (short)10;
            Assert.That(i2 == new Length(10));
            Length i3 = (uint)10;
            Assert.That(i3 == new Length(10));
            Length i4 = (long)10;
            Assert.That(i4 == new Length(10));
            Length i5 = 10; // int32
            Assert.That(i5 == new Length(10));
            Length i6 = 10f; // float
            Assert.That(i6 == new Length(10));
            Length i7 = (double)10;
            Assert.That(i7 == new Length(10));
            Length i8 = 10m; // decimal
            Assert.That(i8 == new Length(10));
        }

        [Test()]
        public void RandomTests()
        {
            // assignment
            Length i1 = new Length(100, Length.UnitType.Meters);
            i1 = new Length(25);
            Assert.That(i1 == new Length(25));

            // more assignment
            Length i2 = i1;
            Assert.That(i2 == new Length(25, Length.UnitType.Meters));

            Length i3 = new Length(i2);
            Assert.That(i3 == i2);
        }

        //[Test()]
        //public void LengthTypeConversions()
        //{
        //    Length t1 = new Length(50, Length.UnitType.Meters);

        //    Assert.That(t1.ToInt32(FormatProi) == 50);
        //}
    }
}
