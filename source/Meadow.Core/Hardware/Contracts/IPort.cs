using System;

namespace Meadow.Hardware
{
    public interface IPort<C> : IDisposable where C : IChannelInfo
    {
        PortDirectionType Direction { get; }
        SignalType SignalType { get; }
        C Channel { get; }
        IPin Pin { get; }
    }
}
