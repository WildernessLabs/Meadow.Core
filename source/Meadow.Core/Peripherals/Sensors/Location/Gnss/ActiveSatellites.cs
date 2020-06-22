using System;

namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    // TODO: Should this be a struct with fields?
    /// <summary>
    /// Active satellite information (GSA message information).
    /// </summary>
    public class ActiveSatellites : IGnssResult
    {
        /// <summary>
        /// The first two letters (after the starting delimiter) comprise the
        /// Talker ID, which describes the system in use, for instance "GL" means
        /// that the data came from the GLONASS system. "BD" means BeiDou, etc.
        ///
        /// Default value is "GP".
        /// </summary>
        public string TalkerID { get; set; } = "GP";

        /// <summary>
        /// Retreives the full name associated with the TalkerID via the
        /// `KnownTalkerIDs` property of the Lookups class.
        /// </summary>
        public string TalkerSystemName {
            get {
                string name = Lookups.KnownTalkerIDs[TalkerID];
                return (name != null) ? name : "";
            }
        }

        /// <summary>
        /// Time the reading was generated.
        /// </summary>
        public DateTime TimeOfReading { get; set; }

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
