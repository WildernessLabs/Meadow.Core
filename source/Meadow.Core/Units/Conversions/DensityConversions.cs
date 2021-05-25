namespace Meadow.Units.Conversions
{
	internal static class DensityConversions
	{
		public static double Convert(double value, Density.UnitType from, Density.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * densityConversions[(int)to] / densityConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] densityConversions =
		{
			0.001,//gram cm^3
            1000,//gram m^3
            1.0,//gram/l
            1.0,//kg m^3
            0.000578036672,//ounce inch
            0.99884736919,//ounce ft
            0.000036127292,//pound /inch
            0.062427960576,//pound /ft^3
            0.0010018032458,//water (20)
	    };
	}
}