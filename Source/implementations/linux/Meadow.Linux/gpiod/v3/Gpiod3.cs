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

    public override LineInfo2 GetLine(GpiodPin pin)
    {
        // TODO: V3 uses different types - need adapter or return LineInfo3
        throw new NotImplementedException("Gpiod v3 requires migration to LineInfo3 types");
    }

    public override LineInfo2 GetLine(LinuxFlexiPin pin)
    {
        // TODO: V3 uses different types - need adapter or return LineInfo3
        throw new NotImplementedException("Gpiod v3 requires migration to LineInfo3 types");
    }

    /// <summary>
    /// Get a line using v3 API - returns LineInfo3
    /// </summary>
    public LineInfo3 GetLine3(string chipName, int offset)
    {
        var chip = _chips[chipName] as ChipInfo3;
        if (chip == null)
        {
            throw new NativeException($"Unknown GPIO chip {chipName}");
        }

        return chip.Lines[offset];
    }
}
