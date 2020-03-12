using System;

namespace Meadow.Peripherals.Sensors.Light
{
    /// <summary>
    /// Light sensor interface requirements.
    /// </summary>
    public interface ILightSensor : ISensor, IObservable<FloatChangeResult>
    {
        /// <summary>
        /// Last value read from the Light sensor.
        /// </summary>
        float Luminosity { get; }

        /// <summary>
        /// Raised when a change in light is detected.
        /// </summary>
        event EventHandler<FloatChangeResult> LightLevelChanged;
    }
}