using System;
namespace Meadow.Hardware
{
    [Flags]
    public enum SpiLineType
    {
        None = 0,
        MOSI = 1 << 0,
        MISO = 1 << 1,
        Clock = 1 << 2,
        ChipSelect = 1 << 3
    }
}
