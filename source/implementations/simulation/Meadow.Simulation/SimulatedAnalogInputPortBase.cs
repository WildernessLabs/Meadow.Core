using Meadow.Hardware;
using Meadow.Units;
using System;

namespace Meadow.Simulation;

/// <summary>
/// A base class for SimulatedAnalogInputPorts that allows for setting the Voltage
/// </summary>
public abstract class SimulatedAnalogInputPortBase : AnalogInputPortBase
{
    /// <summary>
    /// Creates a SimulatedAnalogInputPortBase
    /// </summary>
    /// <param name="pin">The simulated pin for the port</param>
    /// <param name="channel">The channel info for the port</param>
    /// <param name="sampleCount">The sample count for the port</param>
    /// <param name="sampleInterval">The sample interval for the port</param>
    /// <param name="referenceVoltage">The reference voltage for the port</param>
    protected SimulatedAnalogInputPortBase(SimulatedPin pin, IAnalogChannelInfo channel, int sampleCount, TimeSpan sampleInterval, Voltage referenceVoltage)
        : base(pin, channel, sampleCount, sampleInterval, referenceVoltage)
    {
    }

    /// <summary>
    /// Gets the port's voltage as implemented by VoltageImpl
    /// </summary>
    public sealed override Voltage Voltage // shenanigans required because C# doesn't allow `new` and `override` in the same sig
    {
        get { return VoltageImpl; }
    }

    /// <summary>
    /// Override this property to retrieve the port's voltage
    /// </summary>
    protected abstract Voltage VoltageImpl { get; }
}
