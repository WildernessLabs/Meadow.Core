using Meadow.Hardware;

namespace Meadow.Pinouts;

/// <summary>
/// Defines the pinout configuration for an NVIDIA Jetson Xavier AGX
/// </summary>
public class JetsonXavierAGXPinout : PinDefinitionBase, IPinDefinitions
{
    internal JetsonXavierAGXPinout() { }

    /// <summary>
    /// Gets the I2C_GP5_DAT pin.
    /// </summary>
    public IPin I2C_GP5_DAT => new Pin(Controller, "I2C_GP5_DAT", "PIN03",
        new IChannelInfo[]
        {
            new I2cChannelInfo("I2C_GP5_DAT", I2cChannelFunctionType.Data, busNumber: 8)
        });

    /// <summary>
    /// Gets the I2C_GP5_CLK pin.
    /// </summary>
    public IPin I2C_GP5_CLK => new Pin(Controller, "I2C_GP5_CLK", "PIN05",
        new IChannelInfo[]
        {
            new I2cChannelInfo("I2C_GP5_CLK", I2cChannelFunctionType.Clock, busNumber: 8)
        });

    /// <summary>
    /// Gets the MCLK05 pin.
    /// </summary>
    public IPin MCLK05 => new SysFsPin(Controller, "MCLK05", "PIN07", 422);

    /// <summary>
    /// Gets the UART1_RTS pin.
    /// </summary>
    public IPin UART1_RTS => new SysFsPin(Controller, "UART1_RTS", "PIN11", 428);

    /// <summary>
    /// Gets the I2S2_CLK pin.
    /// </summary>
    public IPin I2S2_CLK => new SysFsPin(Controller, "I2S2_CLK", "PIN12", 351);

    /// <summary>
    /// Gets the PWM01 pin.
    /// </summary>
    public IPin PWM01 => new SysFsPin(Controller, "PWM01", "PIN13", 424);

    /// <summary>
    /// Gets the GPIO27_PWM2 pin.
    /// </summary>
    public IPin GPIO27_PWM2 => new SysFsPin(Controller, "GPIO27_PWM2", "PIN15", 393);

    /// <summary>
    /// Gets the GPIO8_AO_DMIC_IN_DAT pin.
    /// </summary>
    public IPin GPIO8_AO_DMIC_IN_DAT => new SysFsPin(Controller, "GPIO8_AO_DMIC_IN_DAT", "PIN16", 256);

    /// <summary>
    /// Gets the GPIO35_PWM3 pin.
    /// </summary>
    public IPin GPIO35_PWM3 => new SysFsPin(Controller, "GPIO35_PWM3", "PIN18", 344);

    /// <summary>
    /// Gets the SPI1_MOSI pin.
    /// </summary>
    public IPin SPI1_MOSI => new SysFsPin(Controller, "SPI1_MOSI", "PIN19", 493,
        new IChannelInfo[]
        {
            new GpiodDigitalChannelInfo("SPI1_MOSI"),
            new SpiChannelInfo("SPI1_MOSI", SpiLineType.MOSI, busNumber: 1)
        });

    /// <summary>
    /// Gets the SPI1_MISO pin.
    /// </summary>
    public IPin SPI1_MISO => new SysFsPin(Controller, "SPI1_MISO", "PIN21", 492,
        new IChannelInfo[]
        {
            new GpiodDigitalChannelInfo("SPI1_MISO"),
            new SpiChannelInfo("SPI1_MISO", SpiLineType.MISO, busNumber: 1)
        });

    /// <summary>
    /// Gets the GPIO17_40HEADER pin.
    /// </summary>
    public IPin GPIO17_40HEADER => new SysFsPin(Controller, "GPIO17_40HEADER", "PIN22", 417);

    /// <summary>
    /// Gets the SPI1_SCLK pin.
    /// </summary>
    public IPin SPI1_SCLK => new SysFsPin(Controller, "SPI1_SCLK", "PIN23", 491,
        new IChannelInfo[]
        {
            new GpiodDigitalChannelInfo("SPI1_SCLK"),
            new SpiChannelInfo("SPI1_SCLK", SpiLineType.Clock, busNumber: 1)
        });

    /// <summary>
    /// Gets the SPI1_CS0 pin.
    /// </summary>
    public IPin SPI1_CS0 => new SysFsPin(Controller, "SPI1_CS0", "PIN24", 494);

    /// <summary>
    /// Gets the SPI1_CS1 pin.
    /// </summary>
    public IPin SPI1_CS1 => new SysFsPin(Controller, "SPI1_CS1", "PIN26", 495);

    /// <summary>
    /// Gets the I2C_GP2_DAT pin.
    /// </summary>
    public IPin I2C_GP2_DAT => new Pin(Controller, "I2C_GP2_DAT", "PIN27",
        new IChannelInfo[]
        {
            new I2cChannelInfo("I2C_GP2_DAT", I2cChannelFunctionType.Data, busNumber: 1)
        });

    /// <summary>
    /// Gets the I2C_GP2_CLK pin.
    /// </summary>
    public IPin I2C_GP2_CLK => new Pin(Controller, "I2C_GP2_CLK", "PIN28",
        new IChannelInfo[]
        {
            new I2cChannelInfo("I2C_GP2_CLK", I2cChannelFunctionType.Clock, busNumber: 1)
        });

