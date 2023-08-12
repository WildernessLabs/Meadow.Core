using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Meadow.Simulation;

public abstract class SimulatedAnalogInputPortBase : AnalogInputPortBase
{
    protected SimulatedAnalogInputPortBase(SimulatedPin pin, IAnalogChannelInfo channel, int sampleCount, TimeSpan sampleInterval, Voltage referenceVoltage)
        : base(pin, channel, sampleCount, sampleInterval, referenceVoltage)
    {
    }

    public sealed override Voltage Voltage // shenanigans required because C# doesn't allow `new` and `overide` in the same sig
    {
        get { return VoltageImpl; }
    }

    protected abstract Voltage VoltageImpl { get; }
}

public class SimulatedAnalogInputPort : SimulatedAnalogInputPortBase
{
    private SimulatedPin _pin;

    public SimulatedAnalogInputPort(SimulatedPin pin, IAnalogChannelInfo channel, int sampleCount, TimeSpan sampleInterval, Voltage referenceVoltage)
        : base(pin, channel, sampleCount, sampleInterval, referenceVoltage)
    {
        _pin = pin;
    }

    public new Voltage Voltage
    {
        get => _pin.Voltage;
        set => _pin.Voltage = value;
    }

    protected override Voltage VoltageImpl => Voltage;

    public override Task<Voltage> Read()
    {
        return Task.FromResult(_pin.Voltage);
    }

    public override void StartUpdating(TimeSpan? updateInterval)
    {
    }

    public override void StopUpdating()
    {
    }
}
