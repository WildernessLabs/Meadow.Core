using Meadow.Hardware;
using System;
using System.Runtime.InteropServices;

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
        }
        else if (pin is LinuxFlexiPin { } fp)
        {
            Line = Driver.GetLine(fp);
        }
        else
        {
            throw new NativeException($"Pin {pin.Name} does not support GPIOD operations");
        }

        var result = Line.RequestOutput(Gpiod.Interop.line_request_flags.GPIOD_LINE_REQUEST_FLAG_BIAS_DISABLE, initialState);
        if (!result)
        {
            var err = Marshal.GetLastWin32Error();

            if ((Interop.Errors)err == Interop.Errors.DeviceBusy)
            {
                throw new NativeException($"Pin {pin.Name} already in use");
            }

            throw new NativeException($"Failed to request line for Pin {pin.Name} (error {err})", err);
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
