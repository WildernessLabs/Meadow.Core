using System;
using System.Threading.Tasks;

namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for ports that implement an analog input channel.
    /// </summary>
    public interface IAnalogInputPort : IAnalogPort
    {
        // TODO: BrianK - uncomment after adding FloatChangeResult
        //event EventHandler<FloatChangeResult> Changed;

//        bool IsSampling { get; }

        int Read();

//        void StartSampling(int sampleSize = 10, int sampleIntervalDuration = 40, int sampleSleepDuration = 0);
//        void StopSampling();
    }
}
