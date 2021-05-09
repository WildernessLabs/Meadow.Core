using NUnit.Framework;
using System;
using Meadow.Units;
using static Meadow.Units.Tests.Helpers;

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
            Assert.That(Equals3DigitPrecision(v.KilometersPerHour, 160.934));
            Assert.That(Equals3DigitPrecision(v.Knots, 86.8976));
            Assert.That(Equals3DigitPrecision(v.MetersPerSecond, 44.7038));
            Assert.That(Equals3DigitPrecision(v.FeetPerSecond, 146.6663));

            Speed v2 = new Speed(100, Speed.UnitType.KilometersPerHour);
            Assert.That(Equals3DigitPrecision(v2.MilesPerHour, 62.1371));
            Assert.That(Equals3DigitPrecision(v2.Knots, 53.9957));
            Assert.That(Equals3DigitPrecision(v2.MetersPerSecond, 27.7778));
            Assert.That(Equals3DigitPrecision(v2.FeetPerSecond, 91.1344));

            Speed v3 = new Speed(100, Speed.UnitType.Knots);
            Assert.That(Equals3DigitPrecision(v3.KilometersPerHour, 185.2));
            Assert.That(Equals3DigitPrecision(v3.MilesPerHour, 115.078));
            Assert.That(Equals3DigitPrecision(v3.MetersPerSecond, 51.4444));
            Assert.That(Equals3DigitPrecision(v3.FeetPerSecond, 168.781));

            Speed v4 = new Speed(100, Speed.UnitType.MetersPerSecond);
            Assert.That(Equals3DigitPrecision(v4.KilometersPerHour, 360));
            Assert.That(Equals3DigitPrecision(v4.MilesPerHour, 223.694));
            Assert.That(Equals3DigitPrecision(v4.Knots, 194.384));
            Assert.That(Equals3DigitPrecision(v4.FeetPerSecond, 328.084));

            Speed v5 = new Speed(100, Speed.UnitType.FeetPerSecond);
            Assert.That(Equals3DigitPrecision(v5.KilometersPerHour, 109.728));
            Assert.That(Equals3DigitPrecision(v5.MilesPerHour, 68.1818));
            Assert.That(Equals3DigitPrecision(v5.Knots, 59.2484));
            Assert.That(Equals3DigitPrecision(v5.MetersPerSecond, 30.48));

        }
    }
}
