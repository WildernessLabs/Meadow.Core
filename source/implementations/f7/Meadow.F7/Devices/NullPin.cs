using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow.Devices;

internal class NullPin : IPin
{
    public IPinController? Controller => default!;
    public IList<IChannelInfo>? SupportedChannels => null;

    public string Name => "NULL";

    public object Key => default!;

    public bool Equals(IPin other)
    {
        if (other is NullPin) return true;

        return false;
    }
}


/* ===== MEADOW GPIO PIN MAP =====
    BOARD PIN   SCHEMATIC       CPU PIN   MDW NAME  ALT FN   INT GROUP
    J301-1      RESET                                           - 
    J301-2      3.3                                             - 
    J301-3      VREF                                            - 
    J301-4      GND                                             - 
    J301-5      DAC_OUT1        PA4         A0                  4
    J301-6      DAC_OUT2        PA5         A1                  5
    J301-7      ADC1_IN3        PA3         A2                  3
    J301-8      ADC1_IN7        PA7         A3                  7
    J301-9      ADC1_IN10       PC0         A4                  0
    J301-10     ADC1_IN11       PC1         A5                  1
    J301-11     SPI3_CLK        PC10        SCK                 10
    J301-12     SPI3_MOSI       PB5         MOSI    AF6         5
    J301-13     SPI3_MISO       PC11        MISO    AF6         11
    J301-14     UART4_RX        PI9         D00     AF8         9
    J301-15     UART4_TX        PH13        D01     AF8         13
    J301-16     PC6             PC6         D02                 6
    J301-17     CAN1_RX         PB8         D03     AF9         8
    J301-18     CAN1_TX         PB9         D04     AF9         9

    J302-4      PE3             PE3         D15                 3
    J302-5      PG3             PG3         D14                 3
    J302-6      USART1_RX       PB15        D13     AF4         15
    J302-7      USART1_TX       PB14        D12     AF4         14
    J302-8      PC9             PC9         D11                 9
    J302-9      PH10            PH10        D10                 10
    J302-10     PB1             PB1         D09                 1
    J302-11     I2C1_SCL        PB6         D08     AF4         6
    J302-12     I2C1_SDA        PB7         D07     AF4         7
    J302-13     PB0             PB0         D06                 0
    J302-14     PC7             PC7         D05                 7

    LED_B       PA0
    LED_G       PA1
    LED_R       PA2
*/
