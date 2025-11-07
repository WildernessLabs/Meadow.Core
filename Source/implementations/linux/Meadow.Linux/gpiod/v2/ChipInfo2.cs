using Meadow.Logging;
using System;
using System.Runtime.InteropServices;

namespace Meadow;

/// <summary>
/// ChipInfo for libgpiod v2 (API v1.x)
/// V2 uses struct marshaling for chip information
/// </summary>
internal class ChipInfo2 : ChipInfo
{
    private Gpiod2.Interop.gpiod_chip? Chip { get; }

    public LineCollection2 Lines { get; }

    public override int LineCount => Lines.Count;

    public override bool IsInvalid => Handle.ToInt64() <= 0;

    public static ChipInfo2 FromIntPtr(Logger? logger, IntPtr p)
    {
        return new ChipInfo2(logger, p);
    }

    private ChipInfo2(Logger? logger, IntPtr p)
        : base(logger)
    {
        Handle = p;

        if (IsInvalid)
        {
            Logger?.Debug($"Chip ptr is invalid - cannot get GPIOD chip details");
            Lines = new LineCollection2(this, 0);
        }
        else
        {
            Chip = Marshal.PtrToStructure<Gpiod2.Interop.gpiod_chip>(Handle);
            Name = Chip.Value.name;
            Label = Chip.Value.label;

            // Init as an array of nulls.  We'll populate as they are accessed
            Lines = new LineCollection2(this, (int)Chip.Value.num_lines);
        }
    }

    public override void Dispose()
    {
        if (IsInvalid) return;

        Gpiod2.Interop.gpiod_chip_close(Handle);
        Handle = IntPtr.Zero;
    }
}
