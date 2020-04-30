using System;

namespace Meadow.Peripherals.Sensors.Motion
{
    /// <summary>
    /// Represents a generic accelerometer sensor.
    /// </summary>
    public interface IAccelerometer :  ISensor, IObservable<AccelerationConditionChangeResult>
    {
        /// <summary>
        /// Last value read from the Temperature sensor.
        /// </summary>
        AccelerationConditions Conditions { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<AccelerationConditionChangeResult> Updated;

        ///// <summary>
        ///// Convenience method to get the current temperature. For frequent reads, use
        ///// StartSampling() and StopSampling() in conjunction with the SampleBuffer.
        ///// </summary>
        //Task<AccelerationConditions> Read();
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
