using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    public class RaspberryPiPinout : IPinDefinitions
    {
        public IList<IPin> AllPins => new List<IPin> {
            GPIO2, GPIO3
        };

        public IPin GPIO2 => new Pin(
            "GPIO2", "PIN03");
        public IPin GPIO3 => new Pin(
            "GPIO3", "PIN05");

        public IPin I2C1_SDA => GPIO2;
        public IPin I2C1_SCL => GPIO3;

    }
}
