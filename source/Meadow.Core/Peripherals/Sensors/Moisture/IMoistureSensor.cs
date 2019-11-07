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
        /// Starts continuously sampling the sensor.
        ///
        /// This method also starts raising `Changed` events and IObservable
        /// subscribers getting notified. Use the `readIntervalDuration` parameter
        /// to specify how often events and notifications are raised/sent.
        /// </summary>
        /// <param name="sampleCount">How many samples to take during a given
        /// reading. These are automatically averaged to reduce noise.</param>
        /// <param name="sampleIntervalDuration">The time, in milliseconds,
        /// to wait in between samples during a reading.</param>
        /// <param name="readIntervalDuration">The time, in milliseconds, to wait
        /// between sets of sample readings. This value determines how often
        /// `Changed` events are raised and `IObservable` consumers are notified.</param>
        void StartUpdating(
            int sampleCount = 10,
            int sampleIntervalDuration = 40,
            int readIntervalDuration = 0);

        /// <summary>
        /// Stops sampling the moisture level.
        /// </summary>
        void StopUpdating();
    }
}
