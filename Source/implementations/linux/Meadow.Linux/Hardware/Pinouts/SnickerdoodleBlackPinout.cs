using Meadow.Hardware;

namespace Meadow.Pinouts;

/// <summary>
/// Defines the pinout configuration for an krtkl Snickerdoodle Black
/// </summary>
public class SnickerdoodleBlackPinout : PinDefinitionBase, IPinDefinitions
{
    internal SnickerdoodleBlackPinout() { }

    //        public IPin GPIO0 => new GpiodPin("GPIO0", "PIN001", "gpiochip0", 0);
    /// <summary>
    /// Gets GPIO1 pin.
    /// </summary>
    public IPin GPIO1 => new GpiodPin(Controller, "GPIO1", "PIN001", "gpiochip0", 1);

    /// <summary>
    /// Gets GPIO2 pin.
    /// </summary>
    public IPin GPIO2 => new GpiodPin(Controller, "GPIO2", "PIN001", "gpiochip0", 2);

    /// <summary>
    /// Gets GPIO3 pin.
    /// </summary>
    public IPin GPIO3 => new GpiodPin(Controller, "GPIO3", "PIN001", "gpiochip0", 3);

    /// <summary>
    /// Gets GPIO4 pin.
    /// </summary>
    public IPin GPIO4 => new GpiodPin(Controller, "GPIO4", "PIN001", "gpiochip0", 4);

    /// <summary>
    /// Gets GPIO5 pin.
    /// </summary>
    public IPin GPIO5 => new GpiodPin(Controller, "GPIO5", "PIN001", "gpiochip0", 5);

    /// <summary>
    /// Gets GPIO6 pin.
    /// </summary>
    public IPin GPIO6 => new GpiodPin(Controller, "GPIO6", "PIN001", "gpiochip0", 6);

    /// <summary>
    /// Gets GPIO7 pin.
    /// </summary>
    public IPin GPIO7 => new GpiodPin(Controller, "GPIO7", "PIN001", "gpiochip0", 7);

    /// <summary>
    /// Gets GPIO8 pin.
    /// </summary>
    public IPin GPIO8 => new GpiodPin(Controller, "GPIO8", "PIN001", "gpiochip0", 8);
    //        public IPin GPIO9 => new GpiodPin("GPIO9", "PIN001", "gpiochip0", 9);

}