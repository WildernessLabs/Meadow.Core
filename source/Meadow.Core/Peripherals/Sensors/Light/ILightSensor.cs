using Meadow.Bases;
using Meadow.Units;
using System;

namespace Meadow.Peripherals.Sensors.Light
{
    /// <summary>
    /// Light sensor interface requirements.
    /// </summary>
    public interface ILightSensor : ISensor
    {
        /// <summary>
        /// Last value read from the Light sensor.
        /// </summary>
        Illuminance Illuminance { get; }

        /// <summary>
        /// Raised when a change in light is detected.
        /// </summary>
        event EventHandler<CompositeChangeResult<Illuminance>> LuminosityUpdated;
    }
}