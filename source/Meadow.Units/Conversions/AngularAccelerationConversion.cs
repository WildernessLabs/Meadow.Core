namespace Meadow.Units.Conversions
{
	internal static class AngularAccelerationConversions
	{
		public static double Convert(double value, AngularAcceleration.UnitType from, AngularAcceleration.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * angularAccelerationConversions[(int)to] / angularAccelerationConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] angularAccelerationConversions =
		{
			1, //revolutions per second^2
			3600, //revolutions per minute^2
			6.28318530717959, //radians per second^2
			22619.4671058465, //radians per minute^2
			360, //degrees per second^2
			1296000, //degrees per minute^2
		};
	}
}