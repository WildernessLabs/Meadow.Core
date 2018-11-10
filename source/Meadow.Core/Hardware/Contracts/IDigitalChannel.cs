using System;
namespace Meadow.Hardware
{
    public interface IDigitalChannel
    {
        bool InterrruptCapable { get; }
        bool PullDownCapable { get; }
        bool PullUpCapable { get; }
    }
}
