using Meadow.Hardware;
using System;
using System.Runtime.InteropServices;

namespace Meadow;

/// <summary>
/// LineInfo for libgpiod v3 (API v2.x)
/// V3 uses line requests with config objects instead of direct line manipulation
/// </summary>
internal class LineInfo3 : ILineInfo
{
    private readonly ChipInfo3 _chip;
    private IntPtr _infoHandle;
    private IntPtr _requestHandle;

    public string Name { get; private set; } = string.Empty;
    public string Consumer { get; private set; } = string.Empty;
    public int Offset { get; }
    public Gpiod3.Interop.gpiod_line_direction Direction { get; private set; }
    public bool IsUsed { get; private set; }
    public bool IsInvalid => _infoHandle == IntPtr.Zero;

    public LineInfo3(ChipInfo3 chip, int offset)
    {
        _chip = chip;
        Offset = offset;

        RefreshInfo();
    }

    private void RefreshInfo()
    {
        if (_chip.Handle == IntPtr.Zero) return;

        // Free previous info if exists
        if (_infoHandle != IntPtr.Zero)
        {
            Gpiod3.Interop.gpiod_line_info_free(_infoHandle);
        }

        _infoHandle = Gpiod3.Interop.gpiod_chip_get_line_info(_chip.Handle, (uint)Offset);

        if (_infoHandle != IntPtr.Zero)
        {
            // Get name
            var namePtr = Gpiod3.Interop.gpiod_line_info_get_name(_infoHandle);
            Name = namePtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(namePtr) ?? string.Empty : string.Empty;

            // Get consumer
            var consumerPtr = Gpiod3.Interop.gpiod_line_info_get_consumer(_infoHandle);
            Consumer = consumerPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(consumerPtr) ?? string.Empty : string.Empty;

            // Get direction
            Direction = Gpiod3.Interop.gpiod_line_info_get_direction(_infoHandle);

            // Check if used
            IsUsed = Gpiod3.Interop.gpiod_line_info_is_used(_infoHandle);
        }
    }

    // ILineInfo implementation
    bool ILineInfo.RequestOutput(GpiodLineBias bias, bool initialState)
    {
        try
        {
            RequestOutput(bias.AsGpiod3LineBias(), initialState);
            return true;
        }
        catch
        {
            return false;
        }
    }

    void ILineInfo.RequestInput(GpiodLineBias bias)
    {
        Request(
            Gpiod3.Interop.gpiod_line_direction.GPIOD_LINE_DIRECTION_INPUT,
            bias.AsGpiod3LineBias());
    }

    void ILineInfo.RequestInterrupts(InterruptMode mode, GpiodLineBias bias)
    {
        RequestInterrupts(mode, bias.AsGpiod3LineBias());
    }

    /// <summary>
    /// Request this line for input or output
    /// </summary>
    public void Request(Gpiod3.Interop.gpiod_line_direction direction, Gpiod3.Interop.gpiod_line_bias bias = Gpiod3.Interop.gpiod_line_bias.GPIOD_LINE_BIAS_AS_IS)
    {
        if (_requestHandle != IntPtr.Zero)
        {
            throw new InvalidOperationException("Line already requested");
        }

        // Create request config
        var reqConfig = Gpiod3.Interop.gpiod_request_config_new();
        if (reqConfig == IntPtr.Zero)
        {
            throw new NativeException("Failed to create request config");
        }

        try
        {
            Gpiod3.Interop.gpiod_request_config_set_consumer(reqConfig, "Meadow");

            // Create line settings
            var settings = Gpiod3.Interop.gpiod_line_settings_new();
            if (settings == IntPtr.Zero)
            {
                throw new NativeException("Failed to create line settings");
            }

            try
            {
                // Set direction
                if (Gpiod3.Interop.gpiod_line_settings_set_direction(settings, direction) < 0)
                {
                    throw new NativeException("Failed to set direction");
                }

                // Set bias if specified
                if (bias != Gpiod3.Interop.gpiod_line_bias.GPIOD_LINE_BIAS_AS_IS)
                {
                    Gpiod3.Interop.gpiod_line_settings_set_bias(settings, bias);
                }

                // Create line config
                var lineConfig = Gpiod3.Interop.gpiod_line_config_new();
                if (lineConfig == IntPtr.Zero)
                {
                    throw new NativeException("Failed to create line config");
                }

                try
                {
                    // Add this line to the config
                    uint[] offsets = { (uint)Offset };
                    if (Gpiod3.Interop.gpiod_line_config_add_line_settings(lineConfig, offsets, (UIntPtr)1, settings) < 0)
                    {
                        throw new NativeException("Failed to add line settings");
                    }

                    // Request the line
                    _requestHandle = Gpiod3.Interop.gpiod_chip_request_lines(_chip.Handle, reqConfig, lineConfig);
                    if (_requestHandle == IntPtr.Zero)
                    {
                        var errorCode = Marshal.GetLastWin32Error();
                        if (errorCode == 16) // EBUSY
                        {
                            throw new NativeException($"Pin {Offset} ({Name}) is already in use", errorCode);
                        }
                        throw new NativeException($"Failed to request line {Offset}", errorCode);
                    }
                }
                finally
                {
                    Gpiod3.Interop.gpiod_line_config_free(lineConfig);
                }
            }
            finally
            {
                Gpiod3.Interop.gpiod_line_settings_free(settings);
            }
        }
        finally
        {
            Gpiod3.Interop.gpiod_request_config_free(reqConfig);
        }

        RefreshInfo();
    }

