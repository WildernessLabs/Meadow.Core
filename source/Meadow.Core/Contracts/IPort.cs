using System;

namespace Meadow
{
    //TODO: add IDisposable
    public interface IPort //: IDisposable
    {
        PortDirectionType DirectionType { get; }
        SignalType SignalType { get; } 
    }
}
