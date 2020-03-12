using System;
namespace Meadow.Hardware
{
    public interface II2cPeripheral : IByteCommunications
    {
        byte Address { get; }
        II2cBus Bus { get; }
    }
}
