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
			1,		//grams (identity unit)
		    0.001,	//kg
			0.0352739619495804129156758082152,//ounces
		    0.0022046226218487758072297380134,	//pounds
		    0.000001,	//tonnes metric
		    0.0000011023113109243879036148690,	//Tons US Short ... 5000 pounds
		    0.0000009842065276110606282275616,//Tons UK Long
			15.4323584, // grains
			5, // Carats
		};
	}
}