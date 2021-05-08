using System;
using Meadow.Units;

namespace Meadow.Peripherals.Sensors.Weather
{
    public interface IWindVane : ISensor
    {
        /// <summary>
        /// Raised when the azimuth of the wind changes.
        /// </summary>
        event EventHandler<IChangeResult<Azimuth>> Updated;

        /// <summary>
        /// The last recorded azimuth of the wind.
        /// </summary>
        Azimuth? WindAzimuth { get;}
    }
}
