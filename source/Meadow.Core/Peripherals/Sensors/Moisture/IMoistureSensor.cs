using System;

namespace Meadow.Peripherals.Sensors.Moisture
{
    public interface IMoistureSensor : ISensor, IObservable<FloatChangeResult>
    {
        /// <summary>
        /// Last value read from the moisture sensor.
        /// </summary>
        float Moisture { get; }

        /// <summary>
        /// Raised when a change in moisture is detected.
        /// </summary>
        event EventHandler<FloatChangeResult> Changed;
    }
}
