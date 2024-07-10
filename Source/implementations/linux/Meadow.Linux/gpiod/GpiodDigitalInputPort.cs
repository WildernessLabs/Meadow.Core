using Meadow.Hardware;
using System;
using static Meadow.Gpiod.Interop;

namespace Meadow;

/// <summary>
/// Represents a digital input port for a GPIO pin controlled by the GPIO character device (gpiod).
/// </summary>
public class GpiodDigitalInputPort : DigitalInputPortBase
{
    private Gpiod Driver { get; }
    private LineInfo Line { get; }

    /// <inheritdoc/>
    public override bool State => Line.GetValue();

    internal GpiodDigitalInputPort(
        Gpiod driver,
        IPin pin,
        GpiodDigitalChannelInfo channel,
        ResistorMode resistorMode)
        : base(pin, channel)
    {
        Driver = driver;
        Pin = pin;
        LineInfo? li = null;

        if (pin is GpiodPin { } gp)
        {
            li = Driver.GetLine(gp);
        }
        else if (pin is LinuxFlexiPin { } lp)
        {
            li = Driver.GetLine(lp);
        }

        if (li != null)
        {
            Line = li;

            line_request_flags flags;

            switch (resistorMode)
            {
                case ResistorMode.InternalPullUp:
                    flags = line_request_flags.GPIOD_LINE_REQUEST_FLAG_BIAS_PULL_UP;
                    break;
                case ResistorMode.InternalPullDown:
                    flags = line_request_flags.GPIOD_LINE_REQUEST_FLAG_BIAS_PULL_DOWN;
                    break;
                default:
                    flags = line_request_flags.GPIOD_LINE_REQUEST_FLAG_BIAS_DISABLE;
                    break;
            }

            Line.RequestInput(flags);

        }
        else
        {
            throw new NativeException($"Pin {pin.Name} does not support GPIOD operations");
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        Line.Release();
        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    public override ResistorMode Resistor
    {
        get => ResistorMode.Disabled;
        set => throw new NotSupportedException("Resistor Mode not supported on this platform");
    }
}
