using System;
namespace Meadow.Units.Conversions
{
    internal static class PressureConversions
    {
        public static Func<double, double> PaToPsi = (value) => (value / 6894.7572931783);
        public static Func<double, double> PaToAt = (value) => (value / 101325);
        public static Func<double, double> PaToBar = (value) => (value / 100000);

        public static Func<double, double> AtToPsi = (value) => (value * 14.695948775);
        public static Func<double, double> AtToPa = (value) => (value * 101325);
        public static Func<double, double> AtToBar = (value) => (value * 1.01325);

        public static Func<double, double> PsiToAt = (value) => (value * 0.0680459639);
        public static Func<double, double> PsiToBar = (value) => (value * 0.0689475729);
        public static Func<double, double> PsiToPascal = (value) => (value * 6894.7572931783);

        public static Func<double, double> BarToPa = (value) => (value * 100000);
        public static Func<double, double> BarToAt = (value) => (value / 1.01325);
        public static Func<double, double> BarToPsi = (value) => (value * 14.5038);

    }
}
