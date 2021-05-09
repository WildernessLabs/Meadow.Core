using NUnit.Framework;
using System;
using Meadow.Units;

namespace Meadow.Units.Tests
{
    [TestFixture()]
    public class TemperatureTests
    {
        [Test()]
        public void TemperatureCtors()
        {
            Temperature t = new Temperature(32, Temperature.UnitType.Fahrenheit);
            Assert.That(t.Fahrenheit == 32);

            Temperature t2 = new Temperature();
            Assert.That(t2.Celsius == 0);

            Temperature t3 = Temperature.FromFahrenheit(32);
            Assert.That(t3.Fahrenheit == 32);
        }

        [Test()]
        public void TemperatureConversions()
        {
            Temperature t = new Temperature(32, Temperature.UnitType.Fahrenheit);
            Assert.That(t.Celsius == 0);
            Assert.That(t.Kelvin == 273.15);

            Temperature t2 = new Temperature(0, Temperature.UnitType.Celsius);
            Assert.That(t2.Fahrenheit == 32);
            Assert.That(t2.Kelvin == 273.15);

            Temperature t3 = new Temperature(273.15, Temperature.UnitType.Kelvin);
            Assert.That(t3.Fahrenheit == 32);
            Assert.That(t3.Celsius == 0);
        }

        [Test()]
        public void TemperatureMathOps()
        {
            Temperature t1 = new Temperature(1);
            Temperature t2 = new Temperature(10);
            Temperature t3 = new Temperature(-3);
            Assert.That(t1 != t2);
            Assert.That((t1 + t2) == new Temperature(11));
            Assert.That((t2 - t1) == new Temperature(9));
            Assert.That((t1 * t2) == new Temperature(10));
            Assert.That(t3.Abs() == new Temperature(3));
        }

        [Test()]
        public void TemperatureComparisons()
        {
            Temperature t1 = new Temperature(1);
            Temperature t2 = new Temperature(10);
            Temperature t3 = new Temperature(10);

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
