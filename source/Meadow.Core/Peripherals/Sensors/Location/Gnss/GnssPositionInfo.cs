using System;
namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    // TODO: Should this be a struct with fields?
    /// <summary>
    /// Represents a GNSS/GPS position reading.
    /// </summary>
    public class GnssPositionInfo : IGnssResult
    {
        /// <summary>
        /// Time the reading was generated.
        /// </summary>
        public DateTime TimeOfReading { get; set; }

        /// <summary>
        /// Indicate if the data is valid or not.
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// Current speed in Knots.
        /// </summary>
        public decimal? SpeedInKnots { get; set; }

        //NOTE: pulled from PositionCourseAndTime
        /// <summary>
        /// Course in degrees (true heading).
        /// </summary>
        public decimal? CourseHeading { get; set; }

        //NOTE: pulled from PositionCourseAndTime
        /// <summary>
        /// Magnetic variation.
        /// </summary>
        public CardinalDirection MagneticVariation { get; set; } = CardinalDirection.Unknown;

        /// <summary>
        /// Global position
        /// </summary>
        public SphericalPositionInfo? Position { get; set; } = new SphericalPositionInfo();

        /// <summary>
        /// Quality of the fix.
        /// </summary>
        public FixType FixQuality { get; set; } = FixType.Invalid;

        /// <summary>
        /// Number of satellites used to generate the positional information.
        /// </summary>
        public int NumberOfSatellites { get; set; }

        /// <summary>
        /// Horizontal dilution of position (HDOP).
        /// </summary>
        public decimal HorizontalDilutionOfPrecision { get; set; }

        public GnssPositionInfo()
        {
        }
    }
}
