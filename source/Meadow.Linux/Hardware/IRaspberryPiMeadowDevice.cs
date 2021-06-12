using Meadow.Devices;
using Meadow.Hardware;

namespace Meadow
{
    public interface IRaspberryPiMeadowDevice :
    IMeadowDevice,
    IIOController<RaspberryPiPinout> {}
}