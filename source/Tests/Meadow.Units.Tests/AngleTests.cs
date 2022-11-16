using System;
using Xunit;

namespace Meadow.Units.Tests
{
    public class AngleTests
    {
        private Random _random = new Random();

        [Fact]
        public void ConstructorTests()
        {
            foreach (Angle.UnitType unit in Enum.GetValues(typeof(Angle.UnitType)))
            {
                var value = _random.NextDouble() * 360;
                var a = new Angle(value, unit);
                var p = typeof(Angle).GetProperty(Enum.GetName(typeof(Angle.UnitType), unit));

                Assert.Equal(Math.Round(value, 6), Math.Round((double)p.GetValue(a), 6));
            }            
        }

        [Fact]
        public void ZeroCrossTests()
        {
            var a1 = new Angle(270);
            var a2 = a1 + new Angle(180);
            Assert.Equal(90d, a2.Degrees);

            var a3 = new Angle(90);
            var a4 = a3 - new Angle(180);
            Assert.Equal(270d, a4.Degrees);

            var a5 = a3 * 5;
            Assert.Equal(90d, a5.Degrees);
        }

        [Fact]
        public void MultiplicationTests()
        {
            var initial = _random.NextDouble() * 360;
            var operand = _random.NextDouble() * 10;

            var a = new Angle(initial);
            a *= operand;
            Assert.Equal(initial * operand, a.Degrees);
        }

        [Fact]
        public void DivisionTests()
        {
            var initial = _random.NextDouble() * 360;
            var operand = _random.NextDouble() * 10;

            var a = new Angle(initial);
            a /= operand;
            Assert.Equal(initial / operand, a.Degrees);
        }
    }
}
