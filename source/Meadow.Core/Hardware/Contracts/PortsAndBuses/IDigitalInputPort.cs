using System;

namespace Meadow.Hardware
{
    public interface IDigitalInputPort : IDigitalInterruptPort, IDigitalPort, IObservable<DigitalPortResult>
    {
        bool State { get; }
        ResistorMode Resistor { get; set;  }
    }
}