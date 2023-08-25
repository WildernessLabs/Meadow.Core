using Meadow.Hardware;
using Meadow.Units;

namespace Meadow.Simulation;

internal class SimulatedDigitalInputPort : SimulatedDigitalInputPortBase
{
    private SimulatedPin SimPin => Pin as SimulatedPin ?? throw new System.Exception("Pin is not a SimulatedPin");

    public SimulatedDigitalInputPort(SimulatedPin pin, IDigitalChannelInfo channel)
        : base(pin, channel)
    {
    }

    internal void SetVoltage(Voltage voltage)
    {
        if (voltage == SimPin.Voltage) return;

        SimPin.Voltage = voltage;
    }

    protected override bool StateImpl => State;

    public new bool State
    {
        get => SimPin.Voltage >= SimulationEnvironment.ActiveVoltage;
        set => SimPin.Voltage = value ? SimulationEnvironment.ActiveVoltage : SimulationEnvironment.InactiveVoltage;
    }

    public override ResistorMode Resistor { get; set; }
}
