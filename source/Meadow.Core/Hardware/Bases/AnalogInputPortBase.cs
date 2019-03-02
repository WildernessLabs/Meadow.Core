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
        public override PortDirectionType Direction => PortDirectionType.Input;

        protected AnalogInputPortBase(IAnalogChannel channelInfo)
            : base (channelInfo)
        {
        }

        public abstract Task<byte> Read(int sampleCount, int sampleInterval);
        public abstract Task<byte> ReadVoltage(int sampleCount, int sampleInterval, float referenceVoltage);
    }
}
