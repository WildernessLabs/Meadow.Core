using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Meadow.Simulation;

/// <summary>
/// A simulated IAnalogInputPort
/// </summary>
public class SimulatedAnalogInputPort : SimulatedAnalogInputPortBase
{
    private readonly SimulatedPin _pin;

    /// <summary>
    /// Creates a SimulatedAnalogInputPort instance
    /// </summary>
    /// <param name="pin">The simulated pin for the port</param>
    /// <param name="channel">The channel info for the port</param>
    /// <param name="referenceVoltage">The reference voltage for the port</param>
    public SimulatedAnalogInputPort(SimulatedPin pin, IAnalogChannelInfo channel, Voltage? referenceVoltage)
        : base(pin, channel, referenceVoltage)
    {
        _pin = pin;
    }

    /// <summary>
    /// Gets or sets the port's voltage
    /// </summary>
    public new Voltage Voltage
    {
        get => _pin.Voltage;
        set => _pin.Voltage = value;
    }

    /// <inheritdoc/>
    protected override Voltage VoltageImpl => Voltage;

    /// <inheritdoc/>
    public override Task<Voltage> Read()
    {
        return Task.FromResult(_pin.Voltage);
    }

    /// <inheritdoc/>
    public override void StartUpdating(TimeSpan? updateInterval)
    {
    }

    /// <inheritdoc/>
    public override void StopUpdating()
    {
    }
}
