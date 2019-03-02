using System;
namespace Meadow.Hardware
{
    public interface ISerialCommunicationChannel
    {
        SerialDirectionType SerialDirection { get; }
    }
}
