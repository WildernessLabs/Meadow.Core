using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for much of the common tasks of 
    /// implementing IAnalogPort
    /// </summary>
    public abstract class AnalogPortBase : IAnalogPort
    {
        public SignalType SignalType => SignalType.Analog;

        public IAnalogChannel ChannelInfo
        {
            get => _channelinfo;
        }
        protected IAnalogChannel _channelinfo;

        public abstract PortDirectionType Direction { get; }

        public IAnalogChannel _channelInfo => throw new NotImplementedException();

        protected AnalogPortBase(IAnalogChannel channelInfo)
        {
            _channelinfo = channelInfo;
        }
    }
}
