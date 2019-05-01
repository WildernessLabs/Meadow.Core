using System;
namespace Meadow.Hardware.Communications
{
    public interface ISpiPeripheral : IReadWriteByteCommunications
    {
        IDigitalOutputPort ChipSelect { get; }
    }
}
