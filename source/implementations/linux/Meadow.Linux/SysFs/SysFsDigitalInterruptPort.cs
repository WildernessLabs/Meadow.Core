using Meadow.Hardware;
using System;
using System.Threading;

namespace Meadow;

/// <summary>
/// Represents a digital interrupt port for SysFs-based GPIO operations.
/// </summary>
public class SysFsDigitalInterruptPort : DigitalInterruptPortBase, IDigitalInputPort
{
    private int Gpio { get; set; } = -1;
    private SysFsGpioDriver Driver { get; }
    private ResistorMode _resistorMode = ResistorMode.Disabled;
    private int? _lastInterrupt = null;
    private InterruptMode _interruptMode;

    /// <inheritdoc/>
    public override bool State => Driver.GetValue(Gpio);
    /// <inheritdoc/>
    public override TimeSpan DebounceDuration { get; set; }

    internal SysFsDigitalInterruptPort(
        SysFsGpioDriver driver,
        IPin pin,
        SysFsDigitalChannelInfo channel,
        InterruptMode interruptMode,
        ResistorMode resistorMode,
        TimeSpan debounceDuration,
        TimeSpan glitchDuration)
        : base(pin, channel)
    {
        Resistor = resistorMode;
        DebounceDuration = debounceDuration;
        GlitchDuration = glitchDuration;
        Driver = driver;
        Pin = pin;
        if (pin is SysFsPin { } sp)
        {
            Gpio = sp.Gpio;
        }
        else if (pin is LinuxFlexiPin { } l)
        {
            Gpio = l.SysFsGpio;
        }
        else
        {
            throw new NativeException($"Pin {pin.Name} does not support SYS FS GPIO operations");
        }

        Driver.Export(Gpio);
        Thread.Sleep(100); // this seems to be required to prevent an error 13
        Driver.SetDirection(Gpio, SysFsGpioDriver.GpioDirection.Input);
        InterruptMode = interruptMode;
    }

    /// <inheritdoc/>
    public override InterruptMode InterruptMode
    {
        get => _interruptMode;
        set
        {
            _interruptMode = value;
            switch (_interruptMode)
            {
                case InterruptMode.None:
                    // nothing to do
                    break;
                default:
                    Driver.HookInterrupt(Gpio, _interruptMode, InterruptCallback);
                    break;
            }
        }
    }

    private void InterruptCallback()
    {
        // TODO: implement old/new

        if (DebounceDuration.TotalMilliseconds > 0)
        {
            var now = Environment.TickCount;

            if (_lastInterrupt != null &&
                now - _lastInterrupt < DebounceDuration.TotalMilliseconds) { return; }

            _lastInterrupt = now;
        }

        RaiseChangedAndNotify(new DigitalPortResult());
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (Gpio >= 0)
        {
            Driver.UnhookInterrupt(Gpio);
            Driver.Unexport(Gpio);
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    public override ResistorMode Resistor
    {
        get => _resistorMode;
        set
        {
            switch (value)
            {
                case ResistorMode.InternalPullUp:
                case ResistorMode.InternalPullDown:
                    throw new NotSupportedException("Internal Resistor Modes are not supported on the current OS");
            }
        }
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
