namespace Meadow.Units.Conversions
{
    internal static class MassConversions
    {
        public static double Convert(double value, Mass.UnitType from, Mass.UnitType to)
        {
			if(from == to)
            {
				return value;
            }
			return value * massConversions[(int)to] / massConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] massConversions =
		{
			1000.0,//grams
		    1.0,	//kg
		    35.273961949580412915675808215204,//ounces
		    2.2046226218487758072297380134503,	//pounds
		    0.001,	//tonnes metric
		    0.0011023113109243879036148690067251,	//Tons US Short ... 5000 pounds
		    0.00098420652761106062822756161314744,//Tons UK Long
			15432.3584, // grains
			5000, // karats
		};
	}
}