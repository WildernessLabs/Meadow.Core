using Meadow.Devices;
using Meadow.Hardware;

namespace Meadow
{
    public interface IJetsonNanoMeadowDevice :
        IMeadowDevice,
        IIOController<JetsonNanoPinout> 
    { 
    }
}