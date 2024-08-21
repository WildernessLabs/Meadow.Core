using Meadow.Hardware;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Meadow.Gpiod.Interop;

namespace Meadow;

internal delegate void LineEventHandler(LineInfo lineInfo, gpiod_line_event evt);

internal class LineInfo
{
    private IntPtr Handle { get; set; }
    private gpiod_line? Line { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string Consumer { get; private set; } = string.Empty;
    public int Offset { get; }
    public line_direction Direction { get; private set; }
    public line_active_state ActiveState { get; private set; }

    public bool IsInvalid => Handle.ToInt64() <= 0;

    private bool _istIsRunning = false;
    private bool _istShouldStop = false;
    public event LineEventHandler InterruptOccurred = delegate { };

    public LineInfo(ChipInfo chip, int offset)
    {
        Offset = offset;
        Handle = gpiod_chip_get_line(chip.Handle, Offset);

        if (!IsInvalid)
        {
            UpdatePropsFromHandle();
        }
    }

    private void UpdatePropsFromHandle()
    {
        Line = Marshal.PtrToStructure<gpiod_line>(Handle);
        Name = Line.Value.name;
        Consumer = Line.Value.consumer;
        Direction = Line.Value.direction;
        ActiveState = Line.Value.active_state;
    }

    public void Update()
    {
        gpiod_line_update(Handle);
        UpdatePropsFromHandle();
    }

    private const string MeadowConsumer = "Meadow";

    public void Request(line_direction direction)
    {
        // TODO: check for free?
        int result;

        if (direction == line_direction.GPIOD_LINE_DIRECTION_INPUT)
        {
            result = gpiod_line_request_input(Handle, MeadowConsumer);
        }
        else
        {
            result = gpiod_line_request_output(Handle, MeadowConsumer, 0);
        }

        if (result == -1)
        {
            var errorCode = Marshal.GetLastWin32Error();
            if (errorCode == 16)
            {
                // resource is busy - we might have crashed leaving the driver with the pin locked
                // or some other app, etc might be using it
                throw new NativeException($"Unable to access output pin.  Pin {Line?.name} is in use", errorCode);
            }
            else
            {
                throw new NativeException($"Unable to access output pin.  Error code {errorCode}", errorCode);
            }
        }
    }

    public void RequestInput(line_request_flags flags)
    {
        // TODO: check for free?
        var result = gpiod_line_request_input_flags(Handle, MeadowConsumer, flags);

        if (result == -1)
        {
            var err = Marshal.GetLastWin32Error();
            throw new NativeException("Failed to request line", Marshal.GetLastWin32Error());
        }
    }

    public bool RequestOutput(line_request_flags flags, bool initialState)
    {
        // TODO: check for free?
        var result = gpiod_line_request_output_flags(Handle, MeadowConsumer, flags, initialState ? 1 : 0);

        return result != -1;
    }

    public void RequestInterrupts(InterruptMode mode, line_request_flags flags)
    {
        if (_istIsRunning) return;

        _istShouldStop = false;

        int result;

        switch (mode)
        {
            case InterruptMode.EdgeRising:
                result = gpiod_line_request_rising_edge_events_flags(Handle, MeadowConsumer, flags);
                break;
            case InterruptMode.EdgeFalling:
                result = gpiod_line_request_falling_edge_events(Handle, MeadowConsumer);
                break;
            case InterruptMode.EdgeBoth:
                result = gpiod_line_request_both_edges_events(Handle, MeadowConsumer);
                break;
            default:
                return;
        }

        if (result == -1)
        {
            throw new NativeException($"Failed to request interrupts: {Marshal.GetLastWin32Error()}", Marshal.GetLastWin32Error());
        }
        else
        {
            Task.Run(() => IST());
        }
    }

    private void IST()
    {
        _istIsRunning = true;
        timespec timeout = new timespec { tv_sec = 1 }; // 1-second timeout
        gpiod_line_event evnt = new gpiod_line_event();

        while (!_istShouldStop)
        {
            var result = gpiod_line_event_wait(Handle, ref timeout);

            switch (result)
            {
                case 0: // timeout
                    // nop, just wait again
                    break;
                case 1: // event
                    result = gpiod_line_event_read(Handle, ref evnt);

                    if (result == 0)
                    {
                        InterruptOccurred?.Invoke(this, evnt);
                    }
                    else
                    {
                        throw new NativeException("Failed to read interrupt data", Marshal.GetLastWin32Error());
                    }
                    break;
                case -1: // error
                default: //undefined
                    throw new NativeException("Waiting for interrupt event failed", Marshal.GetLastWin32Error());
            }

        }

        _istIsRunning = false;
    }

    public void Release()
    {
        _istShouldStop = true;
        gpiod_line_release(Handle);
    }

    public void SetValue(bool state)
    {
        var result = gpiod_line_set_value(Handle, state ? 1 : 0);

        if (result == -1)
        {
            throw new NativeException("Failed to set line value", Marshal.GetLastWin32Error());
        }
    }

    public bool GetValue()
    {
        var result = gpiod_line_get_value(Handle);

        if (result == -1)
        {
            throw new NativeException("Failed to set line value", Marshal.GetLastWin32Error());
        }

        return result == 1;
    }

    public override string ToString()
    {
        // same format as gpioinfo
        // gpiochip0 [pinctrl-bcm2711] (58 lines)
        var c = string.IsNullOrEmpty(Consumer) ? "unused" : $"\"{Consumer}\"";
        var n = $"\"{Name}\"";
        var d = Direction == line_direction.GPIOD_LINE_DIRECTION_INPUT ? " input" : "output";
        var s = ActiveState == line_active_state.GPIOD_LINE_ACTIVE_STATE_LOW ? "active-low" : "active-high";

        return $"line {Offset:00}: {n,24}{c,24}  {d}  {s}";
    }
}
