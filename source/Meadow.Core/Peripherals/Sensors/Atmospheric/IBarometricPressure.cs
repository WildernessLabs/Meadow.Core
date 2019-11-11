using System;
using System.Threading.Tasks;

namespace Meadow.Peripherals.Sensors.Atmospheric
{
    /// <summary>
    /// Pressure sensor interface requirements.
    /// </summary>
    public interface IBarometricPressureSensor : ISensor, IObservable<AtmosphericConditionChangeResult>
    {
        /// <summary>
        /// Last value read from the Pressure sensor.
        /// </summary>
        float Pressure { get; }

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
