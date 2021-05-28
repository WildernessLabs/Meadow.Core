namespace Meadow.Units.Conversions
{
	internal static class SpeedConversions
	{
		public static double Convert(double value, Speed.UnitType from, Speed.UnitType to)
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
			196850.3937,//		ft/min		
		    3280.839895,//		ft/s		
		    3600.0,//		km/h
            60.0,//km/min
		    1.0,//		km/s
		    1943.8444925,//		knots			
		    60000.0,//		m/min
		    1000.0,//		m/s			
		    2236.9362922064,//		mph		
			37.2822715344,//mpm
			0.62137119224,//mps
		    0.000003335640952,//		c	
		    2.938669958//		mach	
	    };
	}
}