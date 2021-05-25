namespace Meadow.Units.Conversions
{
	internal static class TorqueConversions
	{
		public static double Convert(double value, Torque.UnitType from, Torque.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * speedConversions[(int)to] / speedConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] speedConversions =
		{
			1.355817952,//newton-meter
            1.0,//foot pound
            0.13825495475,//kilogram-meter
            13.825495475,//kg-cm
            13825.495475,//g-cm
            12.0,//inch pound
            192.0,//inch oz     
            13558179.52,//dyne centimeter
	    };
	}
}