    /// <summary>
    /// Request this line for output with specific bias and initial value
    /// </summary>
    public void RequestOutput(Gpiod3.Interop.gpiod_line_bias bias, bool initialState)
    {
        if (_requestHandle != IntPtr.Zero)
        {
            throw new InvalidOperationException("Line already requested");
        }

        var reqConfig = Gpiod3.Interop.gpiod_request_config_new();
        if (reqConfig == IntPtr.Zero)
        {
            throw new NativeException("Failed to create request config");
        }

        try
        {
            Gpiod3.Interop.gpiod_request_config_set_consumer(reqConfig, "Meadow");

            var settings = Gpiod3.Interop.gpiod_line_settings_new();
            if (settings == IntPtr.Zero)
            {
                throw new NativeException("Failed to create line settings");
            }

            try
            {
                // Set as output
                Gpiod3.Interop.gpiod_line_settings_set_direction(settings, Gpiod3.Interop.gpiod_line_direction.GPIOD_LINE_DIRECTION_OUTPUT);

                // Set initial value
                var value = initialState ? Gpiod3.Interop.gpiod_line_value.GPIOD_LINE_VALUE_ACTIVE : Gpiod3.Interop.gpiod_line_value.GPIOD_LINE_VALUE_INACTIVE;
                Gpiod3.Interop.gpiod_line_settings_set_output_value(settings, value);

                // Set bias
                if (bias != Gpiod3.Interop.gpiod_line_bias.GPIOD_LINE_BIAS_AS_IS)
                {
                    Gpiod3.Interop.gpiod_line_settings_set_bias(settings, bias);
                }

                var lineConfig = Gpiod3.Interop.gpiod_line_config_new();
                if (lineConfig == IntPtr.Zero)
                {
                    throw new NativeException("Failed to create line config");
                }

                try
                {
                    uint[] offsets = { (uint)Offset };
                    if (Gpiod3.Interop.gpiod_line_config_add_line_settings(lineConfig, offsets, (UIntPtr)1, settings) < 0)
                    {
                        throw new NativeException("Failed to add line settings");
                    }

                    _requestHandle = Gpiod3.Interop.gpiod_chip_request_lines(_chip.Handle, reqConfig, lineConfig);
                    if (_requestHandle == IntPtr.Zero)
                    {
                        var errorCode = Marshal.GetLastWin32Error();
                        if (errorCode == 16) // EBUSY
                        {
                            throw new NativeException($"Pin {Offset} ({Name}) is already in use", errorCode);
                        }
                        throw new NativeException($"Failed to request line {Offset} for output", errorCode);
                    }
                }
                finally
                {
                    Gpiod3.Interop.gpiod_line_config_free(lineConfig);
                }
            }
            finally
            {
                Gpiod3.Interop.gpiod_line_settings_free(settings);
            }
        }
        finally
        {
            Gpiod3.Interop.gpiod_request_config_free(reqConfig);
        }

        RefreshInfo();
    }

