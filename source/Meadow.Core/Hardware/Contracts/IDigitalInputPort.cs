using System;

namespace Meadow.Hardware
{
    public interface IDigitalInputPort : IDigitalPort
    {
        bool State { get; }

        int DebounceDuration { get; set; }
        int GlitchFilterCycleCount { get; set; }

        event EventHandler<PortEventArgs> Changed;
    }
}