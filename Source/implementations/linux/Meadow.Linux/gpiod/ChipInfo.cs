using Meadow.Logging;
using System;

namespace Meadow;

/// <summary>
/// Abstract base class for GPIO chip information across libgpiod versions
/// </summary>
internal abstract class ChipInfo : IDisposable
{
    protected Logger? Logger { get; }

    /// <summary>
    /// Native handle to the chip
    /// </summary>
    public IntPtr Handle { get; protected set; }

    /// <summary>
    /// Chip name (e.g., "gpiochip0")
    /// </summary>
    public string Name { get; protected set; } = string.Empty;

    /// <summary>
    /// Chip label/description (e.g., "pinctrl-bcm2711")
    /// </summary>
    public string Label { get; protected set; } = string.Empty;

    /// <summary>
    /// Number of GPIO lines available on this chip
    /// </summary>
    public abstract int LineCount { get; }

    /// <summary>
    /// Whether this chip handle is invalid
    /// </summary>
    public abstract bool IsInvalid { get; }

    protected ChipInfo(Logger? logger)
    {
        Logger = logger;
    }

    public abstract void Dispose();

    public override string ToString()
    {
        // Same format as gpiodetect
        // gpiochip0 [pinctrl-bcm2711] (58 lines)
        return $"{Name} [{Label}] ({LineCount} lines)";
    }
}
