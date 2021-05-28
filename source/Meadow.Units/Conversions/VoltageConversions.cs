namespace Meadow.Units.Conversions
{
	internal static class VoltageConversions
	{
		public static double Convert(double value, Voltage.UnitType from, Voltage.UnitType to)
		{
			if (from == to) {
				return value;
			}
			return value * voltageConversions[(int)to] / voltageConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] voltageConversions =
		{
			1, //Volts
			1000, //Millivolts
			1000000, //Microvolts
			0.001, //Kilovolts
			0.000001, //Megavolts
			0.000000001, //Gigavolts
			299.792458, //Statvolts
		};
	}
}