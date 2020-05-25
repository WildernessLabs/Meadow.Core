using System;

namespace Meadow.Hardware
{
    public interface IDigitalInterruptPort
    {
        event EventHandler<DigitalInputPortEventArgs> Changed;

        InterruptMode InterruptMode { get; }
        double DebounceDuration { get; set; }
        double GlitchDuration { get; set; }
    }
}
