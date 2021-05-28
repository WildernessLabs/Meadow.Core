using System;

namespace Meadow.Units.Conversions
{
	internal static class EnergyConversions
	{
		public static double Convert(double value, Energy.UnitType from, Energy.UnitType to)
		{
			if (from == to)
			{
				return value;
			}
			return value * energyConversions[(int)to] / energyConversions[(int)from];
		}

		//must align to enum
		private static readonly double[] energyConversions =
		{
			0.00094845138281,	//BTU
		    0.23900573614,	//calories (thermo)
		    1.0,//joules
		    0.00023900573614,	//kilocals
		    0.001,//kilojoules
		    0.000000277777777778,	//kilwatt hours
		    9.4804342797*Math.Pow(10.0,-9.0), //therms
		    0.000277777777778,	//watt hours,
		    1.0,	//watt seconds
	    };
	}
}