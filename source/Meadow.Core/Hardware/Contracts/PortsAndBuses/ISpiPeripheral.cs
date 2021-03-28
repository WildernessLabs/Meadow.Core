using System;
namespace Meadow.Hardware
{
    public interface ISpiPeripheral : IByteCommunications
    {
        IDigitalOutputPort ChipSelect { get; }
        ISpiBus Bus { get; }
    }
}
