namespace Meadow.Units.Conversions
{
	internal static class CurrentConversions
	{
		public static double Convert(double value, Current.UnitType from, Current.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * currentConversions[(int)to] / currentConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] currentConversions =
		{
			1, //Amps
			1000, //Milliamps
			1000000, //Microamps
			0.001, //Kiloamps
			0.000001, //Megaamps
			0.000000001, //Gigaamps
		};
	}
}