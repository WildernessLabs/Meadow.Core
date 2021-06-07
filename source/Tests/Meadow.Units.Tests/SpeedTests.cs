using Xunit;
using System;
using Meadow.Units;

namespace Meadow.Units.Tests
{
    public class SpeedTests
    {
        [Fact()]
        public void SpeedCtors()
        {
            Speed v = new Speed(100, Speed.UnitType.FeetPerSecond);
            Assert.True(v.FeetPerSecond == 100);

            Speed v2 = new Speed();
            Assert.True(v2.KilometersPerHour == 0);

            Speed v3 = new Speed(150, Speed.UnitType.MetersPerSecond);
            Assert.True(v3.MetersPerSecond == 150);

            Speed v4 = new Speed(100, Speed.UnitType.Knots);
            Assert.True(v4.Knots == 100);
        }

        [Fact()]
        public void SpeedConversions()
        {
            Speed v = new Speed(100, Speed.UnitType.MilesPerHour);
            Assert.True(v.KilometersPerHour.Equals3DigitPrecision(160.934));
            Assert.True(v.Knots.Equals3DigitPrecision(86.8976));
            Assert.True(v.MetersPerSecond.Equals3DigitPrecision(44.7038));
            Assert.True(v.FeetPerSecond.Equals3DigitPrecision(146.6663));

            Speed v2 = new Speed(100, Speed.UnitType.KilometersPerHour);
            Assert.True(v2.MilesPerHour.Equals3DigitPrecision(62.1371));
            Assert.True(v2.Knots.Equals3DigitPrecision(53.9957));
            Assert.True(v2.MetersPerSecond.Equals3DigitPrecision(27.7778));
            Assert.True(v2.FeetPerSecond.Equals3DigitPrecision(91.1344));

            Speed v3 = new Speed(100, Speed.UnitType.Knots);
            Assert.True(v3.KilometersPerHour.Equals3DigitPrecision(185.2));
            Assert.True(v3.MilesPerHour.Equals3DigitPrecision(115.078));
            Assert.True(v3.MetersPerSecond.Equals3DigitPrecision(51.4444));
            Assert.True(v3.FeetPerSecond.Equals3DigitPrecision(168.781));

            Speed v4 = new Speed(100, Speed.UnitType.MetersPerSecond);
            Assert.True(v4.KilometersPerHour.Equals3DigitPrecision(360));
            Assert.True(v4.MilesPerHour.Equals3DigitPrecision(223.694));
            Assert.True(v4.Knots.Equals3DigitPrecision(194.384));
            Assert.True(v4.FeetPerSecond.Equals3DigitPrecision(328.084));

            Speed v5 = new Speed(100, Speed.UnitType.FeetPerSecond);
            Assert.True(v5.KilometersPerHour.Equals3DigitPrecision(109.728));
            Assert.True(v5.MilesPerHour.Equals3DigitPrecision(68.1818));
            Assert.True(v5.Knots.Equals3DigitPrecision(59.2484));
            Assert.True(v5.MetersPerSecond.Equals3DigitPrecision(30.48));

        }
    }
}
