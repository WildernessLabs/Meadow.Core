using System;

namespace Meadow.Units.Conversions
{
    public static class AzimuthConversions
    {
        // To Base (`CompassDegrees`)
        public static Func<Azimuth16PointCardinalNames, double> Compass16CardinalsToDegrees = (value) => {
            switch (value) {
                case Azimuth16PointCardinalNames.N:
                    return 0d;
                case Azimuth16PointCardinalNames.NNE:
                    return 22.5d;
                case Azimuth16PointCardinalNames.NE:
                    return 45d;
                case Azimuth16PointCardinalNames.ENE:
                    return 67.5d;
                case Azimuth16PointCardinalNames.E:
                    return 90d;
                case Azimuth16PointCardinalNames.ESE:
                    return 112.5d;
                case Azimuth16PointCardinalNames.SE:
                    return 135d;
                case Azimuth16PointCardinalNames.SSE:
                    return 157.5d;
                case Azimuth16PointCardinalNames.S:
                    return 180d;
                case Azimuth16PointCardinalNames.SSW:
                    return 202.5d;
                case Azimuth16PointCardinalNames.SW:
                    return 225d;
                case Azimuth16PointCardinalNames.WSW:
                    return 247.5d;
                case Azimuth16PointCardinalNames.W:
                    return 270d;
                case Azimuth16PointCardinalNames.WNW:
                    return 292.5d;
                case Azimuth16PointCardinalNames.NW:
                    return 315d;
                case Azimuth16PointCardinalNames.NNW:
                    return 337.5d;
                default:
                    return 0d;
            }
        };

        // From Base
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

            switch (value){
                case double v when (v >= 348.75f && v < 11.25):
                    return Azimuth16PointCardinalNames.N;
                case double v when (v >= 11.25f && v < 33.75):
                    return Azimuth16PointCardinalNames.NNE;
                case double v when (v >= 33.75f && v < 56.25):
                    return Azimuth16PointCardinalNames.NE;
                case double v when (v >= 56.25f && v < 78.75):
                    return Azimuth16PointCardinalNames.ENE;
                case double v when (v >= 78.75f && v < 101.25):
                    return Azimuth16PointCardinalNames.E;
                case double v when (v >= 101.25f && v < 123.75):
                    return Azimuth16PointCardinalNames.ESE;
                case double v when (v >= 123.75f && v < 146.25):
                    return Azimuth16PointCardinalNames.SE;
                case double v when (v >= 146.25f && v < 168.75):
                    return Azimuth16PointCardinalNames.SSE;
                case double v when (v >= 168.75f && v < 191.25):
                    return Azimuth16PointCardinalNames.S;
                case double v when (v >= 191.25f && v < 213.75):
                    return Azimuth16PointCardinalNames.SSW;
                case double v when (v >= 213.75f && v < 236.25):
                    return Azimuth16PointCardinalNames.SW;
                case double v when (v >= 236.25f && v < 258.75):
                    return Azimuth16PointCardinalNames.WSW;
                case double v when (v >= 258.75f && v < 281.25):
                    return Azimuth16PointCardinalNames.W;
                case double v when (v >= 281.25f && v < 303.75):
                    return Azimuth16PointCardinalNames.WNW;
                case double v when (v >= 303.75f && v < 326.25):
                    return Azimuth16PointCardinalNames.NW;
                case double v when (v >= 326.25f && v < 348.75):
                    return Azimuth16PointCardinalNames.NNW;
                default:
                    return Azimuth16PointCardinalNames.N;
            }
        };

    }
}
