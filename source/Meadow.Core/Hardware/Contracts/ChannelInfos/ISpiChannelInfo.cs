using System;
namespace Meadow.Hardware
{
    public interface ISpiChannelInfo : IDigitalChannelInfo
    {
        SpiLineType LineTypes { get; }
    }
}
