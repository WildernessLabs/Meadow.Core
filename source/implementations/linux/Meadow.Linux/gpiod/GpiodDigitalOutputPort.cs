using Meadow.Hardware;
using System;

namespace Meadow;

/// <summary>
/// Represents a digital output port for a GPIO pin controlled by the GPIO character device (gpiod).
/// </summary>
public class GpiodDigitalOutputPort : IDigitalOutputPort
{
    /// <inheritdoc/>
    public bool InitialState { get; private set; }
    /// <inheritdoc/>
    public IPin Pin { get; private set; }
    private bool LastState { get; set; }

    private Gpiod Driver { get; }
    private LineInfo Line { get; }


    /// <inheritdoc/>
    public IDigitalChannelInfo Channel => throw new NotImplementedException(); // TODO

    internal GpiodDigitalOutputPort(Gpiod driver, IPin pin, bool initialState)
    {
        Driver = driver;
        Pin = pin;
        InitialState = initialState;

        if (pin is GpiodPin { } gp)
        {
            Line = Driver.GetLine(gp);
            Line.Request(Gpiod.Interop.line_direction.GPIOD_LINE_DIRECTION_OUTPUT);
        }
        else if (pin is LinuxFlexiPin { } fp)
        {
            Line = Driver.GetLine(fp);
            Line.Request(Gpiod.Interop.line_direction.GPIOD_LINE_DIRECTION_OUTPUT);
        }
        else
        {
            throw new NativeException($"Pin {pin.Name} does not support GPIOD operations");
        }

    }

    /// <inheritdoc/>
    public bool State
    {
        get => LastState;
        set
        {
            Line.SetValue(value);

            LastState = value;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Line.Release();
    }
}
