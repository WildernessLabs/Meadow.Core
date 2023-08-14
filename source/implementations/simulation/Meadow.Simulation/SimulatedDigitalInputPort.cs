using Meadow.Hardware;
using Meadow.Units;

namespace Meadow.Simulation;

public abstract class SimulatedDigitalInputPortBase : DigitalInputPortBase
{
    protected SimulatedDigitalInputPortBase(SimulatedPin pin, IDigitalChannelInfo channel)
        : base(pin, channel)
    {
    }

    public sealed override bool State // shenanigans required because C# doesn't allow `new` and `overide` in the same sig
    {
        get { return StateImpl; }
    }

    protected abstract bool StateImpl { get; }
}

public class SimulatedDigitalInputPort : SimulatedDigitalInputPortBase
{
    private SimulatedPin SimPin => Pin as SimulatedPin ?? throw new System.Exception("Pin is no a SimulatedPin");

    public SimulatedDigitalInputPort(SimulatedPin pin, IDigitalChannelInfo channel)
        : base(pin, channel)
    {
    }

    internal void SetVoltage(Voltage voltage)
    {
        if (voltage == SimPin.Voltage) return;

        SimPin.Voltage = voltage;
    }

    public new bool State
    {
        get => SimPin.Voltage >= SimulationEnvironment.ActiveVoltage;
        set => SimPin.Voltage = value ? new Voltage(SimulationEnvironment.ActiveVoltage) : new Voltage(SimulationEnvironment.InactiveVoltage);
    }

    protected override bool StateImpl
    {
        get => State;
    }

    public override ResistorMode Resistor { get; set; }
}
