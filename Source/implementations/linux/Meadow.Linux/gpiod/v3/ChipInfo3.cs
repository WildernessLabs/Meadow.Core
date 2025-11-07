using Meadow.Logging;
using System;
using System.Runtime.InteropServices;

namespace Meadow;

/// <summary>
/// ChipInfo for libgpiod v3 (API v2.x)
/// V3 uses opaque pointers instead of direct struct access
/// </summary>
internal class ChipInfo3 : ChipInfo
{
    private IntPtr _infoHandle;

    public string Path { get; } = string.Empty;
    public uint NumLines { get; }

    public LineCollection3 Lines { get; }

    public override int LineCount => Lines.Count;

    public override bool IsInvalid => Handle == IntPtr.Zero;

    public static ChipInfo3 FromPath(Logger? logger, string path)
    {
        var chipHandle = Gpiod3.Interop.gpiod_chip_open(path);
        return new ChipInfo3(logger, chipHandle, path);
    }

    private ChipInfo3(Logger? logger, IntPtr chipHandle, string path)
        : base(logger)
    {
        Handle = chipHandle;
        Path = path;

        if (IsInvalid)
        {
            Logger?.Debug($"Chip handle is invalid for {path}");
            Lines = new LineCollection3(this, 0);
        }
        else
        {
            // Get chip info
            _infoHandle = Gpiod3.Interop.gpiod_chip_get_info(Handle);

            if (_infoHandle != IntPtr.Zero)
            {
                // Get name (returns IntPtr to string)
                var namePtr = Gpiod3.Interop.gpiod_chip_info_get_name(_infoHandle);
                Name = namePtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(namePtr) ?? string.Empty : string.Empty;

                // Get label
                var labelPtr = Gpiod3.Interop.gpiod_chip_info_get_label(_infoHandle);
                Label = labelPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(labelPtr) ?? string.Empty : string.Empty;

                // Get number of lines
                NumLines = Gpiod3.Interop.gpiod_chip_info_get_num_lines(_infoHandle);

                Lines = new LineCollection3(this, (int)NumLines);
            }
            else
            {
                Logger?.Warn($"Failed to get chip info for {path}");
                Lines = new LineCollection3(this, 0);
            }
        }
    }

    public override void Dispose()
    {
        if (_infoHandle != IntPtr.Zero)
        {
            Gpiod3.Interop.gpiod_chip_info_free(_infoHandle);
            _infoHandle = IntPtr.Zero;
        }

        if (Handle != IntPtr.Zero)
        {
            Gpiod3.Interop.gpiod_chip_close(Handle);
            Handle = IntPtr.Zero;
        }
    }
}
