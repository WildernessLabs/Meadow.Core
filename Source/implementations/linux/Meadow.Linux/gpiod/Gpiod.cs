using Meadow.Logging;
using System;

namespace Meadow;

internal partial class Gpiod : IDisposable
{
    private class PinInfo
    {
        public int FileDescriptor { get; set; }
        public int ReferenceCount { get; set; }
    }

    public bool IsDisposed { get; private set; }

    private ChipCollection Chips { get; set; }
    private Logger Logger { get; }

    public unsafe Gpiod(Logger logger)
    {
        Chips = new ChipCollection();
        Logger = logger;

        var iter = Interop.gpiod_chip_iter_new();

        try
        {
            IntPtr p;

            do
            {
                p = Interop.gpiod_chip_iter_next_noclose(iter);

                var info = ChipInfo.FromIntPtr(logger, p);
                if (!info.IsInvalid)
                {
                    Chips.Add(info);

                    foreach (var line in info.Lines)
                    {
                        Logger.Debug(line.ToString());
                    }
                }
            } while (p != IntPtr.Zero);
        }
        finally
        {
            Interop.gpiod_chip_iter_free(iter);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            foreach (var chip in Chips)
            {
                chip.Dispose();
            }

            IsDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public LineInfo GetLine(GpiodPin pin)
    {
        if (!Chips.Contains(pin.Chip))
        {
            throw new NativeException($"Unknown GPIO chip {pin.Chip}");
        }

        var line = Chips[pin.Chip]!.Lines[pin.Offset];

        // TODO: check availability, check for other reservations

        return line;
    }

    public LineInfo GetLine(LinuxFlexiPin pin)
    {
        if (!Chips.Contains(pin.GpiodChip))
        {
            throw new NativeException($"Unknown GPIO chip {pin.GpiodChip}");
        }

        var line = Chips[pin.GpiodChip]!.Lines[pin.GpiodOffset];

        // TODO: check availability, check for other reservations

        return line;
    }
}
