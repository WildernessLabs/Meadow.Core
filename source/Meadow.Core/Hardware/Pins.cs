using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// TODO: how to handle this. Each board will have a different set, but we should 
    /// commonize them as much as possible. I.e.; D1 is the same on all devices.
    /// </summary>
    public enum Pins : Byte
    {
        D01,
        D02,
        D03,
        D04,
        D05,
        A00,
        A01,
        A02,
        A03,
        A04,
        OnboardLEDRed,
        OnboardLEDGreen,
        OnboardLEDBlue,
        GPIO_NONE,
    }
}
