using System;

namespace Meadow.Hardware
{
    public interface IDigitalInputPort : IDigitalInterruptPort, IDigitalPort, IObservable<DigitalInputPortEventArgs>
    {
        bool State { get; }
        ResistorMode Resistor { get; set;  }
    }
}