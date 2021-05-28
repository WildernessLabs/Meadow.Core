using System;
namespace Meadow.Peripherals.Sensors.Location
{
    // TODO: Should this be a struct with fields?
    /// <summary>
    /// Represents a position on a globe or sphere, including `Latitude`,
    /// `Longitude`, and `Altitude`.
    /// </summary>
    public class SphericalPositionInfo
    {
        /// <summary>
        /// Latitude of the reading.
        /// </summary>
        public DegreesMinutesSecondsPosition? Latitude { get; set; }

        /// <summary>
        /// Longitude of the reading.
        /// </summary>
        public DegreesMinutesSecondsPosition? Longitude { get; set; }

        /// <summary>
        /// Altitude above mean sea level in meters.
        public decimal? Altitude { get; set; }

        public SphericalPositionInfo()
        {
        }
    }
}
