namespace Meadow.Units.Conversions
{
	internal static class AngularVelocityConversions
	{
		public static double Convert(double value, AngularVelocity.UnitType from, AngularVelocity.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * angularVelocityConversions[(int)to] / angularVelocityConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] angularVelocityConversions =
		{
			1, //revolutions per second
			60, //revolutions per minute
			6.28318530717959, //radians per second
			376.991118430775, //radians per minute
			360, //degrees per second
			21600, //degrees per minute
		};
	}
}