using System;

namespace Meadow.Hardware
{
    public interface IDigitalInterruptPort
    {
        event EventHandler<DigitalInputPortChangeResult> Changed;

        InterruptMode InterruptMode { get; }
        double DebounceDuration { get; set; }
        double GlitchDuration { get; set; }
    }
}
