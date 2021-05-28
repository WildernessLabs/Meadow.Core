namespace Meadow.Units.Conversions
{
	internal static class PowerConversions
	{
		public static double Convert(double value, Power.UnitType from, Power.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * powerConversions[(int)to] / powerConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] powerConversions =
		{
            0.000000001,		//gigawatts
            0.000001,			//mwatts
		    0.001,				//kwatts
		    1.0,				//watts
		    1000.0,				//milliwatt

            0.0013596216173,	//HP metric
            0.0013410220924,	//HP IT
        
            0.23884589663,		//cals/s
            14.330753798,		//cals/min
            859.84522786,		//cals/hour
		    0.00094781712087,	//BTU/sec
            0.056869027252,		//BTU/min
		    3.4121416351,		//BTU/hour
		
		    0.73756217557,		//ft-pounds/sec
            44.253730534,		//ft-pound/min
		    2655.2238321,		//ft-pounds/hour
            0.00028434513609399,//Tons refrigeration
	    };
	}
}