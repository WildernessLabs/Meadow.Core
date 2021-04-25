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
            1.0,    // ppm
            1000,   // ppb
    	};
    }
}