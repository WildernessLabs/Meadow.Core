using Meadow.Hardware;
using System;
using System.Threading;

namespace Meadow;

/// <summary>
/// Represents a digital output port for SysFs-based GPIO operations.
/// </summary>
public class SysFsDigitalOutputPort : IDigitalOutputPort
{
    /// <inheritdoc/>
    public bool InitialState { get; private set; }
    /// <inheritdoc/>
    public IPin Pin { get; private set; }
    private bool LastState { get; set; }
    private int Gpio { get; set; } = -1;
    private SysFsGpioDriver Driver { get; }

    /// <inheritdoc/>
    public IDigitalChannelInfo Channel => throw new NotImplementedException(); // TODO

    internal SysFsDigitalOutputPort(SysFsGpioDriver driver, IPin pin, bool initialState)
    {
        Driver = driver;
        Pin = pin;
        InitialState = initialState;
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

        try
        {
            Initialize();
        }
        catch (DeviceBusyException)
        {
            //if the device is busy, it might be that our app tore down before and never Unexported.  Try to unexport and retry
            Driver.Unexport(Gpio);
            Thread.Sleep(500);
            Initialize();
        }
    }

    private void Initialize()
    {
        Driver.Export(Gpio);

        // wait for the sysfs driver to generate the GPIO folder.  If we don't we'll get an error 13
        // TODO: actually look at the filesystem rather than a hard-coded delay
        Thread.Sleep(500);

        // this may throw if the driver is already open
        Driver.SetDirection(Gpio, SysFsGpioDriver.GpioDirection.Output);

        State = InitialState;
    }

    /// <inheritdoc/>
    public bool State
    {
        get => LastState;
        set
        {
            Driver.SetValue(Gpio, value);
            LastState = value;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (Gpio >= 0)
        {
            Driver.Unexport(Gpio);
        }
    }
}
