using System;
using System.Collections.Generic;

namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    //public class Satellites : List<Satellite>, IGnssResult
    //{
    //    public DateTime TimeOfReading { get; set; }
    //}

    /// <summary>
    /// Satellite information to use in the GSV (Satellites in View) decoder.
    /// </summary>
    public struct Satellite
    {
        /// <summary>
        /// Satellite ID.
        /// </summary>
        public int ID;

        /// <summary>
        /// Angle of elevation.
        /// </summary>
        public int Elevation;

        /// <summary>
        /// Satellite azimuth.
        /// </summary>
        public int Azimuth;

        /// <summary>
        /// Signal to noise ratio of the signal.
        /// </summary>
        public int SignalTolNoiseRatio;
    }
}
