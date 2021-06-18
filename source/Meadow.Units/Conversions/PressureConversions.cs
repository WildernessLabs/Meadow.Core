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


		public static double Convert(double value, Pressure.UnitType from, Pressure.UnitType to)
		{
			if (from == to) {
				return value;
			}
			return value * pressureConversions[(int)to] / pressureConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] pressureConversions =
		{
			1,//Bar
			100000, //Pascal,
			14.503773773, //Psi,
			0.9869232667, //StandardAtmosphere,
			1000, // mBar
			1000, // hPa (yes, same as millibar, but both are common)
			100, //kPa
	    };

	}
}
