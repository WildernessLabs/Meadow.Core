using System;
using System.Collections.Generic;

namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    // TODO: Should this be a struct with fields?
    /// <summary>
    /// Satellite information to use in the GSV (Satellites in View) decoder.
    /// </summary>
    public class Satellite
    {
        /// <summary>
        /// Satellite ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Angle of elevation.
        /// </summary>
        public int Elevation { get; set; }

        /// <summary>
        /// Satellite azimuth.
        /// </summary>
        public int Azimuth { get; set; }

        /// <summary>
        /// Signal to noise ratio of the signal.
        /// </summary>
        public int SignalTolNoiseRatio;
    }
}