    /// <summary>
    /// Gets the CAN0_DIN pin.
    /// </summary>
    public IPin CAN0_DIN => new SysFsPin(Controller, "CAN0_DIN", "PIN29", 251);

    /// <summary>
    /// Gets the CAN0_DOUT pin.
    /// </summary>
    public IPin CAN0_DOUT => new SysFsPin(Controller, "CAN0_DOUT", "PIN31", 250);

    /// <summary>
    /// Gets the GPIO9_CAN1_GPIO0_DMIC_CLK pin.
    /// </summary>
    public IPin GPIO9_CAN1_GPIO0_DMIC_CLK => new SysFsPin(Controller, "GPIO9_CAN1_GPIO0_DMIC_CLK", "PIN32", 257);

    /// <summary>
    /// Gets the CAN1_DOUT pin.
    /// </summary>
    public IPin CAN1_DOUT => new SysFsPin(Controller, "CAN1_DOUT", "PIN33", 248);

    /// <summary>
    /// Gets the I2S_FS pin.
    /// </summary>
    public IPin I2S_FS => new SysFsPin(Controller, "I2S_FS", "PIN35", 354);

    /// <summary>
    /// Gets the UART1_CTS pin.
    /// </summary>
    public IPin UART1_CTS => new SysFsPin(Controller, "UART1_CTS", "PIN36", 429);

    /// <summary>
    /// Gets the CAN1_DIN pin.
    /// </summary>
    public IPin CAN1_DIN => new SysFsPin(Controller, "CAN1_DIN", "PIN37", 249);

    /// <summary>
    /// Gets the I2S_SDIN pin.
    /// </summary>
    public IPin I2S_SDIN => new SysFsPin(Controller, "I2S_SDIN", "PIN38", 353);

    /// <summary>
    /// Gets the I2S_SDOUT pin.
    /// </summary>
    public IPin I2S_SDOUT => new SysFsPin(Controller, "I2S_SDOUT", "PIN40", 352);

    // aliases for sanity
    /// <summary>
    /// Gets the MCLK05 pin.
    /// </summary>
    public IPin Pin7 => MCLK05;

    /// <summary>
    /// Gets the UART1_RTS pin.
    /// </summary>
    public IPin Pin11 => UART1_RTS;

    /// <summary>
    /// Gets the I2S2_CLK pin.
    /// </summary>
    public IPin Pin12 => I2S2_CLK;

    /// <summary>
    /// Gets the PWM01 pin.
    /// </summary>
    public IPin Pin13 => PWM01;

    /// <summary>
    /// Gets the GPIO27_PWM2 pin.
    /// </summary>
    public IPin Pin15 => GPIO27_PWM2;

    /// <summary>
    /// Gets the GPIO8_AO_DMIC_IN_DAT pin.
    /// </summary>
    public IPin Pin16 => GPIO8_AO_DMIC_IN_DAT;

    /// <summary>
    /// Gets the GPIO35_PWM3 pin.
    /// </summary>
    public IPin Pin18 => GPIO35_PWM3;

    /// <summary>
    /// Gets the SPI1_MOSI pin.
    /// </summary>
    public IPin Pin19 => SPI1_MOSI;

    /// <summary>
    /// Gets the SPI1_MISO pin.
    /// </summary>
    public IPin Pin21 => SPI1_MISO;

    /// <summary>
    /// Gets the GPIO17_40HEADER pin.
    /// </summary>
    public IPin Pin22 => GPIO17_40HEADER;

    /// <summary>
    /// Gets the SPI1_SCLK pin.
    /// </summary>
    public IPin Pin23 => SPI1_SCLK;

    /// <summary>
    /// Gets the SPI1_CS0 pin.
    /// </summary>
    public IPin Pin24 => SPI1_CS0;

    /// <summary>
    /// Gets the SPI1_CS1 pin.
    /// </summary>
    public IPin Pin26 => SPI1_CS1;

    /// <summary>
    /// Gets the CAN0_DIN pin.
    /// </summary>
    public IPin Pin29 => CAN0_DIN;

    /// <summary>
    /// Gets the CAN0_DOUT pin.
    /// </summary>
    public IPin Pin31 => CAN0_DOUT;

    /// <summary>
    /// Gets the GPIO9_CAN1_GPIO0_DMIC_CLK pin.
    /// </summary>
    public IPin Pin32 => GPIO9_CAN1_GPIO0_DMIC_CLK;

    /// <summary>
    /// Gets the CAN1_DOUT pin.
    /// </summary>
    public IPin Pin33 => CAN1_DOUT;

    /// <summary>
    /// Gets the I2S_FS pin.
    /// </summary>
    public IPin Pin35 => I2S_FS;

    /// <summary>
    /// Gets the UART1_CTS pin.
    /// </summary>
    public IPin Pin36 => UART1_CTS;

    /// <summary>
    /// Gets the CAN1_DIN pin.
    /// </summary>
    public IPin Pin37 => CAN1_DIN;

    /// <summary>
    /// Gets the I2S_SDIN pin.
    /// </summary>
    public IPin Pin38 => I2S_SDIN;

    /// <summary>
    /// Gets the I2S_SDOUT pin.
    /// </summary>
    public IPin Pin40 => I2S_SDOUT;
}