    /// <summary>
    /// Request this line for input with edge detection for interrupts
    /// </summary>
    public void RequestInterrupts(InterruptMode mode, Gpiod3.Interop.gpiod_line_bias bias = Gpiod3.Interop.gpiod_line_bias.GPIOD_LINE_BIAS_AS_IS)
    {
        if (_requestHandle != IntPtr.Zero)
        {
            throw new InvalidOperationException("Line already requested");
        }

        var reqConfig = Gpiod3.Interop.gpiod_request_config_new();
        if (reqConfig == IntPtr.Zero)
        {
            throw new NativeException("Failed to create request config");
        }

        try
        {
            Gpiod3.Interop.gpiod_request_config_set_consumer(reqConfig, "Meadow");

            var settings = Gpiod3.Interop.gpiod_line_settings_new();
            if (settings == IntPtr.Zero)
            {
                throw new NativeException("Failed to create line settings");
            }

            try
            {
                // Set as input
                Gpiod3.Interop.gpiod_line_settings_set_direction(settings, Gpiod3.Interop.gpiod_line_direction.GPIOD_LINE_DIRECTION_INPUT);

                // Set edge detection based on interrupt mode
                var edge = mode switch
                {
                    InterruptMode.EdgeRising => Gpiod3.Interop.gpiod_line_edge.GPIOD_LINE_EDGE_RISING,
                    InterruptMode.EdgeFalling => Gpiod3.Interop.gpiod_line_edge.GPIOD_LINE_EDGE_FALLING,
                    InterruptMode.EdgeBoth => Gpiod3.Interop.gpiod_line_edge.GPIOD_LINE_EDGE_BOTH,
                    _ => Gpiod3.Interop.gpiod_line_edge.GPIOD_LINE_EDGE_NONE
                };

                if (edge != Gpiod3.Interop.gpiod_line_edge.GPIOD_LINE_EDGE_NONE)
                {
                    Gpiod3.Interop.gpiod_line_settings_set_edge_detection(settings, edge);
                }

                // Set bias
                if (bias != Gpiod3.Interop.gpiod_line_bias.GPIOD_LINE_BIAS_AS_IS)
                {
                    Gpiod3.Interop.gpiod_line_settings_set_bias(settings, bias);
                }

                var lineConfig = Gpiod3.Interop.gpiod_line_config_new();
                if (lineConfig == IntPtr.Zero)
                {
                    throw new NativeException("Failed to create line config");
                }

                try
                {
                    uint[] offsets = { (uint)Offset };
                    if (Gpiod3.Interop.gpiod_line_config_add_line_settings(lineConfig, offsets, (UIntPtr)1, settings) < 0)
                    {
                        throw new NativeException("Failed to add line settings");
                    }

                    _requestHandle = Gpiod3.Interop.gpiod_chip_request_lines(_chip.Handle, reqConfig, lineConfig);
                    if (_requestHandle == IntPtr.Zero)
                    {
                        throw new NativeException($"Failed to request line {Offset} for interrupts", Marshal.GetLastWin32Error());
                    }
                }
                finally
                {
                    Gpiod3.Interop.gpiod_line_config_free(lineConfig);
                }
            }
            finally
            {
                Gpiod3.Interop.gpiod_line_settings_free(settings);
            }
        }
        finally
        {
            Gpiod3.Interop.gpiod_request_config_free(reqConfig);
        }
    }

    public void Release()
    {
        if (_requestHandle != IntPtr.Zero)
        {
            Gpiod3.Interop.gpiod_line_request_release(_requestHandle);
            _requestHandle = IntPtr.Zero;
        }

        RefreshInfo();
    }

    public void SetValue(bool state)
    {
        if (_requestHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException("Line not requested");
        }

        var value = state ? Gpiod3.Interop.gpiod_line_value.GPIOD_LINE_VALUE_ACTIVE : Gpiod3.Interop.gpiod_line_value.GPIOD_LINE_VALUE_INACTIVE;

        if (Gpiod3.Interop.gpiod_line_request_set_value(_requestHandle, (uint)Offset, value) < 0)
        {
            throw new NativeException("Failed to set line value", Marshal.GetLastWin32Error());
        }
    }

    public bool GetValue()
    {
        if (_requestHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException("Line not requested");
        }

        var value = Gpiod3.Interop.gpiod_line_request_get_value(_requestHandle, (uint)Offset);

        if (value == Gpiod3.Interop.gpiod_line_value.GPIOD_LINE_VALUE_ERROR)
        {
            throw new NativeException("Failed to get line value", Marshal.GetLastWin32Error());
        }

        return value == Gpiod3.Interop.gpiod_line_value.GPIOD_LINE_VALUE_ACTIVE;
    }

    public override string ToString()
    {
        var c = string.IsNullOrEmpty(Consumer) ? "unused" : $"\"{Consumer}\"";
        var n = string.IsNullOrEmpty(Name) ? $"line-{Offset}" : $"\"{Name}\"";
        var d = Direction == Gpiod3.Interop.gpiod_line_direction.GPIOD_LINE_DIRECTION_INPUT ? " input" : "output";

        return $"line {Offset:00}: {n,24}{c,24}  {d}";
    }

    ~LineInfo3()
    {
        Release();

        if (_infoHandle != IntPtr.Zero)
        {
            Gpiod3.Interop.gpiod_line_info_free(_infoHandle);
            _infoHandle = IntPtr.Zero;
        }
    }
}
