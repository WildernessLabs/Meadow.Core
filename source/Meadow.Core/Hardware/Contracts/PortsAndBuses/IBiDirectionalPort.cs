namespace Meadow.Hardware
{
    public interface IBiDirectionalPort : IDigitalInterruptPort, IDigitalOutputPort
    {
        PortDirectionType Direction { get; set; }
    }
}
