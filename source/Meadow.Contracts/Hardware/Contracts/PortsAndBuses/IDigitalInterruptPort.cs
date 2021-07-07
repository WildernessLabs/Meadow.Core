using System;

namespace Meadow.Hardware
{
    public interface IDigitalInterruptPort
    {
        //TODO: should this be `Updated`?
        event EventHandler<DigitalPortResult> Changed;

        InterruptMode InterruptMode { get; }
        double DebounceDuration { get; set; }
        double GlitchDuration { get; set; }
    }
}
