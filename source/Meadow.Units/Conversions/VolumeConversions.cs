namespace Meadow.Units.Conversions
{
	internal static class VolumeConversions
	{
		public static double Convert(double value, Volume.UnitType from, Volume.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * volumeConversions[(int)to] / volumeConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] volumeConversions =
		{
			1.0,		    //gal US
			133.2278701,    //ounces
	        0.13368055556 ,	//cubic feet
	        231.0,		    //cubic inches
	        3.785411784,	//liters
	        378.5411784,	//cl
	        3785.411784,	//ml
	        0.003785411784, //m3
	    };
	}
}