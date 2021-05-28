using System;

namespace Meadow.Hardware
{
    // TODO: is the interface inheritance backwards here? shouldn't `IDigitalInterruptPort`
    // inherit from `IDigitalInputPort`?
    /// <summary>
    /// Contract for ports that are capable of reading digital input and raising
    /// events when state changes.
    /// </summary>
    public interface IDigitalInputPort : IDigitalInterruptPort, IDigitalPort, IObservable<IChangeResult<DigitalState>>
    {
        bool State { get; }
        ResistorMode Resistor { get; set;  }


        public static FilterableChangeObserver<DigitalState>
            CreateObserver(
                Action<IChangeResult<DigitalState>> handler,
                Predicate<IChangeResult<DigitalState>>? filter = null)
        {
            return new FilterableChangeObserver<DigitalState>(
                handler, filter);
        }

    }
}