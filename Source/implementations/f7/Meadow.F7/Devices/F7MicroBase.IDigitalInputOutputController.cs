using Meadow.Hardware;
using System;

namespace Meadow.Devices;

/// <summary>
/// Represents the base class for Meadow F7 Microcontroller.
/// </summary>
public abstract partial class F7MicroBase
{
    /// <summary>
    /// Creates a digital output port with the specified parameters.
    /// </summary>
    /// <param name="pin">The pin for the digital output port.</param>
    /// <param name="initialState">The initial state of the digital output port.</param>
    /// <param name="initialOutputType">The initial output type of the digital output port.</param>
    /// <returns>The created digital output port.</returns>
    public IDigitalOutputPort CreateDigitalOutputPort(
        IPin pin,
        bool initialState = false,
        OutputType initialOutputType = OutputType.PushPull)
    {
        return DigitalOutputPort.From(pin, this.IoController, initialState, initialOutputType);
    }

    /// <summary>
    /// Creates a digital input port with the specified pin and default resistor mode.
    /// </summary>
    /// <param name="pin">The pin for the digital input port.</param>
    /// <returns>The created digital input port.</returns>
    public IDigitalInputPort CreateDigitalInputPort(
        IPin pin)
    {
        return CreateDigitalInputPort(pin, ResistorMode.Disabled);
    }

    /// <summary>
    /// Creates a digital input port with the specified pin, interrupt mode, and resistor mode.
    /// </summary>
    /// <param name="pin">The pin for the digital input port.</param>
    /// <param name="interruptMode">The interrupt mode for the digital input port.</param>
    /// <param name="resistorMode">The resistor mode for the digital input port.</param>
    /// <returns>The created digital input port.</returns>
    public IDigitalInputPort CreateDigitalInputPort(
        IPin pin,
        InterruptMode interruptMode = InterruptMode.None,
        ResistorMode resistorMode = ResistorMode.Disabled)
    {
        return DigitalInterruptPort.From(pin, this.IoController, interruptMode, resistorMode, TimeSpan.Zero, TimeSpan.Zero);
    }

    /// <summary>
    /// Creates a digital input port with the specified pin and resistor mode.
    /// </summary>
    /// <param name="pin">The pin for the digital input port.</param>
    /// <param name="resistorMode">The resistor mode for the digital input port.</param>
    /// <returns>The created digital input port.</returns>
    public IDigitalInputPort CreateDigitalInputPort(
        IPin pin,
        ResistorMode resistorMode
        )
    {
        return DigitalInputPort.From(pin, this.IoController, resistorMode);
    }

    /// <summary>
    /// Creates a digital interrupt port with the specified pin, interrupt mode, and default resistor mode.
    /// </summary>
    /// <param name="pin">The pin for the digital interrupt port.</param>
    /// <param name="interruptMode">The interrupt mode for the digital interrupt port.</param>
    /// <param name="resistorMode">The resistor mode for the digital interrupt port.</param>
    /// <returns>The created digital interrupt port.</returns>
    public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode = ResistorMode.Disabled)
    {
        return CreateDigitalInterruptPort(pin, interruptMode, resistorMode, TimeSpan.Zero, TimeSpan.Zero);
    }

    /// <summary>
    /// Creates a digital interrupt port with the specified parameters.
    /// </summary>
    /// <param name="pin">The pin for the digital interrupt port.</param>
    /// <param name="interruptMode">The interrupt mode for the digital interrupt port.</param>
    /// <param name="resistorMode">The resistor mode for the digital interrupt port.</param>
    /// <param name="debounceDuration">The debounce duration for the digital interrupt port.</param>
    /// <param name="glitchDuration">The glitch duration for the digital interrupt port.</param>
    /// <returns>The created digital interrupt port.</returns>
    public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
    {
        return DigitalInterruptPort.From(pin, this.IoController, interruptMode, resistorMode, debounceDuration, glitchDuration);
    }

    private bool _supportsOsAnalyzer = true;

    /// <inheritdoc/>
    public virtual IDigitalSignalAnalyzer CreateDigitalSignalAnalyzer(IPin pin)
    {
        if (_supportsOsAnalyzer)
        {
            if (pin is F7Pin f7Pin)
            {
                try
                {
                    var f7Analyzer = new F7DigitalSignalAnalyzer(f7Pin);
                    return f7Analyzer;
                }
                catch (Exception ex)
                {
                    Resolver.Log.Warn($"Failed creating an F7 analyzer. ({ex.GetType().Name}) {ex.Message}");
                    Resolver.Log.Warn($"Falling back to a SoftDigitalSignalAnalyzer.");

                    _supportsOsAnalyzer = false;
                    return new SoftDigitalSignalAnalyzer(pin);
                }
            }
            else
            {
                return new SoftDigitalSignalAnalyzer(pin);
            }
        }
        else
        {
            return new SoftDigitalSignalAnalyzer(pin);
        }
    }
}
