using Meadow.Hardware;

namespace Meadow.Simulation;

/// <summary>
/// A base class for SimulatedDigitalInputPorts that allows for setting the State
/// </summary>
public abstract class SimulatedDigitalInputPortBase : DigitalInputPortBase
{
    protected SimulatedDigitalInputPortBase(SimulatedPin pin, IDigitalChannelInfo channel)
        : base(pin, channel)
    {
    }

    /// <summary>
    /// Gets the port's state as implemented by StateImpl
    /// </summary>
    public sealed override bool State // shenanigans required because C# doesn't allow `new` and `overide` in the same sig
    {
        get { return StateImpl; }
    }

    /// <summary>
    /// Override this property to retrieve the port's state
    /// </summary>
    protected abstract bool StateImpl { get; }
}
