using System;

namespace Meadow.Hardware
{
    public interface IAnalogPort : IPort<IAnalogChannelInfo>
    {
        IAnalogChannelInfo Channel { get; }
    }
}
