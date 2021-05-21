using System;

namespace Meadow.Hardware
{
    // TODO: is the interface inheritance backwards here? shouldn't `IDigitalInterruptPort`
    // inherit from `IDigitalInputPort`?
    /// <summary>
    /// Contract for ports that are capable of reading digital input and raising
    /// events when state changes.
    /// </summary>
    public interface IDigitalInputPort : IDigitalInterruptPort, IDigitalPort, IObservable<DigitalPortResult>
    {
        bool State { get; }
        ResistorMode Resistor { get; set;  }
    }
}