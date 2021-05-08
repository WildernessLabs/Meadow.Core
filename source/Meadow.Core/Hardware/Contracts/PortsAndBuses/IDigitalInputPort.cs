using System;

namespace Meadow.Hardware
{
    public interface IDigitalInputPort : IDigitalInterruptPort, IDigitalPort, IObservable<DigitalInputPortChangeResult>
    {
        bool State { get; }
        ResistorMode Resistor { get; set;  }
    }
}