using System;
namespace Meadow.Hardware
{
    public interface IDigitalChannelInfo : IChannelInfo
    {
        bool InterrruptCapable { get; }
        bool PullDownCapable { get; }
        bool PullUpCapable { get; }
    }
}
