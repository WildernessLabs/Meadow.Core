using System;
namespace Meadow.Hardware.Communications
{
    public interface II2cPeripheral : IByteCommunications
    {
        byte Address { get; }
    }
}
