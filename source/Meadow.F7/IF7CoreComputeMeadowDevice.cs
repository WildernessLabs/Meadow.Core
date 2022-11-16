using Meadow.Hardware;

namespace Meadow
{
    public interface IF7CoreComputeMeadowDevice :
        IF7MeadowDevice,
        IIOController<IF7CoreComputePinout>
    {
    }
}
