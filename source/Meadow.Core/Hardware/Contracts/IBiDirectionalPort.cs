namespace Meadow.Hardware
{
    public interface IBiDirectionalPort : IDigitalInterruptPort, IDigitalOutputPort
    {
        new PortDirectionType Direction { get; set; }
    }
}
