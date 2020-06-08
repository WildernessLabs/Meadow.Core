using System;
namespace Meadow.Peripherals.Sensors.Location
{
    public class SphericalPositionInfo
    {
        /// <summary>
        ///     Latitude of the reading.
        /// </summary>
        public DegreesMinutesSecondsPosition Latitude;

        /// <summary>
        ///     Longitude of the reading.
        /// </summary>
        public DegreesMinutesSecondsPosition Longitude;

        /// <summary>
        /// Altitude above mean sea level (m).
        public decimal Altitude;

        public SphericalPositionInfo()
        {
        }
    }
}
