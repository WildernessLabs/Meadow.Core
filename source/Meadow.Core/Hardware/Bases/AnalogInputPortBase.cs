using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for much of the common tasks of 
    /// implementing IAnalogInputPort
    /// </summary>
    public abstract class AnalogInputPortBase : AnalogPortBase, IAnalogInputPort
    {
        public override PortDirectionType Direction => PortDirectionType.Input;

        public abstract float RawValue { get; }

        public abstract float Voltage { get; }

        protected AnalogInputPortBase(IAnalogChannelInfo channelInfo)
            : base (channelInfo)
        {
        }
    }
}
