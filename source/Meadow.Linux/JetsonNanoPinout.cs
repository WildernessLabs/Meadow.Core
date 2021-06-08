using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    public class JetsonNanoPinout : IPinDefinitions
    {
        public IList<IPin> AllPins => new List<IPin> {
            I2C_2_SDA, I2C_2_SCL, 
            UART_2_TX, UART_2_RX,
            I2C_1_SDA, I2C_1_SCL,
        };

        public IPin I2C_2_SDA => new Pin("I2C_2_SDA", "PIN03");
        public IPin I2C_2_SCL => new Pin("I2C_2_SCL", "PIN05");

        public IPin UART_2_TX => new Pin("UART_2_TX", "PIN08");
        public IPin UART_2_RX => new Pin("UART_2_RX", "PIN10");

        public IPin I2C_1_SDA => new Pin("I2C_1_SDA", "PIN27");
        public IPin I2C_1_SCL => new Pin("I2C_1_SCL", "PIN28");
    }
}
