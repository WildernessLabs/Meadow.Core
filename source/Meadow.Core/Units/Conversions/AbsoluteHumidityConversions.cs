namespace Meadow.Units.Conversions
{
	internal static class AbsoluteHumidityConversions
	{
		public static double Convert(double value, AbsoluteHumidity.UnitType from, AbsoluteHumidity.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * humidityConversions[(int)to] / humidityConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] humidityConversions =
		{
			1000.0,//grams per cubic meter
			1.0,//kilograms per cubic meter
	    };
	}
}