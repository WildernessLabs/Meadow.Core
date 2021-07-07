namespace Meadow.Units.Conversions
{
	internal static class FrequencyConversions
	{
		public static double Convert(double value, Frequency.UnitType from, Frequency.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * frequencyConversions[(int)to] / frequencyConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] frequencyConversions =
		{
			1, //GHz
			1000, //MHz
			1000000, //kHz
			1000000000, //Hz
	    };
	}
}