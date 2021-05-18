using NUnit.Framework;
using System;
using Meadow.Units;

namespace Meadow.Units.Tests
{
    [TestFixture()]
    public class MassTests
    {
        [Test()]
        public void MassCtors()
        {
            Mass t = new Mass(100, Mass.UnitType.Grams);
            Assert.That(t.Grams == 100);

            Mass t2 = new Mass();
            Assert.That(t2.Grams == 0);
        }

        [Test()]
        public void MassConversions()
        {
            Mass t = new Mass(1000, Mass.UnitType.Grams);
            Assert.That(t.Kilograms.Equals3DigitPrecision(1));
            Assert.That(t.Ounces.Equals4DigitPrecision(35.274));
            Assert.That(t.Pounds.Equals4DigitPrecision(2.20462));
            Assert.That(t.TonsMetric.Equals4DigitPrecision(0.001));
            Assert.That(t.TonsUSShort.Equals4DigitPrecision(0.00110231));
            Assert.That(t.TonsUKLong.Equals4DigitPrecision(0.000984207));
            Assert.That(t.Grains.Equals4DigitPrecision(15432.3584));
            Assert.That(t.Karats.Equals4DigitPrecision(5000));


            Mass t2 = new Mass(10, Mass.UnitType.Pounds);
            Assert.That(t2.Kilograms.Equals4DigitPrecision(4.53592));

            Mass t3 = new Mass(50, Mass.UnitType.Grains);
            Assert.That(t3.Grams.Equals4DigitPrecision(3.23995));
        }

        [Test()]
        public void MassMathOps()
        {
            Mass t1 = new Mass(1, Mass.UnitType.Kilograms);
            Mass t2 = new Mass(10, Mass.UnitType.Kilograms);
            Mass t3 = new Mass(-3, Mass.UnitType.Kilograms);
            Assert.That(t1 != t2);
            Assert.That((t1 + t2) == new Mass(11, Mass.UnitType.Kilograms));
            Assert.That((t2 - t1) == new Mass(9, Mass.UnitType.Kilograms));

            // problem: this doesn't make any sense
            Assert.That((t1 * t2) == new Mass(10, Mass.UnitType.Kilograms));

            // problem: this uses an implicit operator of `Mass(int value)`
            Assert.That((t1 * 10) == new Mass(10, Mass.UnitType.Kilograms));
            Assert.That(t3.Abs() == new Mass(3, Mass.UnitType.Kilograms));
        }

        [Test()]
        public void MassComparisons()
        {
            Mass t1 = new Mass(1);
            Mass t2 = new Mass(10);
            Mass t3 = new Mass(10);

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
