using Meadow.Hardware;

namespace Meadow.Pinouts;

/// <summary>
/// Defines the pinout configuration for a Raspberry Pi.
/// </summary>
public class RaspberryPiPinout : PinDefinitionBase, IPinDefinitions
{
    internal RaspberryPiPinout() { }

    internal const string GpiodChipPi4 = "gpiochip0";
    internal const string GpiodChipPi5 = "gpiochip4";

    internal string GpiodChipName { get; } = "gpiochip0";
    internal int SysFsOffset { get; }

    internal RaspberryPiPinout(
        string gpiodChipName = GpiodChipPi4,
        int sysfsOffset = 0)
    {
        GpiodChipName = gpiodChipName;
        SysFsOffset = sysfsOffset;
    }

    /// <summary>
    /// Represents the ACT LED pin.
    /// </summary>
    public IPin ACT_LED => new RaspberryPi.LedPin(Controller, "ACT", "ACT", null);
    /// <summary>
    /// Represents the GPIO2 pin.
    /// </summary>
    public IPin GPIO2 => new LinuxFlexiPin(Controller, "GPIO2", "PIN03", 2 + SysFsOffset, GpiodChipName, 2);
    /// <summary>
    /// Represents the GPIO3 pin.
    /// </summary>
    public IPin GPIO3 => new LinuxFlexiPin(Controller, "GPIO3", "PIN05", 3 + SysFsOffset, GpiodChipName, 3);
    /// <summary>
    /// Represents the GPIO4 pin.
    /// </summary>
    public IPin GPIO4 => new LinuxFlexiPin(Controller, "GPIO4", "PIN07", 4 + SysFsOffset, GpiodChipName, 4);
    /// <summary>
    /// Represents the GPIO17 pin.
    /// </summary>
    public IPin GPIO17 => new LinuxFlexiPin(Controller, "GPIO17", "PIN11", 17 + SysFsOffset, GpiodChipName, 17);
    /// <summary>
    /// Represents the GPIO18 pin.
    /// </summary>
    public IPin GPIO18 => new LinuxFlexiPin(Controller, "GPIO18", "PIN12", 18 + SysFsOffset, GpiodChipName, 18);
    /// <summary>
    /// Represents the GPIO27 pin.
    /// </summary>
    public IPin GPIO27 => new LinuxFlexiPin(Controller, "GPIO27", "PIN13", 27 + SysFsOffset, GpiodChipName, 27);
    /// <summary>
    /// Represents the GPIO22 pin.
    /// </summary>
    public IPin GPIO22 => new LinuxFlexiPin(Controller, "GPIO22", "PIN15", 22 + SysFsOffset, GpiodChipName, 22);
    /// <summary>
    /// Represents the GPIO23 pin.
    /// </summary>
    public IPin GPIO23 => new LinuxFlexiPin(Controller, "GPIO23", "PIN16", 23 + SysFsOffset, GpiodChipName, 23);
    /// <summary>
    /// Represents the GPIO24 pin.
    /// </summary>
    public IPin GPIO24 => new LinuxFlexiPin(Controller, "GPIO24", "PIN18", 24 + SysFsOffset, GpiodChipName, 24);
    /// <summary>
    /// Represents the GPIO10 pin.
    /// </summary>
    public IPin GPIO10 => new LinuxFlexiPin(Controller, "GPIO10", "PIN19", 10 + SysFsOffset, GpiodChipName, 10);
    /// <summary>
    /// Represents the GPIO9 pin.
    /// </summary>
    public IPin GPIO9 => new LinuxFlexiPin(Controller, "GPIO9", "PIN21", 9 + SysFsOffset, GpiodChipName, 9);
    /// <summary>
    /// Represents the GPIO25 pin.
    /// </summary>
    public IPin GPIO25 => new LinuxFlexiPin(Controller, "GPIO25", "PIN22", 25 + SysFsOffset, GpiodChipName, 25);
    /// <summary>
    /// Represents the GPIO11 pin.
    /// </summary>
    public IPin GPIO11 => new LinuxFlexiPin(Controller, "GPIO11", "PIN23", 11 + SysFsOffset, GpiodChipName, 11);
    /// <summary>
    /// Represents the GPIO8 pin.
    /// </summary>
    public IPin GPIO8 => new LinuxFlexiPin(Controller, "GPIO8", "PIN24", 8 + SysFsOffset, GpiodChipName, 8);
    /// <summary>
    /// Represents the GPIO7 pin.
    /// </summary>
    public IPin GPIO7 => new LinuxFlexiPin(Controller, "GPIO7", "PIN26", 7 + SysFsOffset, GpiodChipName, 7);
    /// <summary>
    /// Represents the GPIO5 pin.
    /// </summary>
    public IPin GPIO5 => new LinuxFlexiPin(Controller, "GPIO5", "PIN29", 5 + SysFsOffset, GpiodChipName, 5);
    /// <summary>
    /// Represents the GPIO6 pin.
    /// </summary>
    public IPin GPIO6 => new LinuxFlexiPin(Controller, "GPIO6", "PIN31", 6 + SysFsOffset, GpiodChipName, 6);
    /// <summary>
    /// Represents the GPIO12 pin.
    /// </summary>
    public IPin GPIO12 => new LinuxFlexiPin(Controller, "GPIO12", "PIN32", 12 + SysFsOffset, GpiodChipName, 12);
    /// <summary>
    /// Represents the GPIO13 pin.
    /// </summary>
    public IPin GPIO13 => new LinuxFlexiPin(Controller, "GPIO13", "PIN33", 13 + SysFsOffset, GpiodChipName, 13);
    /// <summary>
    /// Represents the GPIO14 pin.
    /// </summary>
    public IPin GPIO14 => new LinuxFlexiPin(Controller, "GPIO14", "PIN08", 14 + SysFsOffset, GpiodChipName, 14);
    /// <summary>
    /// Represents the GPIO15 pin.
    /// </summary>
    public IPin GPIO15 => new LinuxFlexiPin(Controller, "GPIO15", "PIN10", 15 + SysFsOffset, GpiodChipName, 15);
    /// <summary>
    /// Represents the GPIO19 pin.
    /// </summary>
    public IPin GPIO19 => new LinuxFlexiPin(Controller, "GPIO19", "PIN35", 19 + SysFsOffset, GpiodChipName, 19);
    /// <summary>
    /// Represents the GPIO16 pin.
    /// </summary>
    public IPin GPIO16 => new LinuxFlexiPin(Controller, "GPIO16", "PIN36", 16 + SysFsOffset, GpiodChipName, 16);
    /// <summary>
    /// Represents the GPIO26 pin.
    /// </summary>
    public IPin GPIO26 => new LinuxFlexiPin(Controller, "GPIO26", "PIN37", 26 + SysFsOffset, GpiodChipName, 26);
    /// <summary>
    /// Represents the GPIO20 pin.
    /// </summary>
    public IPin GPIO20 => new LinuxFlexiPin(Controller, "GPIO20", "PIN38", 20 + SysFsOffset, GpiodChipName, 20);
    /// <summary>
    /// Represents the GPIO21 pin.
    /// </summary>
    public IPin GPIO21 => new LinuxFlexiPin(Controller, "GPIO21", "PIN40", 21 + SysFsOffset, GpiodChipName, 21);

