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

        public IPinController Controller { get; set; }

        public RaspberryPi()
        {
        }

        public IPin GPIO2 => new LinuxFlexiPin(Controller, "GPIO2", "PIN03", 2, "gpiochip0", 2);
        public IPin GPIO3 => new LinuxFlexiPin(Controller, "GPIO3", "PIN05", 3, "gpiochip0", 3);
        public IPin GPIO4 => new LinuxFlexiPin(Controller, "GPIO4", "PIN07", 4, "gpiochip0", 4);
        public IPin GPIO17 => new LinuxFlexiPin(Controller, "GPIO17", "PIN11", 17, "gpiochip0", 17);
        public IPin GPIO18 => new LinuxFlexiPin(Controller, "GPIO18", "PIN12", 18, "gpiochip0", 18);
        public IPin GPIO27 => new LinuxFlexiPin(Controller, "GPIO27", "PIN13", 27, "gpiochip0", 27);
        public IPin GPIO22 => new LinuxFlexiPin(Controller, "GPIO22", "PIN15", 22, "gpiochip0", 22);

        // Pi may or may not support GPIOD - depends on OS
        public IPin GPIO23 => new LinuxFlexiPin(Controller, "GPIO23", "PIN16", 23, "gpiochip0", 23);
        public IPin GPIO24 => new LinuxFlexiPin(Controller, "GPIO24", "PIN18", 24, "gpiochip0", 24);

        public IPin GPIO10 => new LinuxFlexiPin(Controller, "GPIO10", "PIN19", 10, "gpiochip0", 10);
        public IPin GPIO9 => new LinuxFlexiPin(Controller, "GPIO9", "PIN21", 9, "gpiochip0", 9);
        public IPin GPIO25 => new LinuxFlexiPin(Controller, "GPIO25", "PIN22", 25, "gpiochip0", 25);
        public IPin GPIO11 => new LinuxFlexiPin(Controller, "GPIO11", "PIN23", 11, "gpiochip0", 11);
        public IPin GPIO8 => new LinuxFlexiPin(Controller, "GPIO8", "PIN24", 8, "gpiochip0", 8);
        public IPin GPIO7 => new LinuxFlexiPin(Controller, "GPIO7", "PIN26", 7, "gpiochip0", 7);
        public IPin GPIO5 => new LinuxFlexiPin(Controller, "GPIO5", "PIN29", 5, "gpiochip0", 5);
        public IPin GPIO6 => new LinuxFlexiPin(Controller, "GPIO6", "PIN31", 6, "gpiochip0", 6);
        public IPin GPIO12 => new LinuxFlexiPin(Controller, "GPIO12", "PIN32", 12, "gpiochip0", 12);
        public IPin GPIO13 => new LinuxFlexiPin(Controller, "GPIO13", "PIN33", 13, "gpiochip0", 13);
        public IPin GPIO19 => new LinuxFlexiPin(Controller, "GPIO19", "PIN35", 19, "gpiochip0", 19);
        public IPin GPIO16 => new LinuxFlexiPin(Controller, "GPIO16", "PIN36", 16, "gpiochip0", 16);
        public IPin GPIO26 => new LinuxFlexiPin(Controller, "GPIO26", "PIN37", 26, "gpiochip0", 26);
        public IPin GPIO20 => new LinuxFlexiPin(Controller, "GPIO20", "PIN38", 20, "gpiochip0", 20);
        public IPin GPIO21 => new LinuxFlexiPin(Controller, "GPIO21", "PIN40", 21, "gpiochip0", 21);

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