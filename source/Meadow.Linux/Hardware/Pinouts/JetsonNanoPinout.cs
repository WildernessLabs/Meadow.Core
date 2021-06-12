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

        public IPin D04 => new SysFsPin("D04", "PIN07", 216);

        public IPin UART_2_TX => new Pin("UART_2_TX", "PIN08");
        public IPin UART_2_RX => new Pin("UART_2_RX", "PIN10");

        public IPin D17 => new SysFsPin("D17", "PIN11", 50);
        public IPin D18 => new SysFsPin("D18", "PIN12", 79);

        public IPin D27 => new SysFsPin("D27", "PIN13", 14);
        public IPin D22 => new SysFsPin("D22", "PIN15", 194);
        public IPin D23 => new SysFsPin("D23", "PIN16", 232);
        public IPin D24 => new SysFsPin("D24", "PIN18", 15);
        public IPin D10 => new SysFsPin("D10", "PIN19", 16);
        public IPin D09 => new SysFsPin("D09", "PIN21", 17);
        public IPin D25 => new SysFsPin("D25", "PIN22", 13);
        public IPin D11 => new SysFsPin("D11", "PIN23", 18);
        public IPin D08 => new SysFsPin("D08", "PIN24", 19);
        public IPin D07 => new SysFsPin("D07", "PIN26", 20);

        public IPin I2C_1_SDA => new Pin("I2C_1_SDA", "PIN27");
        public IPin I2C_1_SCL => new Pin("I2C_1_SCL", "PIN28");

        public IPin D05 => new SysFsPin("D05", "PIN29", 149);
        public IPin D06 => new SysFsPin("D06", "PIN31", 200);
        public IPin D12 => new SysFsPin("D12", "PIN32", 168);
        public IPin D13 => new SysFsPin("D13", "PIN33", 38);
        public IPin D19 => new SysFsPin("D19", "PIN35", 76);
        public IPin D16 => new SysFsPin("D16", "PIN36", 51);
        public IPin D26 => new SysFsPin("D26", "PIN37", 12);
        public IPin D20 => new SysFsPin("D20", "PIN38", 77);
        public IPin D21 => new SysFsPin("D21", "PIN40", 78);

    }
}
