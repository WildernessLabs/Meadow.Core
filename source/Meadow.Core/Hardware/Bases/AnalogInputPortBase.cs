using System;
namespace Meadow.Hardware
{
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
