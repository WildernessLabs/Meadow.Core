using NUnit.Framework;
using System;
using Meadow.Units;
using static Meadow.Units.Tests.Helpers;

namespace Meadow.Units.Tests
{
    [TestFixture()]
    public class VelocityTests
    {
        [Test()]
        public void VelocityCtors()
        {
            Velocity v = new Velocity(100, Velocity.UnitType.Fps);
            Assert.That(v.Fps == 100);

            Velocity v2 = new Velocity();
            Assert.That(v2.Kmh == 0);

            Velocity v3 = Velocity.FromMps(150);
            Assert.That(v3.Mps == 150);

            Velocity v4 = Velocity.FromKnots(100);
            Assert.That(v4.Knots == 100);
        }

        [Test()]
        public void VelocityConversions()
        {
            Velocity v = new Velocity(100, Velocity.UnitType.Mph);
            Assert.That(Equals3DigitPrecision(v.Kmh, 160.934));
            Assert.That(Equals3DigitPrecision(v.Knots, 86.8976));
            Assert.That(Equals3DigitPrecision(v.Mps, 44.7038));
            Assert.That(Equals3DigitPrecision(v.Fps, 146.6663));

            Velocity v2 = new Velocity(100, Velocity.UnitType.Kmh);
            Assert.That(Equals3DigitPrecision(v2.Mph, 62.1371));
            Assert.That(Equals3DigitPrecision(v2.Knots, 53.9957));
            Assert.That(Equals3DigitPrecision(v2.Mps, 27.7778));
            Assert.That(Equals3DigitPrecision(v2.Fps, 91.1344));

            Velocity v3 = new Velocity(100, Velocity.UnitType.Knots);
            Assert.That(Equals3DigitPrecision(v3.Kmh, 185.2));
            Assert.That(Equals3DigitPrecision(v3.Mph, 115.078));
            Assert.That(Equals3DigitPrecision(v3.Mps, 51.4444));
            Assert.That(Equals3DigitPrecision(v3.Fps, 168.781));

            Velocity v4 = new Velocity(100, Velocity.UnitType.Mps);
            Assert.That(Equals3DigitPrecision(v4.Kmh, 360));
            Assert.That(Equals3DigitPrecision(v4.Mph, 223.694));
            Assert.That(Equals3DigitPrecision(v4.Knots, 194.384));
            Assert.That(Equals3DigitPrecision(v4.Fps, 328.084));

            Velocity v5 = new Velocity(100, Velocity.UnitType.Fps);
            Assert.That(Equals3DigitPrecision(v5.Kmh, 109.728));
            Assert.That(Equals3DigitPrecision(v5.Mph, 68.1818));
            Assert.That(Equals3DigitPrecision(v5.Knots, 59.2484));
            Assert.That(Equals3DigitPrecision(v5.Mps, 30.48));

        }
    }
}
