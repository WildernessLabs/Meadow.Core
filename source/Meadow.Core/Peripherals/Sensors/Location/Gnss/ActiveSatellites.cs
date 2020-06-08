using System;
namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    /// <summary>
    /// Active satellite information (GSA message information).
    /// </summary>
    public struct ActiveSatellites
    {
        /// <summary>
        /// Dimensional fix type (No fix, 2D or 3D?)
        /// </summary>
        public DimensionalFixType Dimensions;

        /// <summary>
        /// Satellite selection type (Automatic or manual).
        /// </summary>
        public ActiveSatelliteSelection SatelliteSelection;

        /// <summary>
        /// PRNs of the satellites used in the fix.
        /// </summary>
        public string[] SatellitesUsedForFix;

        /// <summary>
        /// Dilution of precision for the reading.
        /// </summary>
        public double DilutionOfPrecision;

        /// <summary>
        /// Horizontal dilution of precision for the reading.
        /// </summary>
        public double HorizontalDilutionOfPrecision;

        /// <summary>
        /// Vertical dilution of precision for the reading.
        /// </summary>
        public double VerticalDilutionOfPrecision;
    }
}
