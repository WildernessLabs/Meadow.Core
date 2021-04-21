using System;

namespace Meadow.Units.Conversions
{
	internal static class AngleConversions
	{
		public static double Convert(double value, Angle.UnitType from, Angle.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * angleConversions[(int)to] / angleConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] angleConversions =
		{
			1, //revolution
			360, //degrees
			2*Math.PI, //radians
			400, //gradians
			21600, //minutes
			1296000, //seconds
		};
	}
}