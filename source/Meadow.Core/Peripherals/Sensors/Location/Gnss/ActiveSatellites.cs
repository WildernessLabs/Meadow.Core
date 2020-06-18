#nullable enable

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
        public DimensionalFixType Dimensions { get; set; }

        /// <summary>
        /// Satellite selection type (Automatic or manual).
        /// </summary>
        public ActiveSatelliteSelection SatelliteSelection { get; set; }

        /// <summary>
        /// PRNs of the satellites used in the fix.
        /// </summary>
        public string[]? SatellitesUsedForFix { get; set; }

        /// <summary>
        /// Dilution of precision for the reading.
        /// </summary>
        public decimal DilutionOfPrecision { get; set; }

        /// <summary>
        /// Horizontal dilution of precision for the reading.
        /// </summary>
        public decimal HorizontalDilutionOfPrecision { get; set; }

        /// <summary>
        /// Vertical dilution of precision for the reading.
        /// </summary>
        public decimal VerticalDilutionOfPrecision { get; set; }
    }
}
