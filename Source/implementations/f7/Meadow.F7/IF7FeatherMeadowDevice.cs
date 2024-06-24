using Meadow.Hardware;

namespace Meadow
{
    /// <summary>
    /// Represents a Meadow Feather device built on the STM32F7 hardware
    /// </summary>
    public interface IF7FeatherMeadowDevice :
        IF7MeadowDevice,
        IIOController<IF7FeatherPinout>
    {
    }
}
