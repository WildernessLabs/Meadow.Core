using System;
namespace Meadow.Peripherals.Sensors.Location
{
    /// <summary>
    /// Represents a positional point on a spherical axis.
    /// </summary>
    public struct DegreesMinutesSecondsPosition
    {
        /// <summary>
        /// Latitudinal: -90º to 90º
        /// Longitudinal: -180º to 180º
        /// </summary>
        public int Degrees;
        /// <summary>
        /// 0' to 60'
        /// </summary>
        public decimal Minutes;
        /// <summary>
        /// 0" to 60"
        /// </summary>
        public decimal seconds;
        /// <summary>
        /// Cardinal direction.
        /// </summary>
        public CardinalDirection Direction;


        public override string ToString()
        {
            var position = $"{this.Degrees:f2}º {this.Minutes:f2}' {this.seconds:f2}\"";
            switch (this.Direction) {
                case CardinalDirection.East:
                    position += "E";
                    break;
                case CardinalDirection.West:
                    position += "W";
                    break;
                case CardinalDirection.North:
                    position += "N";
                    break;
                case CardinalDirection.South:
                    position += "S";
                    break;
                case CardinalDirection.Unknown:
                    position += "Unknown";
                    break;
            }
            return position;

        }
    }
}
