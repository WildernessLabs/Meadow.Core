using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    public class RaspberryPiPinout : IPinDefinitions
    {
        public IList<IPin> AllPins => new List<IPin> {
            GPIO2, GPIO3, GPIO4, GPIO10, GPIO11, GPIO12, GPIO13, GPIO16, GPIO17, GPIO18, GPIO19, GPIO20, GPIO21, GPIO22, GPIO23, GPIO24, GPIO25, GPIO26, GPIO27
        };

        public IPin GPIO2 => new Pin("GPIO2", "PIN03");
        public IPin GPIO3 => new Pin("GPIO3", "PIN05");
        public IPin GPIO4 => new Pin("GPIO4", "PIN07");

        public IPin GPIO17 => new Pin("GPIO17", "PIN11");
        public IPin GPIO18 => new Pin("GPIO18", "PIN12");
        public IPin GPIO27 => new Pin("GPIO27", "PIN13");
        public IPin GPIO22 => new Pin("GPIO22", "PIN15");
        public IPin GPIO23 => new Pin("GPIO23", "PIN16");
        public IPin GPIO24 => new Pin("GPIO24", "PIN18");

        public IPin GPIO10 => new Pin("GPIO10", "PIN19");
        public IPin GPIO9 => new Pin("GPIO9", "PIN21");
        public IPin GPIO25 => new Pin("GPIO25", "PIN22");
        public IPin GPIO11 => new Pin("GPIO11", "PIN23");
        public IPin GPIO8 => new Pin("GPIO8", "PIN24");
        public IPin GPIO7 => new Pin("GPIO7", "PIN26");

        public IPin GPIO5 => new Pin("GPIO5", "PIN29");
        public IPin GPIO6 => new Pin("GPIO6", "PIN31");
        public IPin GPIO12 => new Pin("GPIO12", "PIN32");
        public IPin GPIO13 => new Pin("GPIO13", "PIN33");
        public IPin GPIO19 => new Pin("GPIO19", "PIN35");
        public IPin GPIO16 => new Pin("GPIO16", "PIN36");
        public IPin GPIO26 => new Pin("GPIO26", "PIN37");
        public IPin GPIO20 => new Pin("GPIO20", "PIN38");
//        public IPin GPIO21 => new Pin("GPIO21", "PIN40");
        public IPin GPIO21 => new SysFsPin("GPIO21", "PIN40", 21);

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