using System;
using System.Threading.Tasks;
using Meadow.Peripherals.Sensors.Atmospheric;

namespace Meadow.Peripherals.Sensors.Temperature
{
    /// <summary>
    /// Temperature sensor interface requirements.
    /// </summary>
    public interface ITemperatureSensor : ISensor, IObservable<AtmosphericConditionChangeResult>
    {
        /// <summary>
        /// Last value read from the Temperature sensor.
        /// </summary>
        float Temperature { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<AtmosphericConditionChangeResult> Updated;

        ///// <summary>
        ///// Convenience method to get the current temperature. For frequent reads, use
        ///// StartSampling() and StopSampling() in conjunction with the SampleBuffer.
        ///// </summary>
        //Task<AtmosphericConditions> Read();
        ///// <summary>
        ///// Starts continuously sampling the sensor.
        /////
        ///// This method also starts raising `Changed` events and IObservable
        ///// subscribers getting notified.
        ///// </summary>
        //void StartUpdating();
        ///// <summary>
        ///// Stops sampling the temperature.
        ///// </summary>
        //void StopUpdating();
    }
}
