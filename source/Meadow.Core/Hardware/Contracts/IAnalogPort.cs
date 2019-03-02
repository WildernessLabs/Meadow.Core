using System;

namespace Meadow.Hardware
{
    public interface IAnalogPort : IPort
    {
        IAnalogChannel _channelInfo { get; }
    }
}
