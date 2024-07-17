using Meadow.Hardware;
using System;
using System.Threading;

namespace Meadow;

/// <summary>
/// Represents a digital input port for SysFs-based GPIO operations.
/// </summary>
public class SysFsDigitalInputPort : DigitalInputPortBase, IDigitalInputPort
{
    private int Gpio { get; set; } = -1;
    private SysFsGpioDriver Driver { get; }
    private ResistorMode _resistorMode = ResistorMode.Disabled;

    /// <inheritdoc/>
    public override bool State => Driver.GetValue(Gpio);

    internal SysFsDigitalInputPort(
        SysFsGpioDriver driver,
        IPin pin,
        SysFsDigitalChannelInfo channel,
        ResistorMode resistorMode)
        : base(pin, channel)
    {
        switch (resistorMode)
        {
            case ResistorMode.InternalPullUp:
            case ResistorMode.InternalPullDown:
                throw new NotSupportedException("Internal Resistor Modes are not supported on the current OS");
        }

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
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (Gpio >= 0)
        {
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
}
