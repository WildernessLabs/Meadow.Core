using Meadow.Hardware;
using Meadow.Units;

namespace Meadow.Simulation;

public class SimulatedDigitalOutputPort : DigitalOutputPortBase
{
    private SimulatedPin _pin;

    public SimulatedDigitalOutputPort(IPin pin, IDigitalChannelInfo channel, bool initialState, OutputType initialOutputType)
        : base(pin, channel, initialState, initialOutputType)
    {
        _pin = pin as SimulatedPin;
    }

    public override bool State
    {
        get => _pin.Voltage >= SimulationEnvironment.ActiveVoltage;
        set => _pin.Voltage = value ? new Voltage(SimulationEnvironment.ActiveVoltage) : new Voltage(SimulationEnvironment.InactiveVoltage);
    }
}
