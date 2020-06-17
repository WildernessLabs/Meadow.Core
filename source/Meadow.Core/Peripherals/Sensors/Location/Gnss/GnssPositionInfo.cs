using System;
namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    /// <summary>
    /// Represents a GNSS/GPS position reading.
    /// </summary>
    public class GnssPositionInfo
    {
        //NOTE: pulled from PositionCourseAndTime
        /// <summary>
        /// Time the reading was generated.
        /// </summary>
        public DateTime TimeOfReading { get; set; }

        //NOTE: pulled from PositionCourseAndTime
        /// <summary>
        ///     Indicate if the data is valid or not.
        /// </summary>
        public bool Valid { get; set; }

        //NOTE: pulled from PositionCourseAndTime
        /// <summary>
        /// Current speed in Knots.
        /// </summary>
        public decimal SpeedInKnots { get; set; }

        //NOTE: pulled from PositionCourseAndTime
        /// <summary>
        /// Course in degrees (true heading).
        /// </summary>
        public decimal CourseHeading { get; set; }

        //NOTE: pulled from PositionCourseAndTime
        /// <summary>
        /// Magnetic variation.
        /// </summary>
        public CardinalDirection MagneticVariation { get; set; } = CardinalDirection.Unknown;

        /// <summary>
        /// Global position
        /// </summary>
        public SphericalPositionInfo Position { get; set; } = new SphericalPositionInfo();

        ///// <summary>
        ///// Time that the reading was taken.  The date component is fixed for each reading.
        ///// </summary>
        //public DateTime ReadingTime { get; set; }

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
