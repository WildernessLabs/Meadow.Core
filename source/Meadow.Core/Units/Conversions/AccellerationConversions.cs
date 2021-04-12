using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Units.Conversions
{
    internal static class AccellerationConversions
    {
		public static double Convert(double value, Accelleration.UnitType from, Accelleration.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * accelConversions[(int)to] / accelConversions[(int)from];
		}

		private static readonly double[] accelConversions =
		{
			1.0, //meters per second squared
			100.0, //cm per s squared
            0.1019716213,//gravity
            3.280839895,//feet per second squared
            39.37007874,//inches per s squared
    	};
	}
}
