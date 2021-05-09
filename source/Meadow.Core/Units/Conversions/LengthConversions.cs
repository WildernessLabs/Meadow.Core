namespace Meadow.Units.Conversions
{
	internal static class LengthConversions
	{
		public static double Convert(double value, Length.UnitType from, Length.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * lengthConversions[(int)to] / lengthConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] lengthConversions =
		{
			0.001,//km
			1,//meter
			100,//cm
			10,//dm
			1000,//millimeter
			1000000,//micron / micrometer 
			1000000000,//nm
			0.00062137119224,//mile
			0.000539956803,//nautical miles
			1.0936132983,//yard
			3.280839895,//ft
			39.37007874,//inch
		};
	}
}