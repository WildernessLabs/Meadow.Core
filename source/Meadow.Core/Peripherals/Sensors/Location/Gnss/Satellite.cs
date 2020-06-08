using System;
namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    /// <summary>
    /// Satellite information to use in the GSV (Satellites in View) decoder.
    /// </summary>
    public struct Satellite
    {
        /// <summary>
        /// Satellite ID.
        /// </summary>
        public string ID;

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
