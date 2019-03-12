using System;

namespace Meadow.Hardware
{
    public interface IDigitalInterruptPort
    {
        event EventHandler<PortEventArgs> Changed;

        bool InterrupEnabled { get; set; }
    }
}
