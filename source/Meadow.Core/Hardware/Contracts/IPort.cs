using System;

namespace Meadow.Hardware
{
    public interface IPort<C> : IDisposable where C : IChannelInfo
    {
        C Channel { get; }
        IPin Pin { get; }
    }
}
