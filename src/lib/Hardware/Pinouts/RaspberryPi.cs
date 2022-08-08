using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Pinouts
{
    public class RaspberryPi : IPinDefinitions
    {
        public IEnumerator<IPin> GetEnumerator() => AllPins.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IList<IPin> AllPins => new List<IPin>
        {
            GPIO2, GPIO3, GPIO4, GPIO17, GPIO18, GPIO27, GPIO22, GPIO23,
            GPIO24, GPIO10, GPIO9, GPIO25, GPIO11, GPIO8, GPIO7, GPIO5,
            GPIO6, GPIO12, GPIO13, GPIO19, GPIO16, GPIO26, GPIO20, GPIO21
        };

        public IPin GPIO2 => new GpiodPin("GPIO2", "PIN03", "gpiochip0", 2);
        public IPin GPIO3 => new GpiodPin("GPIO3", "PIN05", "gpiochip0", 3);
        public IPin GPIO4 => new GpiodPin("GPIO4", "PIN07", "gpiochip0", 4);
        public IPin GPIO17 => new GpiodPin("GPIO17", "PIN11", "gpiochip0", 17);
        public IPin GPIO18 => new GpiodPin("GPIO18", "PIN12", "gpiochip0", 18);
        public IPin GPIO27 => new GpiodPin("GPIO27", "PIN13", "gpiochip0", 27);
        public IPin GPIO22 => new GpiodPin("GPIO22", "PIN15", "gpiochip0", 22);

        // Pi may or may not support GPIOD - depends on OS
        public IPin GPIO23 => new LinuxFlexiPin("GPIO23", "PIN16", 23, "gpiochip0", 23);
        public IPin GPIO24 => new LinuxFlexiPin("GPIO24", "PIN18", 24, "gpiochip0", 24);

        public IPin GPIO10 => new GpiodPin("GPIO10", "PIN19", "gpiochip0", 10);
        public IPin GPIO9 => new GpiodPin("GPIO9", "PIN21", "gpiochip0", 9);
        public IPin GPIO25 => new GpiodPin("GPIO25", "PIN22", "gpiochip0", 25);
        public IPin GPIO11 => new GpiodPin("GPIO11", "PIN23", "gpiochip0", 11);
        public IPin GPIO8 => new GpiodPin("GPIO8", "PIN24", "gpiochip0", 8);
        public IPin GPIO7 => new GpiodPin("GPIO7", "PIN26", "gpiochip0", 7);
        public IPin GPIO5 => new GpiodPin("GPIO5", "PIN29", "gpiochip0", 5);
        public IPin GPIO6 => new GpiodPin("GPIO6", "PIN31", "gpiochip0", 6);
        public IPin GPIO12 => new GpiodPin("GPIO12", "PIN32", "gpiochip0", 12);
        public IPin GPIO13 => new GpiodPin("GPIO13", "PIN33", "gpiochip0", 13);
        public IPin GPIO19 => new GpiodPin("GPIO19", "PIN35", "gpiochip0", 19);
        public IPin GPIO16 => new GpiodPin("GPIO16", "PIN36", "gpiochip0", 16);
        public IPin GPIO26 => new GpiodPin("GPIO26", "PIN37", "gpiochip0", 26);
        public IPin GPIO20 => new GpiodPin("GPIO20", "PIN38", "gpiochip0", 20);
        public IPin GPIO21 => new GpiodPin("GPIO21", "PIN40", "gpiochip0", 21);

        // aliases for sanity
        public IPin Pin3 => GPIO2;
        public IPin Pin5 => GPIO3;
        public IPin Pin7 => GPIO4;
        public IPin Pin11 => GPIO17;
        public IPin Pin12 => GPIO18;
        public IPin Pin13 => GPIO27;
        public IPin Pin15 => GPIO22;
        public IPin Pin16 => GPIO23;
        public IPin Pin18 => GPIO24;
        public IPin Pin19 => GPIO10;
        public IPin Pin21 => GPIO9;
        public IPin Pin22 => GPIO25;
        public IPin Pin23 => GPIO11;
        public IPin Pin24 => GPIO8;
        public IPin Pin26 => GPIO7;
        public IPin Pin29 => GPIO5;
        public IPin Pin31 => GPIO6;
        public IPin Pin32 => GPIO12;
        public IPin Pin33 => GPIO13;
        public IPin Pin35 => GPIO19;
        public IPin Pin36 => GPIO16;
        public IPin Pin37 => GPIO26;
        public IPin Pin38 => GPIO20;
        public IPin Pin40 => GPIO21;

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