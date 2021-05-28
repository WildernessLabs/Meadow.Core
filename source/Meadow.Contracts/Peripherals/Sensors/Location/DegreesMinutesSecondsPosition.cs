using System;
namespace Meadow.Peripherals.Sensors.Location
{
    // TODO: Should this be a struct with fields?
    /// <summary>
    /// Represents a positional point on a spherical axis.
    /// </summary>
    public class DegreesMinutesSecondsPosition
    {
        /// <summary>
        /// Latitudinal: -90º to 90º
        /// Longitudinal: -180º to 180º
        /// </summary>
        public int Degrees { get; set; }
        /// <summary>
        /// 0' to 60'
        /// </summary>
        public decimal Minutes { get; set; }
        /// <summary>
        /// 0" to 60"
        /// </summary>
        public decimal seconds { get; set; }
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
