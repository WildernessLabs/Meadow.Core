namespace Meadow.Units.Conversions
{
	internal static class MagneticFieldConversions
	{
		public static double Convert(double value, MagneticField.UnitType from, MagneticField.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * magneticFieldConversions[(int)to] / magneticFieldConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] magneticFieldConversions =
		{
			0.000001, //MegaTesla
			0.001, //KiloTesla
			1.0, //Tesla
			1000.0,//MilliTesla
			1000000.0, //MicroTesla
			1000000000.0, //NanoTesla
			1000000000000.0, //PicoTesla
			10000.0, //Gauss
	    };
	}
}