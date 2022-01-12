using System;

namespace Meadow.Units.Conversions
{
    /// <summary>
    /// Azimuth value conversions
    /// </summary>
    public static class AzimuthConversions
    {
        /// <summary>
        /// Compass 16 Cardinals to degrees
        /// </summary>
        // To Base (`CompassDegrees`)
        public static Func<Azimuth16PointCardinalNames, double> Compass16CardinalsToDegrees = (value) => {
            return value switch
            {
                Azimuth16PointCardinalNames.N => 0d,
                Azimuth16PointCardinalNames.NNE => 22.5d,
                Azimuth16PointCardinalNames.NE => 45d,
                Azimuth16PointCardinalNames.ENE => 67.5d,
                Azimuth16PointCardinalNames.E => 90d,
                Azimuth16PointCardinalNames.ESE => 112.5d,
                Azimuth16PointCardinalNames.SE => 135d,
                Azimuth16PointCardinalNames.SSE => 157.5d,
                Azimuth16PointCardinalNames.S => 180d,
                Azimuth16PointCardinalNames.SSW => 202.5d,
                Azimuth16PointCardinalNames.SW => 225d,
                Azimuth16PointCardinalNames.WSW => 247.5d,
                Azimuth16PointCardinalNames.W => 270d,
                Azimuth16PointCardinalNames.WNW => 292.5d,
                Azimuth16PointCardinalNames.NW => 315d,
                Azimuth16PointCardinalNames.NNW => 337.5d,
                _ => 0d,
            };
        };

        /// <summary>
        /// Degrees to compass cardinal names
        /// </summary>
        public static Func<double, Azimuth16PointCardinalNames> DegressToCompass16PointCardinalName = (value) => {
            /*
            N 348.75 - 11.25
            NNE 11.25 - 33.75
            NE 33.75 - 56.25
            ENE 56.25 - 78.75
            E 78.75 - 101.25
            ESE 101.25 - 123.75
            SE 123.75 - 146.25
            SSE 146.25 - 168.75
            S 168.75 - 191.25
            SSW 191.25 - 213.75
            SW 213.75 - 236.25
            WSW 236.25 - 258.75
            W 258.75 - 281.25
            WNW 281.25 - 303.75
            NW 303.75 - 326.25
            NNW 326.25 - 348.75
            */

            return value switch
            {
                double v when (v >= 348.75f && v < 11.25) => Azimuth16PointCardinalNames.N,
                double v when (v >= 11.25f && v < 33.75) => Azimuth16PointCardinalNames.NNE,
                double v when (v >= 33.75f && v < 56.25) => Azimuth16PointCardinalNames.NE,
                double v when (v >= 56.25f && v < 78.75) => Azimuth16PointCardinalNames.ENE,
                double v when (v >= 78.75f && v < 101.25) => Azimuth16PointCardinalNames.E,
                double v when (v >= 101.25f && v < 123.75) => Azimuth16PointCardinalNames.ESE,
                double v when (v >= 123.75f && v < 146.25) => Azimuth16PointCardinalNames.SE,
                double v when (v >= 146.25f && v < 168.75) => Azimuth16PointCardinalNames.SSE,
                double v when (v >= 168.75f && v < 191.25) => Azimuth16PointCardinalNames.S,
                double v when (v >= 191.25f && v < 213.75) => Azimuth16PointCardinalNames.SSW,
                double v when (v >= 213.75f && v < 236.25) => Azimuth16PointCardinalNames.SW,
                double v when (v >= 236.25f && v < 258.75) => Azimuth16PointCardinalNames.WSW,
                double v when (v >= 258.75f && v < 281.25) => Azimuth16PointCardinalNames.W,
                double v when (v >= 281.25f && v < 303.75) => Azimuth16PointCardinalNames.WNW,
                double v when (v >= 303.75f && v < 326.25) => Azimuth16PointCardinalNames.NW,
                double v when (v >= 326.25f && v < 348.75) => Azimuth16PointCardinalNames.NNW,
                _ => Azimuth16PointCardinalNames.N,
            };
        };

    }
}
