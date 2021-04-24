namespace Meadow.Units.Conversions
{
	internal static class DistanceConversions
	{
		public static double Convert(double value, Distance.UnitType from, Distance.UnitType to)
		{
			if (from == to) {
				return value;
			}
			return value * distanceConversions[(int)to] / distanceConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] distanceConversions =
		{
			1.0, //Meters,
			1000, //Milimeters,
			100, //Centimeters,
			0.000621371, //Miles,
			0.000539957, //NauticalMiles,
			39.3701, //Inches,
			3.28084, //Feet,
		};
	}
}