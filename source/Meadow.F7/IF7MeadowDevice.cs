using Meadow.Devices;
using Meadow.Hardware;

namespace Meadow
{
    public interface IF7MeadowDevice :
        IMeadowDevice,
        IIOController<IF7MicroPinout>
    {

    }
}
