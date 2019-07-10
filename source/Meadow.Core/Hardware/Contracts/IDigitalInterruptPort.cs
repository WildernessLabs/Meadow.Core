using System;

namespace Meadow.Hardware
{
    public interface IDigitalInterruptPort
    {
        event EventHandler<DigitalInputPortEventArgs> Changed;

        int DebounceDuration { get; set; }
        int GlitchFilterCycleCount { get; set; }
        InterruptMode InterruptMode { get; }
    }
}
