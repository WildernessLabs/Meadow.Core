using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Pinouts
{
    public class JetsonNano : IPinDefinitions
    {
        public IEnumerator<IPin> GetEnumerator() => AllPins.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IList<IPin> AllPins => new List<IPin> {
            I2C_2_SDA, I2C_2_SCL,
            UART_2_TX, UART_2_RX,
            I2C_1_SDA, I2C_1_SCL,
        };

        public IPinController Controller { get; set; }

        public JetsonNano()
        {
        }

        public IPin I2C_2_SDA => new Pin(Controller, "I2C_2_SDA", "PIN03", null);
        public IPin I2C_2_SCL => new Pin(Controller, "I2C_2_SCL", "PIN05", null);

        public IPin D04 => new SysFsPin(Controller, "D04", "PIN07", 216);

        public IPin UART_2_TX => new Pin(Controller, "UART_2_TX", "PIN08", null);
        public IPin UART_2_RX => new Pin(Controller, "UART_2_RX", "PIN10", null);

        public IPin D17 => new SysFsPin(Controller, "D17", "PIN11", 50);
        public IPin D18 => new SysFsPin(Controller, "D18", "PIN12", 79);

        public IPin D27 => new SysFsPin(Controller, "D27", "PIN13", 14);
        public IPin D22 => new SysFsPin(Controller, "D22", "PIN15", 194);
        public IPin D23 => new SysFsPin(Controller, "D23", "PIN16", 232);
        public IPin D24 => new SysFsPin(Controller, "D24", "PIN18", 15);
        public IPin D10 => new SysFsPin(Controller, "D10", "PIN19", 16);
        public IPin D09 => new SysFsPin(Controller, "D09", "PIN21", 17);
        public IPin D25 => new SysFsPin(Controller, "D25", "PIN22", 13);
        public IPin D11 => new SysFsPin(Controller, "D11", "PIN23", 18);
        public IPin D08 => new SysFsPin(Controller, "D08", "PIN24", 19);
        public IPin D07 => new SysFsPin(Controller, "D07", "PIN26", 20);

        public IPin I2C_1_SDA => new Pin(Controller, "I2C_1_SDA", "PIN27", null);
        public IPin I2C_1_SCL => new Pin(Controller, "I2C_1_SCL", "PIN28", null);

        public IPin D05 => new SysFsPin(Controller, "D05", "PIN29", 149);
        public IPin D06 => new SysFsPin(Controller, "D06", "PIN31", 200);
        public IPin D12 => new SysFsPin(Controller, "D12", "PIN32", 168);
        public IPin D13 => new SysFsPin(Controller, "D13", "PIN33", 38);
        public IPin D19 => new SysFsPin(Controller, "D19", "PIN35", 76);
        public IPin D16 => new SysFsPin(Controller, "D16", "PIN36", 51);
        public IPin D26 => new SysFsPin(Controller, "D26", "PIN37", 12);
        public IPin D20 => new SysFsPin(Controller, "D20", "PIN38", 77);
        public IPin D21 => new SysFsPin(Controller, "D21", "PIN40", 78);

    }
}
