using System;
namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    /// <summary>
    /// Decoded data for the VTG - Course over ground and ground speed messages.
    /// </summary>
    public struct CourseOverGround : IGnssResult
    {
        /// <summary>
        /// Time the reading was generated.
        /// </summary>
        public DateTime TimeOfReading { get; set; }

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
