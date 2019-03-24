using System;

namespace Meadow.Hardware
{
    public interface IDigitalInterruptPort
    {
        event EventHandler<DigitalInputPortEventArgs> Changed;

        bool InterrupEnabled { get; set; }
    }
}
