using Xunit;
using System;
using Meadow.Units;

namespace Meadow.Units.Tests
{
    public class TemperatureTests
    {
        [Fact()]
        public void TemperatureCtors()
        {
            Temperature t = new Temperature(32, Temperature.UnitType.Fahrenheit);
            Assert.True(t.Fahrenheit == 32);

            Temperature t2 = new Temperature();
            Assert.True(t2.Celsius == 0);
        }

        [Fact()]
        public void TemperatureConversions()
        {
            Temperature t = new Temperature(32, Temperature.UnitType.Fahrenheit);
            Assert.True(t.Celsius == 0);
            Assert.True(t.Kelvin == 273.15);

            Temperature t2 = new Temperature(0, Temperature.UnitType.Celsius);
            Assert.True(t2.Fahrenheit == 32);
            Assert.True(t2.Kelvin == 273.15);

            Temperature t3 = new Temperature(273.15, Temperature.UnitType.Kelvin);
            Assert.True(t3.Fahrenheit == 32);
            Assert.True(t3.Celsius == 0);
        }

        [Fact()]
        public void TemperatureMathOps()
        {
            Temperature t1 = new Temperature(1);
            Temperature t2 = new Temperature(10);
            Temperature t3 = new Temperature(-3);
            Assert.True(t1 != t2);
            Assert.True((t1 + t2) == new Temperature(11));
            Assert.True((t2 - t1) == new Temperature(9));

            Assert.True(t3.Abs() == new Temperature(3));
        }

        [Fact()]
        public void TemperatureComparisons()
        {
            Temperature t1 = new Temperature(1);
            Temperature t2 = new Temperature(10);
            Temperature t3 = new Temperature(10);

            Assert.True(t1 < t2);
            Assert.True(t2 <= t3);
            Assert.True(t2 > t1);
            Assert.True(t3 >= t2);

            Assert.True(t2 == t3);

            Assert.True(t2.Celsius == 10);
            Assert.True(t2.Celsius > 5);
        }
    }
}
