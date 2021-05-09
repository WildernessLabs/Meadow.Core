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
        }
    }
}
