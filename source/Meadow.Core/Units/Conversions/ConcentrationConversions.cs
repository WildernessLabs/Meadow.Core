namespace Meadow.Units.Conversions
{
    internal static class ConcentrationConversions
    {
        public static double Convert(double value, Concentration.UnitType from, Concentration.UnitType to)
        {
            if (from == to)
            {
                return value;
            }

            return value * concentrationConversions[(int)to] / concentrationConversions[(int)from];
        }

        private static readonly double[] concentrationConversions =
        {
            1.0,//pph
            10.0,//ppt
            10000.0,    // ppm
            10000000.0,   // ppb
    	};
    }
}