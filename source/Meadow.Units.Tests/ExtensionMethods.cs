using System;
namespace Meadow.Units.Tests
{
    public static class ExtensionMethods
    {
        public static bool Equals4DigitPrecision(this double source, double target)
        {
            return Math.Abs(source - target) < 0.0001;
        }

        public static bool Equals3DigitPrecision(this double source, double target)
        {
            return Math.Abs(source - target) < 0.001;
        }
    }
}
