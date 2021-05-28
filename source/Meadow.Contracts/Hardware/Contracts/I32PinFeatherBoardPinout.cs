using System;
namespace Meadow.Hardware
{
    public interface I32PinFeatherBoardPinout// : IPinDefinitions
    {
        //==== LED
        IPin OnboardLedBlue { get; }
        IPin OnboardLedGreen { get; }
        IPin OnboardLedRed { get; }

        //==== Left Header
        IPin A00 { get; }
        IPin A01 { get; }
        IPin A02 { get; }
        IPin A03 { get; }
        IPin A04 { get; }
        IPin A05 { get; }
        IPin SCK { get; }
        public IPin MOSI => COPI;
        IPin COPI { get; }
        public IPin MISO => CIPO;
        IPin CIPO { get; }
        IPin D01 { get; }
        IPin D00 { get; }
        IPin D02 { get; }
        IPin D03 { get; }
        IPin D04 { get; }

        //==== Right Header
        IPin D05 { get; }
        IPin D06 { get; }
        IPin D07 { get; }
        IPin D08 { get; }
        IPin D09 { get; }
        IPin D10 { get; }
        IPin D11 { get; }
        IPin D12 { get; }
        IPin D13 { get; }
        IPin D14 { get; }
        IPin D15 { get; }

        //==== Convenience pins
        IPin I2C_SDA => D08;
        IPin I2C_SCL => D07;


    }
}
