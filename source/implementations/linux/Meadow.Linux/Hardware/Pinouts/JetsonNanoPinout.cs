using Meadow.Hardware;

namespace Meadow.Pinouts;

/// <summary>
/// Defines the pinout configuration for an NVIDIA Jetson Nano
/// </summary>
public class JetsonNanoPinout : PinDefinitionBase, IPinDefinitions
{
    internal JetsonNanoPinout() { }

    /// <summary>
    /// Represents the I2C_2_SDA pin.
    /// </summary>
    public IPin I2C_2_SDA => new Pin(Controller, "I2C_2_SDA", "PIN03", null);
    /// <summary>
    /// Represents the I2C_2_SCL pin.
    /// </summary>
    public IPin I2C_2_SCL => new Pin(Controller, "I2C_2_SCL", "PIN05", null);

    /// <summary>
    /// Represents the D04 pin.
    /// </summary>
    public IPin D04 => new SysFsPin(Controller, "D04", "PIN07", 216);

    /// <summary>
    /// Represents the UART_2_TX pin.
    /// </summary>
    public IPin UART_2_TX => new Pin(Controller, "UART_2_TX", "PIN08", null);
    /// <summary>
    /// Represents the UART_2_RX pin.
    /// </summary>
    public IPin UART_2_RX => new Pin(Controller, "UART_2_RX", "PIN10", null);

    /// <summary>
    /// Represents the D17 pin.
    /// </summary>
    public IPin D17 => new SysFsPin(Controller, "D17", "PIN11", 50);
    /// <summary>
    /// Represents the D18 pin.
    /// </summary>
    public IPin D18 => new SysFsPin(Controller, "D18", "PIN12", 79);

    /// <summary>
    /// Represents the D27 pin.
    /// </summary>
    public IPin D27 => new SysFsPin(Controller, "D27", "PIN13", 14);
    /// <summary>
    /// Represents the D22 pin.
    /// </summary>
    public IPin D22 => new SysFsPin(Controller, "D22", "PIN15", 194);
    /// <summary>
    /// Represents the D23 pin.
    /// </summary>
    public IPin D23 => new SysFsPin(Controller, "D23", "PIN16", 232);
    /// <summary>
    /// Represents the D24 pin.
    /// </summary>
    public IPin D24 => new SysFsPin(Controller, "D24", "PIN18", 15);
    /// <summary>
    /// Represents the D10 pin.
    /// </summary>
    public IPin D10 => new SysFsPin(Controller, "D10", "PIN19", 16);
    /// <summary>
    /// Represents the D09 pin.
    /// </summary>
    public IPin D09 => new SysFsPin(Controller, "D09", "PIN21", 17);
    /// <summary>
    /// Represents the D25 pin.
    /// </summary>
    public IPin D25 => new SysFsPin(Controller, "D25", "PIN22", 13);
    /// <summary>
    /// Represents the D11 pin.
    /// </summary>
    public IPin D11 => new SysFsPin(Controller, "D11", "PIN23", 18);
    /// <summary>
    /// Represents the D08 pin.
    /// </summary>
    public IPin D08 => new SysFsPin(Controller, "D08", "PIN24", 19);
    /// <summary>
    /// Represents the D07 pin.
    /// </summary>
    public IPin D07 => new SysFsPin(Controller, "D07", "PIN26", 20);

    /// <summary>
    /// Represents the I2C_1_SDA pin.
    /// </summary>
    public IPin I2C_1_SDA => new Pin(Controller, "I2C_1_SDA", "PIN27", null);
    /// <summary>
    /// Represents the I2C_1_SCL pin.
    /// </summary>
    public IPin I2C_1_SCL => new Pin(Controller, "I2C_1_SCL", "PIN28", null);

    /// <summary>
    /// Represents the D05 pin.
    /// </summary>
    public IPin D05 => new SysFsPin(Controller, "D05", "PIN29", 149);
    /// <summary>
    /// Represents the D06 pin.
    /// </summary>
    public IPin D06 => new SysFsPin(Controller, "D06", "PIN31", 200);
    /// <summary>
    /// Represents the D12 pin.
    /// </summary>
    public IPin D12 => new SysFsPin(Controller, "D12", "PIN32", 168);
    /// <summary>
    /// Represents the D13 pin.
    /// </summary>
    public IPin D13 => new SysFsPin(Controller, "D13", "PIN33", 38);
    /// <summary>
    /// Represents the D19 pin.
    /// </summary>
    public IPin D19 => new SysFsPin(Controller, "D19", "PIN35", 76);
    /// <summary>
    /// Represents the D16 pin.
    /// </summary>
    public IPin D16 => new SysFsPin(Controller, "D16", "PIN36", 51);
    /// <summary>
    /// Represents the D26 pin.
    /// </summary>
    public IPin D26 => new SysFsPin(Controller, "D26", "PIN37", 12);
    /// <summary>
    /// Represents the D20 pin.
    /// </summary>
    public IPin D20 => new SysFsPin(Controller, "D20", "PIN38", 77);
    /// <summary>
    /// Represents the D21 pin.
    /// </summary>
    public IPin D21 => new SysFsPin(Controller, "D21", "PIN40", 78);
}
