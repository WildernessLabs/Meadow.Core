using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for ports that implement an analog input channel.
    /// </summary>
    public interface IAnalogInputPort : IAnalogPort
    {
        /// <summary>
        /// Raised when the value of the reading changes.
        /// </summary>
        event EventHandler<FloatChangeResult> Changed;

        /// <summary>
        /// Gets the sample buffer.
        /// </summary>
        /// <value>The sample buffer.</value>
        IList<float> VoltageSampleBuffer { get; }

        float ReferenceVoltage { get; }

        float AverageVoltageBufferValue { get; }


        Task<float> Read(int sampleCount = 10, int sampleInterval = 40);
        void StartSampling(int sampleSize = 10, int sampleIntervalDuration = 40, int sampleSleepDuration = 0);
        void StopSampling();
    }
}
