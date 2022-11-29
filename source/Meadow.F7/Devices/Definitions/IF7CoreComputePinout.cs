namespace Meadow.Hardware
{
    public interface IF7CoreComputePinout : I32PinFeatherBoardPinout, IEspCoprocessorPinout, IPinDefinitions
    {
        IPin I2C1_SCL { get; }
        IPin I2C1_SDA { get; }
        IPin I2C3_SCL { get; }
        IPin I2C3_SDA { get; }
        IPin SPI3_SCK { get; }
        IPin SPI3_COPI { get; }
        IPin SPI3_CIPO { get; }
        IPin SPI5_SCK { get; }
        IPin SPI5_COPI { get; }
        IPin SPI5_CIPO { get; }
        IPin D16 { get; }
        IPin D17 { get; }
        IPin D18 { get; }
        IPin D19 { get; }
        IPin D20 { get; }
    }
}
