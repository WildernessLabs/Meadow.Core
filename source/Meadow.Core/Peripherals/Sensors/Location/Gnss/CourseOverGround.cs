using System;
namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    /// <summary>
    /// Decoded data for the VTG - Course over ground and ground speed messages.
    /// </summary>
    public struct CourseOverGround
    {
        /// <summary>
        /// True heading in degrees.
        /// </summary>
        public double TrueHeading;

        /// <summary>
        /// Magnetic heading.
        /// </summary>
        public double MagneticHeading;

        /// <summary>
        /// Speed measured in knots.
        /// </summary>
        public double Knots;

        /// <summary>
        /// Speed measured in kilometers per hour.
        /// </summary>
        public double Kph;
    }
}
