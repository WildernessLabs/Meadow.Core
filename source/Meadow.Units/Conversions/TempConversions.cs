using System;
namespace Meadow.Units.Conversions
{
    internal static class TempConversions
    {
        public static Func<double, double> FToC = (value) => (value - 32D) * (5D / 9D);
        public static Func<double, double> FToK = (value) => (value - 32D) * (5D / 9D) + 273.15D;

        public static Func<double, double> CToF = (value) => value * (9D / 5D) + 32D;
        public static Func<double, double> CToK = (value) => value + 273.15D;

        public static Func<double, double> KToC = (value) => value - 273.15D;
        public static Func<double, double> KToF = (value) => (value - 273.15D) * (9D / 5D) + 32D;
    }
}
