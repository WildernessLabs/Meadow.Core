using System;
namespace Meadow.Units.Conversions
{
    internal static class PressureConversions
    {
        // To Base (`Pa`)
        public static Func<double, double> PsiToPa = (value) => (value * 6894.7572931783);
        public static Func<double, double> AtToPa = (value) => (value * 101325);
        public static Func<double, double> BarToPa = (value) => (value * 100000);

        // From Base
        public static Func<double, double> PaToPsi = (value) => (value / 6894.7572931783);
        public static Func<double, double> PaToAt = (value) => (value / 101325);
        public static Func<double, double> PaToBar = (value) => (value / 100000);

    }
}
