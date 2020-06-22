using System;
namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    // TODO: Should this be a struct with fields?
    /// <summary>
    /// Decoded data for the VTG - Course over ground and ground speed messages.
    /// </summary>
    public class CourseOverGround : IGnssResult
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
        public DateTime? TimeOfReading { get; set; }

        /// <summary>
        /// True heading in degrees.
        /// </summary>
        public decimal TrueHeading { get; set; }

        /// <summary>
        /// Magnetic heading.
        /// </summary>
        public decimal MagneticHeading { get; set; }

        /// <summary>
        /// Speed measured in knots.
        /// </summary>
        public decimal Knots { get; set; }

        /// <summary>
        /// Speed measured in kilometers per hour.
        /// </summary>
        public decimal Kph { get; set; }
    }
}
