using System;
namespace Meadow.Hardware
{
    public interface ISerialCommunicationChannelInfo
    {
        SerialDirectionType SerialDirection { get; }
    }
}
