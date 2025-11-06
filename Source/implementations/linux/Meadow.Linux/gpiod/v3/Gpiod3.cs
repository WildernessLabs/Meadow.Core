using Meadow.Logging;
using System;
using System.IO;

namespace Meadow;

/// <summary>
/// Gpiod implementation for libgpiod v3 (API v2.x)
/// Used on Debian Trixie and newer systems
/// </summary>
internal partial class Gpiod3 : Gpiod
{
    private readonly ChipCollection3 _chips = new ChipCollection3();
    public override IChipCollection Chips => _chips;

    public Gpiod3(Logger logger)
        : base(logger)
    {
        // V3 API doesn't have chip iterators - enumerate /dev/gpiochip* directly
        var chipPaths = Directory.GetFiles("/dev", "gpiochip*");
        Array.Sort(chipPaths); // Ensure consistent ordering

        foreach (var path in chipPaths)
        {
            Logger.Debug($"Opening {path}");

            try
            {
                var chipInfo = ChipInfo3.FromPath(logger, path);
                if (!chipInfo.IsInvalid)
                {
                    _chips.Add(chipInfo);
                    Logger.Info($"Found GPIO chip: {chipInfo.Name} ({chipInfo.NumLines} lines)");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Failed to open {path}: {ex.Message}");
            }
        }

        if (_chips.Count == 0)
        {
            Logger.Warn("No GPIO chips found");
        }
    }

    public override ILineInfo GetLine(GpiodPin pin)
    {
        if (!_chips.Contains(pin.Chip))
        {
            throw new NativeException($"Unknown GPIO chip {pin.Chip}");
        }

        var chip = _chips[pin.Chip] as ChipInfo3;
        if (chip == null)
        {
            throw new NativeException($"Chip {pin.Chip} is not a v3 chip");
        }

        return chip.Lines[pin.Offset];
    }

    public override ILineInfo GetLine(LinuxFlexiPin pin)
    {
        if (!_chips.Contains(pin.GpiodChip))
        {
            throw new NativeException($"Unknown GPIO chip {pin.GpiodChip}");
        }

        var chip = _chips[pin.GpiodChip] as ChipInfo3;
        if (chip == null)
        {
            throw new NativeException($"Chip {pin.GpiodChip} is not a v3 chip");
        }

        return chip.Lines[pin.GpiodOffset];
    }
}
