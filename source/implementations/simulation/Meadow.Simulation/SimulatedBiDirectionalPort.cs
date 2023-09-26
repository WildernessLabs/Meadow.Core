using Meadow.Hardware;
using System;

namespace Meadow.Simulation;

internal class SimulatedBiDirectionalPort : BiDirectionalPortBase
{
    private SimulatedPin _pin;

    public SimulatedBiDirectionalPort(IPin pin, IDigitalChannelInfo channel, bool initialState, ResistorMode resistorMode, PortDirectionType initialDirection, OutputType initialOutputType = OutputType.PushPull)
        : base(pin, channel, initialState, resistorMode, initialDirection, initialOutputType)
    {
        _pin = pin as SimulatedPin;
        Direction = initialDirection;

        if (initialState)
        {
            State = InitialState;
        }
    }

    public override bool State
    {
        get => _pin.Voltage >= SimulationEnvironment.ActiveVoltage;
        set
        {
            if (Direction == PortDirectionType.Input) throw new Exception("Port currently set as Input");
            _pin.Voltage = value ? SimulationEnvironment.ActiveVoltage : SimulationEnvironment.InactiveVoltage;
        }
    }

    public override PortDirectionType Direction { get; set; }

    protected override void Dispose(bool disposing)
    {
    }
}
