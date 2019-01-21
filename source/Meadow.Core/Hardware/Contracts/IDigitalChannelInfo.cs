using System;
namespace Meadow.Hardware
{
    public interface IDigitalChannelInfo
    {
        bool InterrruptCapable { get; }
        bool PullDownCapable { get; }
        bool PullUpCapable { get; }
    }
}
