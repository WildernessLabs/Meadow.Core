using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    public class JetsonNanoPinout : IPinDefinitions
    {
        public IList<IPin> AllPins => new List<IPin> {
            I2C_2_SDA, I2C_2_SCL
        };

        public IPin I2C_2_SDA => new Pin(
            "I2C_2_SDA", "PIN03");
        public IPin I2C_2_SCL => new Pin(
            "I2C_2_SCL", "PIN05");

    }
}
