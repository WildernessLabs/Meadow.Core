using NUnit.Framework;
using System;
using Meadow.Units;
using static Meadow.Units.Tests.Helpers;

namespace Meadow.Units.Tests
{
    [TestFixture()]
    public class AzimuthTests
    {
        [Test()]
        public void AzimuthCtors()
        {
            Azimuth a = new Azimuth(0d);
            Assert.That(a.DecimalDegrees == 0);

            Azimuth a2 = new Azimuth();
            Assert.That(a.DecimalDegrees == 0);

            Azimuth a3 = Azimuth.FromCompass16PointCardinalName(Compass16PointCardinalNames.E);
            Assert.That(a3.Compass16PointCardinalName == Compass16PointCardinalNames.E);

            Azimuth v4 = Azimuth.FromDecimalDegrees(270);
            Assert.That(v4.DecimalDegrees == 270);
        }

        [Test()]
        public void AzimuthConversions()
        {
            Azimuth v = new Azimuth(90);
            Assert.That(v.Compass16PointCardinalName == Compass16PointCardinalNames.E);

            Azimuth v2 = new Azimuth(Compass16PointCardinalNames.NNW);
            Assert.That(v2.DecimalDegrees == 337.5);

            Azimuth v3 = new Azimuth(191.25);
            Assert.That(v3.Compass16PointCardinalName == Compass16PointCardinalNames.SSW);

        }
    }
}
