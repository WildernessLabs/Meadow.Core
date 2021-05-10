using System;
namespace Meadow.Units.Conversions
{
    internal static class TempConversions
    {
        // To Base (`C°`)
        public static Func<double, double> FToC = (value) => (value - 32D) * (5D / 9D);
        public static Func<double, double> KToC = (value) => value - 273.15D;

        // From Base
        public static Func<double, double> CToF = (value) => value * (9D / 5D) + 32D;
        public static Func<double, double> CToK = (value) => value + 273.15D;


		public static double Convert(double value, Temperature.UnitType from, Temperature.UnitType to)
		{
			if (from == to) { return value; }

			if (from == Temperature.UnitType.Celsius && to == Temperature.UnitType.Fahrenheit) {
				return CToK(value);
			}
			if (from == Temperature.UnitType.Fahrenheit && to == Temperature.UnitType.Celsius) {

            }

			return value * temperatureConversions[(int)to] / temperatureConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] temperatureConversions =
		{
			1,//Celsius,
			-32*(5D/9D),//Fahrenheit, // NO WAY TO GET THIS TO WORK?
			//(9D / 5D) + 32D,
			273.15D//Kelvin,
		};
	}
}
