using Meadow.Peripherals.Displays;

namespace Meadow;

/// <summary>
/// Represents a desktop implementation of the Meadow device that uses a virtual display.
/// </summary>
public class Desktop<TDisplay> : Desktop where TDisplay : IPixelDisplay
{
    private TDisplay? _virtualDisplay;

    /// <inheritdoc/>
    public override IPixelDisplay? Display
    {
        get
        {
            if (_virtualDisplay is null && Implementation is IPixelDisplayProvider provider)
            {
                var renderer = provider.CreateDisplay();

                var ctor = typeof(TDisplay).GetConstructor([typeof(IResizablePixelDisplay)]);

                _virtualDisplay = (TDisplay)ctor?.Invoke([renderer])!;
            }

            return _virtualDisplay;
        }
    }
    /// <summary>
    /// Initializes a new instance of the Desktop class with a specific display type.
    /// </summary>
    public Desktop() : base()
    {
    }
}