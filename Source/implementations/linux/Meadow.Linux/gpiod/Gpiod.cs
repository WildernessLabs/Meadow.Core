using Meadow.Logging;
using System;
using System.Runtime.InteropServices;

namespace Meadow;

internal abstract class Gpiod : IDisposable
{
    public bool IsDisposed { get; private set; }

    protected Logger? Logger { get; }

    // Abstract members that version-specific implementations must provide
    public abstract IChipCollection Chips { get; }
    public abstract ILineInfo GetLine(GpiodPin pin);
    public abstract ILineInfo GetLine(LinuxFlexiPin pin);

    protected Gpiod(Logger? logger)
    {
        Logger = logger;
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

    public static Gpiod? GetGpiod(Logger? logger)
    {
        // Try v3 first (Debian Trixie and newer)
        if (NativeLibrary.TryLoad("libgpiod.so.3", out IntPtr handle))
        {
            NativeLibrary.Free(handle);
            logger?.Info("Using libgpiod v3 (API v2.x)");
            return new Gpiod3(logger);
        }

        // Fallback to v2 (Debian Bookworm and older)
        if (NativeLibrary.TryLoad("libgpiod.so.2", out handle))
        {
            NativeLibrary.Free(handle);
            logger?.Info("Using libgpiod v2 (API v1.x)");
            return new Gpiod2(logger);
        }

        logger?.Warn("No compatible libgpiod version found");
        return null;
    }
}
