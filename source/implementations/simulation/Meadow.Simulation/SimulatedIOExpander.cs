using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meadow.Simulation;

public class SimulatedIOExpander : IAnalogInputController, IDigitalInputOutputController
{
    private readonly SimulatedPin[] _pins;

    public IPin GetPin(int index)
    {
        return _pins[index];
    }

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

    public IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
    {
        return new SimulatedAnalogInputPort(
            pin as SimulatedPin ?? throw new Exception("Pin must be a SimulatedPin"),
            (IAnalogChannelInfo)pin.SupportedChannels.First(c => c is IAnalogChannelInfo),
            sampleCount,
            sampleInterval,
            voltageReference);
    }

    public IDigitalInputPort CreateDigitalInputPort(IPin pin, ResistorMode resistorMode)
    {
        return new SimulatedDigitalInputPort(
            pin as SimulatedPin ?? throw new Exception("Pin must be a SimulatedPin"),
            (IDigitalChannelInfo)pin.SupportedChannels.First(c => c is IDigitalChannelInfo));
    }

    public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
    {
        return new SimulatedDigitalOutputPort(
            pin as SimulatedPin ?? throw new Exception("Pin must be a SimulatedPin"),
            (IDigitalChannelInfo)pin.SupportedChannels.First(c => c is IDigitalChannelInfo),
            initialState, initialOutputType);
    }
}
