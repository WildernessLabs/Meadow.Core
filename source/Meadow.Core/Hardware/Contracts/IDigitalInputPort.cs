using System;

namespace Meadow.Hardware
{
    public interface IDigitalInputPort : IDigitalPort
    {
        PortDirectionType Direction { get; }
        bool State { get; }
    }
}
