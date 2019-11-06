using System;
using System.Threading.Tasks;

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

        /// <summary>
        /// Convenience method to get the current soil moisture. For frequent reads, use
        /// StartSampling() and StopSampling() in conjunction with the SampleBuffer.
        /// </summary>
        /// <param name="sampleCount">The number of sample readings to take. 
        /// must be greater than 0.</param>
        /// <param name="sampleInterval">The interval, in milliseconds, between
        /// sample readings.</param>
        /// <returns></returns>
        Task<float> Read(int sampleCount = 10, int sampleInterval = 40);

        /// <summary>
        /// Starts continuously sampling the moisture level. Also triggers the
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
        /// Stops sampling the moisture level.
        /// </summary>
        void StopUpdating();
    }
}
