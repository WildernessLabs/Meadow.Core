using System;

namespace Meadow
{
    public interface IPort
    {
        PortDirectionType DirectionType { get; }
        SignalType SignalType { get; } 
    }
}
