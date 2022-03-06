using Meadow.Hardware;

namespace Meadow
{
    public interface IF7FeatherMeadowDevice :
        IF7MeadowDevice,
        IIOController<IF7FeatherPinout>
    {
    }
}
