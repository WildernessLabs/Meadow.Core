using NUnit.Framework;
using System;
using Meadow.Units;

namespace Meadow.Units.Tests
{
    [TestFixture()]
    public class SpeedTests
    {
        [Test()]
        public void SpeedCtors()
        {
            Speed v = new Speed(100, Speed.UnitType.FeetPerSecond);
            Assert.That(v.FeetPerSecond == 100);

            Speed v2 = new Speed();
            Assert.That(v2.KilometersPerHour == 0);

            Speed v3 = new Speed(150, Speed.UnitType.MetersPerSecond);
            Assert.That(v3.MetersPerSecond == 150);

            Speed v4 = new Speed(100, Speed.UnitType.Knots);
            Assert.That(v4.Knots == 100);
        }

        [Test()]
        public void SpeedConversions()
        {
            Speed v = new Speed(100, Speed.UnitType.MilesPerHour);
            Assert.That(v.KilometersPerHour.Equals3DigitPrecision(160.934));
            Assert.That(v.Knots.Equals3DigitPrecision(86.8976));
            Assert.That(v.MetersPerSecond.Equals3DigitPrecision(44.7038));
            Assert.That(v.FeetPerSecond.Equals3DigitPrecision(146.6663));

            Speed v2 = new Speed(100, Speed.UnitType.KilometersPerHour);
            Assert.That(v2.MilesPerHour.Equals3DigitPrecision(62.1371));
            Assert.That(v2.Knots.Equals3DigitPrecision(53.9957));
            Assert.That(v2.MetersPerSecond.Equals3DigitPrecision(27.7778));
            Assert.That(v2.FeetPerSecond.Equals3DigitPrecision(91.1344));

            Speed v3 = new Speed(100, Speed.UnitType.Knots);
            Assert.That(v3.KilometersPerHour.Equals3DigitPrecision(185.2));
            Assert.That(v3.MilesPerHour.Equals3DigitPrecision(115.078));
            Assert.That(v3.MetersPerSecond.Equals3DigitPrecision(51.4444));
            Assert.That(v3.FeetPerSecond.Equals3DigitPrecision(168.781));

            Speed v4 = new Speed(100, Speed.UnitType.MetersPerSecond);
            Assert.That(v4.KilometersPerHour.Equals3DigitPrecision(360));
            Assert.That(v4.MilesPerHour.Equals3DigitPrecision(223.694));
            Assert.That(v4.Knots.Equals3DigitPrecision(194.384));
            Assert.That(v4.FeetPerSecond.Equals3DigitPrecision(328.084));

            Speed v5 = new Speed(100, Speed.UnitType.FeetPerSecond);
            Assert.That(v5.KilometersPerHour.Equals3DigitPrecision(109.728));
            Assert.That(v5.MilesPerHour.Equals3DigitPrecision(68.1818));
            Assert.That(v5.Knots.Equals3DigitPrecision(59.2484));
            Assert.That(v5.MetersPerSecond.Equals3DigitPrecision(30.48));

        }
    }
}
