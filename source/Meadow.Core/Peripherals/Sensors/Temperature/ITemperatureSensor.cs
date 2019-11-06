using System;
using Meadow.Peripherals.Sensors;

namespace Meadow.Peripherals.Temperature
{
    /// <summary>
    /// Temperature sensor interface requirements.
    /// </summary>
    public interface ITemperatureSensor : ISensor, IObservable<FloatChangeResult>
    {
        /// <summary>
        /// Last value read from the Temperature sensor.
        /// </summary>
        float Temperature { get; }

        /// <summary>
        /// Raised when a change in temperature is detected.
        /// </summary>
        event EventHandler<FloatChangeResult> Changed;
    }
}
