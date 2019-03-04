using System;

namespace Meadow.Hardware
{
    public interface IPort<C> : IDisposable where C : IChannelInfo
    {
        // have to move this up the chain because it winds up forcing only get;
        // whereas we need to add set; for BiDirectionalPort
        // TODO (bryan): have someone else validate that it's impossibru
        //PortDirectionType Direction { get; }

        SignalType SignalType { get; }
        C Channel { get; }
        IPin Pin { get; }
    }
}
