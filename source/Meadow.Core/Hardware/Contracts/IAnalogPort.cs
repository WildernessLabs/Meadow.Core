using System;

namespace Meadow.Hardware
{
    public interface IAnalogPort : IPort<IAnalogChannelInfo>
    {
        IAnalogChannelInfo _channelInfo { get; }
    }
}
