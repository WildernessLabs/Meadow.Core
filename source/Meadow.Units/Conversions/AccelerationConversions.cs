namespace Meadow.Units.Conversions
{
    internal static class AccelerationConversions
    {
		public static double Convert(double value, Acceleration.UnitType from, Acceleration.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * accelConversions[(int)to] / accelConversions[(int)from];
		}

		private static readonly double[] accelConversions =
		{
			1.0, //meters per second squared
			100.0, //cm per s squared
			100.0, // gal (galileo)
			100000.0, // milligal
			0.0001019716213, //milli gravity
			0.1019716213,//gravity
			3.280839895,//feet per second squared
			39.37007874,//inches per s squared
		};
	}
}