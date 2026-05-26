using Meadow.Hardware;
using System;
using static Meadow.Gpiod2.Interop;

namespace Meadow;

/// <summary>
/// Represents a digital interrupt port for a GPIO pin controlled by the GPIO character device (gpiod).
/// </summary>
public class GpiodDigitalInterruptPort : DigitalInterruptPortBase
{
    private Gpiod Driver { get; }
    private ILineInfo Line { get; }
    private int _lastHighTime = 0;
    private int _lastLowTime = 0;
    private InterruptMode _interruptMode;
    private ResistorMode _resistorMode;

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
        : base(pin, channel)
    {
        DebounceDuration = debounceDuration;
        GlitchDuration = glitchDuration;
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

            SetInterruptOrInputMode(resistorMode, interruptMode);
        }
        else
        {
            throw new NativeException($"Pin {pin.Name} does not support GPIOD operations");
        }
    }

    private void SetInterruptOrInputMode(ResistorMode resistorMode, InterruptMode interruptMode)
    {
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

        switch (interruptMode)
        {
            case InterruptMode.EdgeRising:
            case InterruptMode.EdgeFalling:
            case InterruptMode.EdgeBoth:
                Line.InterruptOccurred += OnInterruptOccurred;
                Line.RequestInterrupts(interruptMode, bias);
                break;
            default:
                Line.RequestInput(bias);
                break;
        }

        _interruptMode = interruptMode;
        _resistorMode = resistorMode;
    }

    private void OnInterruptOccurred(ILineInfo sender, GpiodEdgeEventArgs e)
    {
        // Read the actual pin state rather than trusting e.EventType, which can
        // misreport on some kernel/gpiod versions.
        var state = Line.GetValue();

        if (DebounceDuration.TotalMilliseconds > 0)
        {
            var now = Environment.TickCount;
            var debounceMs = (int)DebounceDuration.TotalMilliseconds;

            // Filter same-direction repeats within the debounce window.
            // Direction changes (press→release or release→press) are always passed
            // through so a fast click never loses its release edge.
            ref var lastTime = ref (state ? ref _lastHighTime : ref _lastLowTime);
            if (now - lastTime < debounceMs) { return; }
            lastTime = now;
        }

        this.RaiseChangedAndNotify(new DigitalPortResult { New = new DigitalState(state, DateTime.UtcNow) });
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        Line.Release();
        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    public override InterruptMode InterruptMode
    {
        get => _interruptMode;
        set => SetInterruptOrInputMode(Resistor, value);
    }

    /// <inheritdoc/>
    public override ResistorMode Resistor
    {
        get => _resistorMode;
        set => SetInterruptOrInputMode(value, InterruptMode);
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
