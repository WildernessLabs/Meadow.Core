using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    public class RaspberryPiPinout : IPinDefinitions
    {
        public IList<IPin> AllPins => new List<IPin> {
            GPIO2, GPIO3, GPIO4, GPIO10, GPIO11, GPIO12, GPIO13, GPIO16, GPIO17, GPIO18, GPIO19, GPIO20, GPIO21, GPIO22, GPIO23, GPIO24, GPIO25, GPIO26, GPIO27
        };

        public IPin GPIO2 => new SysFsPin("GPIO2", "PIN03", 2);
        public IPin GPIO3 => new SysFsPin("GPIO3", "PIN05", 3);
        public IPin GPIO4 => new SysFsPin("GPIO4", "PIN07", 4);
        public IPin GPIO17 => new SysFsPin("GPIO17", "PIN11", 17);
        public IPin GPIO18 => new SysFsPin("GPIO18", "PIN12", 18);
        public IPin GPIO27 => new SysFsPin("GPIO27", "PIN13", 27);
        public IPin GPIO22 => new SysFsPin("GPIO22", "PIN15", 22);
        public IPin GPIO23 => new SysFsPin("GPIO23", "PIN16", 23);
        public IPin GPIO24 => new SysFsPin("GPIO24", "PIN18", 24);
        public IPin GPIO10 => new SysFsPin("GPIO10", "PIN19", 10);
        public IPin GPIO9 => new SysFsPin("GPIO9", "PIN21", 9);
        public IPin GPIO25 => new SysFsPin("GPIO25", "PIN22", 25);
        public IPin GPIO11 => new SysFsPin("GPIO11", "PIN23", 11);
        public IPin GPIO8 => new SysFsPin("GPIO8", "PIN24", 8);
        public IPin GPIO7 => new SysFsPin("GPIO7", "PIN26", 7);
        public IPin GPIO5 => new SysFsPin("GPIO5", "PIN29", 5);
        public IPin GPIO6 => new SysFsPin("GPIO6", "PIN31", 6);
        public IPin GPIO12 => new SysFsPin("GPIO12", "PIN32", 12);
        public IPin GPIO13 => new SysFsPin("GPIO13", "PIN33", 13);
        public IPin GPIO19 => new SysFsPin("GPIO19", "PIN35", 19);
        public IPin GPIO16 => new SysFsPin("GPIO16", "PIN36", 16);
        public IPin GPIO26 => new SysFsPin("GPIO26", "PIN37", 26);
        public IPin GPIO20 => new SysFsPin("GPIO20", "PIN38", 20);
        public IPin GPIO21 => new SysFsPin("GPIO21", "PIN40", 21);

        // alias for sanity
        public IPin Pin40 => GPIO21;
        public IPin J8_40 => GPIO21;

        public IPin I2C1_SDA => GPIO2;
        public IPin I2C1_SCL => GPIO3;

        public IPin SPI0_MOSI => GPIO10;
        public IPin SPI0_MISO => GPIO9;
        public IPin SPI0_SCLK => GPIO11;
        public IPin SPI0_CS0 => GPIO8;
        public IPin SPI0_CS1 => GPIO7;

        public IPin SPI1_MOSI => GPIO20;
        public IPin SPI1_MISO => GPIO19;
        public IPin SPI1_SCLK => GPIO21;
        public IPin SPI1_CS0 => GPIO16;
    }
}