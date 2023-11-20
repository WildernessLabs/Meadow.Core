using Meadow.Hardware;

namespace Meadow
{
    /// <summary>
    /// Represents a Meadow Core Compute Module built on the STM32F7 hardware
    /// </summary>
    public interface IF7CoreComputeMeadowDevice :
        IF7MeadowDevice,
        IIOController<IF7CoreComputePinout>
    {
    }
}
