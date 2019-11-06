using System;
using System.Threading.Tasks;
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

        /// <summary>
        /// Convenience method to get the current temperature. For frequent reads, use
        /// StartSampling() and StopSampling() in conjunction with the SampleBuffer.
        /// </summary>
        /// <param name="sampleCount">The number of sample readings to take. 
        /// must be greater than 0.</param>
        /// <param name="sampleInterval">The interval, in milliseconds, between
        /// sample readings.</param>
        /// <returns></returns>
        Task<float> Read(int sampleCount = 10, int sampleInterval = 40);

        /// <summary>
        /// Starts continuously sampling the temperature. Also triggers the
        /// events to fire, and IObservable subscribers to get notified.
        /// </summary>
        /// <param name="sampleCount"></param>
        /// <param name="sampleIntervalDuration"></param>
        /// <param name="sampleSleepDuration"></param>
        void StartUpdating(
            int sampleCount = 10,
            int sampleIntervalDuration = 40,
            int sampleSleepDuration = 0);

        /// <summary>
        /// Stops sampling the temperature.
        /// </summary>
        void StopUpdating();

    }
}
