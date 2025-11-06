using Meadow.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Meadow;

internal partial class Gpiod2 : Gpiod
{
    private class PinInfo
    {
        public int FileDescriptor { get; set; }
        public int ReferenceCount { get; set; }
    }

    private readonly ChipCollection2 _chips = new ChipCollection2();
    public override IChipCollection Chips => _chips;

    public unsafe Gpiod2(Logger logger)
        : base(logger)
    {
        var iter = Interop.gpiod_chip_iter_new();

        try
        {
            IntPtr p;

            do
            {
                p = Interop.gpiod_chip_iter_next_noclose(iter);

                var info = ChipInfo2.FromIntPtr(logger, p);
                if (!info.IsInvalid)
                {
                    _chips.Add(info);

                    foreach (var line in info.Lines)
                    {
                        Logger.Debug($"{info.Name} {line}");
                    }
                }
            } while (p != IntPtr.Zero);
        }
        finally
        {
            Interop.gpiod_chip_iter_free(iter);
        }
    }

    internal void LogAllAvailablePins()
    {
        var names = Directory.GetFiles("/dev", "gpiochip*");

        foreach (var n in names)
        {
            Logger.Debug($"opening {n}");

            var info = ChipInfo2.FromIntPtr(Logger, Interop.gpiod_chip_open_by_name(n));
            if (!info.IsInvalid)
            {
                Chips.Add(info);

                Logger.Debug(info.ToString());

                foreach (var line in info.Lines)
                {
                    Logger.Debug(line.ToString());
                }
            }
            else
            {
                Console.WriteLine($"ERR: {Marshal.GetLastWin32Error()}");

                Logger.Error($"Unable to get info for chip {n}");
            }
        }
    }

    public override LineInfo2 GetLine(GpiodPin pin)
    {
        if (!_chips.Contains(pin.Chip))
        {
            throw new NativeException($"Unknown GPIO chip {pin.Chip}");
        }

        var line = (_chips[pin.Chip] as ChipInfo2)!.Lines[pin.Offset];

        // TODO: check availability, check for other reservations

        return line;
    }

    public override LineInfo2 GetLine(LinuxFlexiPin pin)
    {
        if (!_chips.Contains(pin.GpiodChip))
        {
            throw new NativeException($"Unknown GPIO chip {pin.GpiodChip}");
        }

        var line = (_chips[pin.GpiodChip] as ChipInfo2)!.Lines[pin.GpiodOffset];

        // TODO: check availability, check for other reservations

        return line;
    }
}