    /// <summary>
    /// Represents Pin 3, which corresponds to GPIO2.
    /// </summary>
    public IPin Pin3 => GPIO2;
    /// <summary>
    /// Represents Pin 5, which corresponds to GPIO3.
    /// </summary>
    public IPin Pin5 => GPIO3;
    /// <summary>
    /// Represents Pin 7, which corresponds to GPIO4.
    /// </summary>
    public IPin Pin7 => GPIO4;
    /// <summary>
    /// Represents Pin 8, which corresponds to GPIO14.
    /// </summary>
    public IPin Pin8 => GPIO14;
    /// <summary>
    /// Represents Pin 10, which corresponds to GPIO15.
    /// </summary>
    public IPin Pin10 => GPIO15;
    /// <summary>
    /// Represents Pin 11, which corresponds to GPIO17.
    /// </summary>
    public IPin Pin11 => GPIO17;
    /// <summary>
    /// Represents Pin 12, which corresponds to GPIO18.
    /// </summary>
    public IPin Pin12 => GPIO18;
    /// <summary>
    /// Represents Pin 13, which corresponds to GPIO27.
    /// </summary>
    public IPin Pin13 => GPIO27;
    /// <summary>
    /// Represents Pin 15, which corresponds to GPIO22.
    /// </summary>
    public IPin Pin15 => GPIO22;
    /// <summary>
    /// Represents Pin 16, which corresponds to GPIO23.
    /// </summary>
    public IPin Pin16 => GPIO23;
    /// <summary>
    /// Represents Pin 18, which corresponds to GPIO24.
    /// </summary>
    public IPin Pin18 => GPIO24;
    /// <summary>
    /// Represents Pin 19, which corresponds to GPIO10.
    /// </summary>
    public IPin Pin19 => GPIO10;
    /// <summary>
    /// Represents Pin 21, which corresponds to GPIO9.
    /// </summary>
    public IPin Pin21 => GPIO9;
    /// <summary>
    /// Represents Pin 22, which corresponds to GPIO25.
    /// </summary>
    public IPin Pin22 => GPIO25;
    /// <summary>
    /// Represents Pin 23, which corresponds to GPIO11.
    /// </summary>
    public IPin Pin23 => GPIO11;
    /// <summary>
    /// Represents Pin 24, which corresponds to GPIO8.
    /// </summary>
    public IPin Pin24 => GPIO8;
    /// <summary>
    /// Represents Pin 26, which corresponds to GPIO7.
    /// </summary>
    public IPin Pin26 => GPIO7;
    /// <summary>
    /// Represents Pin 29, which corresponds to GPIO5.
    /// </summary>
    public IPin Pin29 => GPIO5;
    /// <summary>
    /// Represents Pin 31, which corresponds to GPIO6.
    /// </summary>
    public IPin Pin31 => GPIO6;
    /// <summary>
    /// Represents Pin 32, which corresponds to GPIO12.
    /// </summary>
    public IPin Pin32 => GPIO12;
    /// <summary>
    /// Represents Pin 33, which corresponds to GPIO13.
    /// </summary>
    public IPin Pin33 => GPIO13;
    /// <summary>
    /// Represents Pin 35, which corresponds to GPIO19.
    /// </summary>
    public IPin Pin35 => GPIO19;
    /// <summary>
    /// Represents Pin 36, which corresponds to GPIO16.
    /// </summary>
    public IPin Pin36 => GPIO16;
    /// <summary>
    /// Represents Pin 37, which corresponds to GPIO26.
    /// </summary>
    public IPin Pin37 => GPIO26;
    /// <summary>
    /// Represents Pin 38, which corresponds to GPIO20.
    /// </summary>
    public IPin Pin38 => GPIO20;
    /// <summary>
    /// Represents Pin 40, which corresponds to GPIO21.
    /// </summary>
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