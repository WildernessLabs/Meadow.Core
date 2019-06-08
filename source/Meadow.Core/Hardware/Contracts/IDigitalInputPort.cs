using System;

namespace Meadow.Hardware
{
    public interface IDigitalInputPort : IDigitalPort, IObservable<DigitalInputPortEventArgs>
    {
        bool State { get; }
        ResistorMode Resistor { get; set;  }
    }
}