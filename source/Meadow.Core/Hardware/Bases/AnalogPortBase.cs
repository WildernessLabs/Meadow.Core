using System;
namespace Meadow.Hardware
{
    public abstract class AnalogPortBase : IAnalogPort
    {
        public SignalType SignalType => SignalType.Analog;

        public IAnalogChannelInfo ChannelInfo
        {
            get => _channelinfo;
        }
        protected IAnalogChannelInfo _channelinfo;

        public abstract PortDirectionType Direction { get; }

        public IAnalogChannelInfo _channelInfo => throw new NotImplementedException();

        protected AnalogPortBase(IAnalogChannelInfo channelInfo)
        {
            _channelinfo = channelInfo;
        }
    }
}
