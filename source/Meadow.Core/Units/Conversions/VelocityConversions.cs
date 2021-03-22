using System;
namespace Meadow.Units.Conversions
{
    internal static class VelocityConversions
    {
        // To Base (`Kmh`)
        public static Func<double, double> MphToKmh = (value) => (value * 1.609344f);
        public static Func<double, double> KnotsToKmh = (value) => (value * 1.852f);
        public static Func<double, double> MpsToKmh = (value) => (value * 3.6f);
        public static Func<double, double> FpsToKmh = (value) => (value * 1.09728f);

        // From Base
        public static Func<double, double> KmhToMph = (value) => (value / 1.609344f);
        public static Func<double, double> KmhToKnots = (value) => (value / 1.852f);
        public static Func<double, double> KmhToMps = (value) => (value / 3.6f);
        public static Func<double, double> KmhToFps = (value) => (value / 1.09728f);
    }
}
