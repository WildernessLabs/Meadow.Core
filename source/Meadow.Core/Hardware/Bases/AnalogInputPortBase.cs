using System;
using System.Threading.Tasks;

namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for much of the common tasks of 
    /// implementing IAnalogInputPort
    /// </summary>
    public abstract class AnalogInputPortBase : AnalogPortBase, IAnalogInputPort
    {
        public bool IsSampling { get; protected set; } = false;

        protected AnalogInputPortBase(IPin pin, IAnalogChannelInfo channel)
            : base (pin, channel)
        {
        }

        public abstract Task<float> Read(int sampleCount = 10, int sampleInterval = 40);

        public abstract void StartSampling(int sampleSize = 10, int sampleIntervalDuration = 40, int sampleSleepDuration = 0);

        public abstract void StopSampling();
    }
}
