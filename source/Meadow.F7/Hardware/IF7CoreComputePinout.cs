namespace Meadow.Hardware
{
    public interface IF7CoreComputePinout : I32PinFeatherBoardPinout, IEspCoprocessorPinout, IPinDefinitions
    {
        IPin LED1 { get; }
        IPin I2C1_SCL { get; }
        IPin I2C1_SDA { get; }
        IPin I2C3_SCL { get; }
        IPin I2C3_SDA { get; }
    }
}
