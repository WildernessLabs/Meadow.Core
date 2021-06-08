using Xunit;
using System;
using Meadow.Units;
using MU = Meadow.Units.Mass.UnitType;

namespace Meadow.Units.Tests
{
    public class MassTests
    {
        [Fact()]
        public void MassCtors()
        {
            Mass t = new Mass(100, MU.Grams);
            Assert.True(t.Grams == 100);

            Mass t2 = new Mass();
            Assert.True(t2.Grams == 0);
        }

        [Fact()]
        public void MassConversions()
        {
            Mass t = new Mass(1000, MU.Grams);
            Assert.True(t.Kilograms.Equals3DigitPrecision(1));
            Assert.True(t.Ounces.Equals4DigitPrecision(35.274));
            Assert.True(t.Pounds.Equals4DigitPrecision(2.20462));
            Assert.True(t.TonsMetric.Equals4DigitPrecision(0.001));
            Assert.True(t.TonsUSShort.Equals4DigitPrecision(0.00110231));
            Assert.True(t.TonsUKLong.Equals4DigitPrecision(0.000984207));
            Assert.True(t.Grains.Equals4DigitPrecision(15432.3584));
            Assert.True(t.Karats.Equals4DigitPrecision(5000));


            Mass t2 = new Mass(10, MU.Pounds);
            Assert.True(t2.Kilograms.Equals4DigitPrecision(4.53592));

            Mass t3 = new Mass(50, MU.Grains);
            Assert.True(t3.Grams.Equals4DigitPrecision(3.23995));
        }

        [Fact()]
        public void MassMathOps()
        {
            Mass t1 = new Mass(1, MU.Kilograms);
            Mass t2 = new Mass(10, MU.Kilograms);
            Mass t3 = new Mass(-3, MU.Kilograms);
            Assert.True(t1 != t2);
            Assert.True((t1 + t2) == new Mass(11, MU.Kilograms));
            Assert.True((t2 - t1) == new Mass(9, MU.Kilograms));

            Assert.True(t3.Abs() == new Mass(3, MU.Kilograms));
        }

        [Fact()]
        public void MassComparisons()
        {
            Mass t1 = new Mass(1);
            Mass t2 = new Mass(10);
            Mass t3 = new Mass(10);

            Assert.True(t1 < t2);
            Assert.True(t2 <= t3);
            Assert.True(t2 > t1);
            Assert.True(t3 >= t2);

            Assert.True(t2 == t3);

            Assert.True(t2 == new Mass(10, MU.Grams));
            Assert.True(t2 > new Mass(5, MU.Grams));
        }
    }
}
