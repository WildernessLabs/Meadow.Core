using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meadow.Simulation;

/// <summary>
/// A simulated IO Expnader that provides digital inputs, outputs and analog inputs
/// </summary>
public class SimulatedIOExpander : IAnalogInputController, IDigitalInputOutputController
{
    private readonly SimulatedPin[] _pins;

    /// <summary>
    /// Gets a specific pin from the expander
    /// </summary>
    /// <param name="index">The 0-based index of the pin to retrieve</param>
    public IPin GetPin(int index)
    {
        return _pins[index];
    }

    /// <summary>
    /// Creates a SimulatedIOExpander with the specified number of pins
    /// </summary>
    /// <param name="pinCount">The number of pins the expander will support</param>
    public SimulatedIOExpander(int pinCount)
    {
        _pins = new SimulatedPin[pinCount];
        for (var i = 0; i < pinCount; i++)
        {
            var name = $"PIN{i}";
            _pins[i] = new SimulatedPin(this, name, i, new List<IChannelInfo>
            {
                    new DigitalChannelInfo(name, true, false, false, false, false),
                    new AnalogChannelInfo(name, 12, true, false)
            });
        }
    }

    /// <inheritdoc/>
    public IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
    {
        return new SimulatedAnalogInputPort(
            pin as SimulatedPin ?? throw new Exception("Pin must be a SimulatedPin"),
            (IAnalogChannelInfo)pin.SupportedChannels.First(c => c is IAnalogChannelInfo),
            sampleCount,
            sampleInterval,
            voltageReference);
    }

    /// <inheritdoc/>
    public IDigitalInputPort CreateDigitalInputPort(IPin pin, ResistorMode resistorMode)
    {
        return new SimulatedDigitalInputPort(
            pin as SimulatedPin ?? throw new Exception("Pin must be a SimulatedPin"),
            (IDigitalChannelInfo)pin.SupportedChannels.First(c => c is IDigitalChannelInfo));
    }

    /// <inheritdoc/>
    public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
    {
        return new SimulatedDigitalOutputPort(
            pin as SimulatedPin ?? throw new Exception("Pin must be a SimulatedPin"),
            (IDigitalChannelInfo)pin.SupportedChannels.First(c => c is IDigitalChannelInfo),
            initialState, initialOutputType);
    }
}
