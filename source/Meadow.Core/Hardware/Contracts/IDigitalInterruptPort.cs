using System;

namespace Meadow.Hardware
{
    public interface IDigitalInterruptPort
    {
        event EventHandler<DigitalInputPortEventArgs> Changed;

        uint DebounceDuration { get; set; }
        uint GlitchDuration { get; set; }
        InterruptMode InterruptMode { get; }
    }
}
