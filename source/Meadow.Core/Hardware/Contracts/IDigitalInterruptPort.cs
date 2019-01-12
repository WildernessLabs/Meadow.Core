using System;

namespace Meadow.Hardware
{
    public interface IDigitalInterruptPort : IDigitalInputPort
    {
        event EventHandler<PortEventArgs> Changed;

        bool InterrupEnabled { get; set; }
    }
}
