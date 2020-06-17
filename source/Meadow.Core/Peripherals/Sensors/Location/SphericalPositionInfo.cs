using System;
namespace Meadow.Peripherals.Sensors.Location
{
    public class SphericalPositionInfo
    {
        /// <summary>
        ///     Latitude of the reading.
        /// </summary>
        public DegreesMinutesSecondsPosition Latitude { get; set; }

        /// <summary>
        ///     Longitude of the reading.
        /// </summary>
        public DegreesMinutesSecondsPosition Longitude { get; set; }

        /// <summary>
        /// Altitude above mean sea level (m).
        public decimal Altitude { get; set; }

        public SphericalPositionInfo()
        {
        }
    }
}
