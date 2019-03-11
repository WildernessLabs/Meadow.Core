using System;
using System.Threading.Tasks;

namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for ports that implement an analog input channel.
    /// </summary>
    public interface IAnalogInputPort : IAnalogPort
    {
        int Read();

//        Task<byte> Read(int sampleCount, int sampleInterval);
//        Task<byte> ReadVoltage(
//            int sampleCount,
//            int sampleInterval,
//            float referenceVoltage);
    }
}
