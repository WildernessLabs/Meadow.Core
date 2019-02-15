using Meadow.Bases;
using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for much of the common tasks of 
    /// implementing IAnalogPort
    /// </summary>
    public abstract class AnalogPortBase : MeadowObservableBase<float>, IAnalogPort
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
