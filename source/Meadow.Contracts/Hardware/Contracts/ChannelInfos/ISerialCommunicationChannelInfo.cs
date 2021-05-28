using System;
namespace Meadow.Hardware
{
    public interface ISerialCommunicationChannelInfo : ICommunicationChannelInfo
    {
        SerialDirectionType SerialDirection { get; }
    }
}
