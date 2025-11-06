using Meadow.Hardware;
using System;

namespace Meadow;

/// <summary>
/// Represents a digital input port for a GPIO pin controlled by the GPIO character device (gpiod).
/// </summary>
public class GpiodDigitalInputPort : DigitalInputPortBase
{
    private Gpiod Driver { get; }
    private ILineInfo Line { get; }

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
        ILineInfo? li = null;

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

            GpiodLineBias bias;

            switch (resistorMode)
            {
                case ResistorMode.InternalPullUp:
                    bias = GpiodLineBias.PullUp;
                    break;
                case ResistorMode.InternalPullDown:
                    bias = GpiodLineBias.PullDown;
                    break;
                default:
                    bias = GpiodLineBias.Disabled;
                    break;
            }

            Line.RequestInput(bias);
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
