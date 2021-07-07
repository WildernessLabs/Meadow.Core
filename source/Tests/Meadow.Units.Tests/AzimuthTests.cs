using System;
using Meadow.Units;
using Xunit;

namespace Meadow.Units.Tests
{
    public class AzimuthTests
    {
        [Fact()]
        public void AzimuthCtors()
        {
            Azimuth a = new Azimuth(0d);
            Assert.True(a.DecimalDegrees == 0);

            Azimuth a2 = new Azimuth();
            Assert.True(a.DecimalDegrees == 0);

            Azimuth a3 = Azimuth.FromCompass16PointCardinalName(Azimuth16PointCardinalNames.E);
            Assert.True(a3.Compass16PointCardinalName == Azimuth16PointCardinalNames.E);

            Azimuth v4 = Azimuth.FromDecimalDegrees(270);
            Assert.True(v4.DecimalDegrees == 270);
        }

        [Fact()]
        public void AzimuthConversions()
        {
            Azimuth v = new Azimuth(90);
            Assert.True(v.Compass16PointCardinalName == Azimuth16PointCardinalNames.E);

            Azimuth v2 = new Azimuth(Azimuth16PointCardinalNames.NNW);
            Assert.True(v2.DecimalDegrees == 337.5);

            Azimuth v3 = new Azimuth(191.25);
            Assert.True(v3.Compass16PointCardinalName == Azimuth16PointCardinalNames.SSW);

        }
    }
}
