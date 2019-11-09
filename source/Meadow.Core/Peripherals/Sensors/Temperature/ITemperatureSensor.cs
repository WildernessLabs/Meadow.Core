using System;
using System.Threading.Tasks;

namespace Meadow.Peripherals.Sensors.Temperature
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
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<FloatChangeResult> Updated;

        ///// <summary>
        ///// Convenience method to get the current temperature. For frequent reads, use
        ///// StartSampling() and StopSampling() in conjunction with the SampleBuffer.
        ///// </summary>
        ///// <param name="sampleCount">The number of sample readings to take. 
        ///// Must be greater than 0. These samples are automatically averaged.</param>
        ///// <param name="sampleIntervalDuration">The time, in milliseconds,
        ///// to wait in between samples during a reading.</param>
        ///// <returns>A float value that's ann average value of all the samples taken.</returns>
        //Task<float> ReadTemperature(int sampleCount = 10, int sampleIntervalDuration = 40);

        ///// <summary>
        ///// Starts continuously sampling the sensor.
        /////
        ///// This method also starts raising `Changed` events and IObservable
        ///// subscribers getting notified. Use the `readIntervalDuration` parameter
        ///// to specify how often events and notifications are raised/sent.
        ///// </summary>
        ///// <param name="sampleCount">How many samples to take during a given
        ///// reading. These are automatically averaged to reduce noise.</param>
        ///// <param name="sampleIntervalDuration">The time, in milliseconds,
        ///// to wait in between samples during a reading.</param>
        ///// <param name="standbyDuration">The time, in milliseconds, to wait
        ///// between sets of sample readings. This value determines how often
        ///// `Changed` events are raised and `IObservable` consumers are notified.</param>
        //void StartUpdating(
        //    int sampleCount = 10,
        //    int sampleIntervalDuration = 40,
        //    int standbyDuration = 1000);

        ///// <summary>
        ///// Stops sampling the temperature.
        ///// </summary>
        //void StopUpdating();

    }
}
