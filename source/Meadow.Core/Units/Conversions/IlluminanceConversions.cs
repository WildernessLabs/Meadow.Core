namespace Meadow.Units.Conversions
{
	internal static class IlluminanceConversions
	{
		public static double Convert(double value, Illuminance.UnitType from, Illuminance.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * illuminanceConversions[(int)to] / illuminanceConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] illuminanceConversions =
		{
			0.001, //kilo lux 
			1.0, //lux
			0.0930578820026056//foot-candle
	    };
	}
}