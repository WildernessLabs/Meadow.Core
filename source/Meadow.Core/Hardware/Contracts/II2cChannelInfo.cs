using System;
namespace Meadow.Hardware
{
    public enum I2cChannelFunctionType
    {
        Data,
        Clock
    }

    public interface II2cChannelInfo : IDigitalChannelInfo, ICommunicationChannelInfo
    {
        I2cChannelFunctionType ChannelFunction { get; }
    }
}
