using System;

namespace Meadow.Units.Tests
{
    public static class Helpers
    {
        public static bool Equals4DigitPrecision(double left, double right)
        {
            return Math.Abs(left - right) < 0.0001;
        }
        public static bool Equals3DigitPrecision(double left, double right)
        {
            return Math.Abs(left - right) < 0.001;
        }
    }
}
