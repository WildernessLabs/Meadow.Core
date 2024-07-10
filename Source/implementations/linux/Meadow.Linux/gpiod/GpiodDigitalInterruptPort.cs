using Meadow.Hardware;
using System;
using static Meadow.Gpiod.Interop;

namespace Meadow;

/// <summary>
/// Represents a digital interrupt port for a GPIO pin controlled by the GPIO character device (gpiod).
/// </summary>
public class GpiodDigitalInterruptPort : DigitalInterruptPortBase
{
    private Gpiod Driver { get; }
    private LineInfo Line { get; }
    private int? _lastInterrupt = null;

    /// <inheritdoc/>
    public override bool State => Line.GetValue();
    /// <inheritdoc/>
    public override TimeSpan DebounceDuration { get; set; }

    internal GpiodDigitalInterruptPort(
        Gpiod driver,
        IPin pin,
        GpiodDigitalChannelInfo channel,
        InterruptMode interruptMode,
        ResistorMode resistorMode,
        TimeSpan debounceDuration,
        TimeSpan glitchDuration)
        : base(pin, channel, interruptMode)
    {
        DebounceDuration = debounceDuration;
        GlitchDuration = glitchDuration;
        Driver = driver;
        Pin = pin;

        line_request_flags flags = line_request_flags.None;

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

            InterruptMode = interruptMode;

            switch (InterruptMode)
            {
                case InterruptMode.EdgeRising:
                case InterruptMode.EdgeFalling:
                case InterruptMode.EdgeBoth:
                    Line.InterruptOccurred += OnInterruptOccurred;
                    Line.RequestInterrupts(InterruptMode, flags);
                    break;
                default:
                    Line.RequestInput(flags);
                    break;
            }
        }
        else
        {
            throw new NativeException($"Pin {pin.Name} does not support GPIOD operations");
        }
    }

    private void OnInterruptOccurred(LineInfo sender, gpiod_line_event e)
    {
        if (DebounceDuration.TotalMilliseconds > 0)
        {
            var now = Environment.TickCount;

            if (_lastInterrupt != null &&
                now - _lastInterrupt < DebounceDuration.TotalMilliseconds) { return; }

            _lastInterrupt = now;
        }

        var state = e.event_type == gpiod_event_type.GPIOD_LINE_EVENT_RISING_EDGE ? true : false;

        this.RaiseChangedAndNotify(new DigitalPortResult { New = new DigitalState(state, DateTime.UtcNow) }); // TODO: convert event time?
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

    /// <inheritdoc/>
    public override TimeSpan GlitchDuration
    {
        get => TimeSpan.Zero;
        set
        {
            if (GlitchDuration == TimeSpan.Zero) { return; }

            throw new NotSupportedException("Glitch filtering is not currently supported on this platform.");
        }
    }
}
