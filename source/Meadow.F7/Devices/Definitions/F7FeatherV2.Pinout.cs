using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Devices
{
    public partial class F7FeatherV2
    {
        private const int ADCPrecisionBits = 12;

        public partial class Pinout : IF7FeatherPinout
        {
            public IList<IPin> AllPins => new List<IPin> {
                // left header
                A00, A01, A02, A03, A04, A05, SCK, COPI, CIPO, D00, D01, D02, D03, D04,
                // right header
                D05, D06, D07, D08, D09, D10, D11, D12, D13, D14, D15,
                // Onboard LED
                OnboardLedRed, OnboardLedGreen, OnboardLedBlue,
                // ESP stuff TODO: Consider removing these from the `AllPins` list.
                ESP_COPI, ESP_CIPO, ESP_CLK, ESP_CS, ESP_BOOT, ESP_RST, ESP_UART_RX, ESP_UART_TX,
                I2C_SCL, I2C_SDA
            };

            //==== LED

            // OnboardLedBlue
            // TIM2_CH1/TIM2_ETR, TIM5_CH1, TIM8_ETR, USART2_CTS, UART4_TX, SAI2_SD_B, ETH_MII_CRS, EVENTOUT
            // ADC1_IN0, ADC2_IN0, ADC3_IN0, WKUP1
            public IPin OnboardLedBlue => new Pin(
                "OnboardLedBlue", "PA0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA0", inverseLogic: true, interruptGroup: 0),
                    new PwmChannelInfo("TIM2_CH1", 2, 1)
                }
            );
            // OnboardLedGreen
            // TIM2_CH2, TIM5_CH2, USART2_RTS, UART4_RX, QUADSPI_BK1_IO3, SAI2_MCLK_B, ETH_MII_RX_CLK/ETH_R MII_REF_CLK, LCD_R2, EVENTOUT
            // ADC1_IN1, ADC2_IN1, ADC3_IN1
            public IPin OnboardLedGreen => new Pin(
                "OnboardLedGreen", "PA1",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA1", inverseLogic: true, interruptGroup: 1),
                    new PwmChannelInfo("TIM2_CH2", 2, 2)
                }
            );
            // OnboardLedRed
            // TIM2_CH3, TIM5_CH3, TIM9_CH1, USART2_TX, SAI2_SCK_B, ETH_MDIO, MDIOS_MDIO, LCD_R1, EVENTOUT
            // ADC1_IN2, ADC2_IN2, ADC3_IN2, WKUP2
            public IPin OnboardLedRed => new Pin(
                "OnboardLedRed", "PA2",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA2", inverseLogic: true, interruptGroup: 2),
                    new PwmChannelInfo("TIM2_CH3", 2, 3)
                }
            );

            //==== Left Header

            // A00
            // SPI1_NSS/I2S1_WS, SPI3_NSS/I2S3_WS, USART2_CK, SPI6_NSS, OTG_HS_SOF, DCMI_HSYNC, LCD_VSYNC, EVENTOUT
            // ADC1_IN4, ADC2_IN4, DAC_OUT1
            public IPin A00 => new Pin(
                "A00", "PA4",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA4", interruptGroup: 4),
                    new AnalogChannelInfo("ADC1_IN4", ADCPrecisionBits, true, false)
                }
            );
            // A01
            // TIM2_CH1/TIM2_ETR, TIM8_CH1N, SPI1_SCK/I2S1_CK, SPI6_SCK, OTG_HS_ULPI_CK, LCD_R4, EVENTOUT
            // ADC1_IN5, ADC2_IN5, DAC_OUT2
            public IPin A01 => new Pin(
                "A01", "PA5",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA5", interruptGroup: 5),
                    new AnalogChannelInfo("ADC1_IN5", ADCPrecisionBits, true, false)
                }
            );
            // A02
            // TIM2_CH4, TIM5_CH4, TIM9_CH2, USART2_RX, LCD_B2, OTG_HS_ULPI_D0, ETH_MII_COL, LCD_B5, EVENTOUT
            // ADC1_IN3, ADC2_IN3, ADC3_IN3
            public IPin A02 => new Pin(
                "A02", "PA3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA3", interruptGroup: 3),
                    new AnalogChannelInfo("ADC1_IN3", ADCPrecisionBits, true, false)
                }
            );
            // A03
            // TIM1_CH2N, TIM3_CH3, TIM8_CH2N, DFSDM_CKOUT, UART4_CTS, LCD_R3, OTG_HS_ULPI_D1, ETH_MII_RXD2, LCD_G1, EVENTOUT
            // ADC1_IN8, ADC2_IN8
            public IPin A03 => new Pin(
                "A03", "PB0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB0", interruptGroup: 0),
                    new AnalogChannelInfo("ADC1_IN8", ADCPrecisionBits, true, false) // TODO: should we move this to the second ADC?
                }
            );
            // A04
            // TIM1_CH3N, TIM3_CH4, TIM8_CH3N, DFSDM_DATIN1, LCD_R6, OTG_HS_ULPI_D2, ETH_MII_RXD3, LCD_G0, EVENTOUT
            // ADC1_IN9, ADC2_IN9
            public IPin A04 => new Pin(
                "A04", "PB1",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB1", interruptGroup: 1),
                    new AnalogChannelInfo("ADC1_IN9", ADCPrecisionBits, true, false)
                }
            );
            // A05
            // DFSDM_CKIN0, DFSDM_DATIN4, SAI2_FS_B, OTG_HS_ULPI_STP, FMC_SDNWE, LCD_R5, EVENTOUT
            // ADC1_IN10, ADC2_IN10, ADC3_IN10
            public IPin A05 => new Pin(
                "A05", "PC0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC0", interruptGroup: 0),
                    new AnalogChannelInfo("ADC1_IN10", ADCPrecisionBits, true, false)
                }
            );
            // SCK
            // SPI3_CLK
            // DFSDM_CKIN5, SPI3_SCK/I2S3_CK, USART3_TX, UART4_TX, QUADSPI_BK1_IO1, SDMMC1_D2, DCMI_D8, LCD_R2, EVENTOUT
            public IPin SCK => new Pin(
                "SCK", "PC10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC10", interruptCapable: false),
                    new SpiChannelInfo("PC10", SpiLineType.Clock)
                }
            );
            // COPI
            // SPI3_MOSI
            // UART5_RX, TIM3_CH2, I2C1_SMBA, SPI1_MOSI/I2S1_SD, SPI3_MOSI/I2S3_SD, SPI6_MOSI, CAN2_RX, OTG_HS_ULPI_D7, ETH_PPS_OUT, FMC_SDCKE1, DCMI_D10, LCD_G7, EVENTOUT
            public IPin MOSI => COPI; // TODO: let the interface handle this when we get to .NET Standard 2.1
            public IPin COPI => new Pin(
                "COPI", "PB5",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB5", interruptGroup: 5),
                    new SpiChannelInfo("PB5", SpiLineType.MOSI)
                }
            );
            // CIPO
            // SPI3_MISO
            public IPin MISO => CIPO; // TODO: let the interface handle this when we get to .NET Standard 2.1
            public IPin CIPO => new Pin(
                "CIPO", "PC11",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC11", interruptGroup: 11),
                    new SpiChannelInfo("PC11", SpiLineType.MISO)
                }
            );

            // D00
            // UART4_RX, CAN1_RX, FMC_D30, LCD_VSYNC, EVENTOUT
            public IPin D00 => new Pin(
                "D00", "PI9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI9", interruptGroup: 9),
                    new UartChannelInfo("UART4_RX", SerialDirectionType.Receive)
                }
            );
            // D01
            // TIM8_CH1N, UART4_TX, CAN1_TX, FMC_D21, LCD_G2, EVENTOUT
            public IPin D01 => new Pin(
                "D01", "PH13",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH13", interruptGroup: 13),
                    new UartChannelInfo("UART4_TX", SerialDirectionType.Transmit)
                }
            );
            // D02
            // TIM5_CH1, I2C4_SMBA, FMC_D18, DCMI_D1, LCD_R4, EVENTOUT
            public IPin D02 => new Pin(
                "D02", "PH10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH10", interruptCapable: false),
                    new PwmChannelInfo("TIM5_CH1", 5, 1)
                }
            );
            // D03
            // I2C4_SCL, TIM4_CH3, TIM10_CH1, I2C1_SCL, DFSDM_CKIN7, UART5_RX, CAN1_RX, SDMMC2_D4, ETH_MII_TXD3, SDMMC1_D4, DCMI_D6, LCD_B6, EVENTOUT
            public IPin D03 => new Pin(
                "D03", "PB8",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB8", interruptGroup: 8),
                    new PwmChannelInfo("TIM4_CH3", 4, 3),
                    new CanChannelInfo("CAN1_RX", SerialDirectionType.Receive)
                }
            );
            // D04
            // I2C4_SDA, TIM4_CH4, TIM11_CH1, I2C1_SDA, SPI2_NSS/I2S2_WS, DFSDM_DATIN7, UART5_TX, CAN1_TX, SDMMC2_D5, I2C4_SMBA, SDMMC1_D5, DCMI_D7, LCD_B7, EVENTOUT
            public IPin D04 => new Pin(
                "D04", "PB9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB9", interruptGroup: 9),
                    new PwmChannelInfo("TIM4_CH4", 4, 4),
                    new CanChannelInfo("CAN1_TX", SerialDirectionType.Transmit)
                }
            );

            //==== Right Header

            // D05 //NOTE: we can expose UART_7 if we want here
            // F7v2.a
            // JTDI, TIM2_CH1/TIM2_ETR, HDMI_CEC, SPI1_NSS/I2S1_WS, SPI3_NSS/I2S3_WS, SPI6_NSS, UART4_RTS, CAN3_TX, UART7_TX, EVENTOUT
            //public IPin D05 => new Pin(
            //    "D05", "PA15",
            //    new List<IChannelInfo> {
            //        new DigitalChannelInfo("PA15", interruptGroup: 15),
            //        //new PwmChannelInfo("TIM2_CH1", 1, 1), // Reserved for OnboardLED
            //        new UartChannelInfo("UART7_RX", SerialDirectionType.Receive)
            //    }
            //);
            // F7v2.b
            // NJTRST, TIM3_CH1, SPI1_MISO, SPI3_MISO, SPI2_NSS/I2S2_WS, SPI6_MISO, SDMMC2_D3, CAN3_TX, UART7_TX, EVENTOUT
            public IPin D05 => new Pin(
                "D05", "PB4",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB4", interruptGroup: 4),
                    new PwmChannelInfo("TIM3_CH1", 3, 1),
                    //new UartChannelInfo("UART7_TX", SerialDirectionType.Transmit)
                }
            );

            // D06 //NOTE: we can expose UART_7 if we want here
            // F7v2.a
            // JTDO/TRACESWO, TIM2_CH2, SPI1_SCK/I2S1_CK, SPI3_SCK/I2S3_CK, SPI6_SCK, SDMMC2_D2, CAN3_RX, UART7_RX, EVENTOUT
            //public IPin D06 => new Pin(
            //    "D06", "PB3",
            //    new List<IChannelInfo> {
            //        new DigitalChannelInfo("PB3", interruptGroup: 3),
            //        //new PwmChannelInfo("TIM2_CH2", 2, 2), // Reserved for OnboardLED
            //        new UartChannelInfo("UART7_TX", SerialDirectionType.Transmit)
            //    }
            //);
            // F7v2.b
            // I2C2_SMBA, SPI5_SCK, TIM12_CH1, ETH_MII_RXD2, FMC_SDNE1, DCMI_D8, EVENTOUT
            public IPin D06 => new Pin(
                "D06", "PB13",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB13", interruptGroup: 13),
                    //new PwmChannelInfo("TIM1_CH1", 1, 1),
                    //new UartChannelInfo("UART7_RX", SerialDirectionType.Receive)
                }
            );

            // D07
            // TIM4_CH2, I2C1_SDA, DFSDM_CKIN5, USART1_RX, I2C4_SDA, FMC_NL, DCMI_VSYNC, EVENTOUT
            public IPin D07 => new Pin(
                "D07", "PB7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB7", interruptGroup: 7),
                    new PwmChannelInfo("TIM4_CH2", 4, 2),
                    new I2cChannelInfo("I2C1_SDA", I2cChannelFunctionType.Data) // or I2C4_SDA
                }
            );


            // D08
            // UART5_TX, TIM4_CH1, HDMI_CEC, I2C1_SCL, DFSDM_DATIN5, USART1_TX, CAN2_TX, QUADSPI_BK1_NCS, I2C4_SCL, FMC_SDNE1, DCMI_D5, EVENTOUT
            public IPin D08 => new Pin(
                "D08", "PB6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB6", interruptGroup: 6),
                    new PwmChannelInfo("TIM4_CH1", 4, 1),
                    new I2cChannelInfo("I2C1_SCL", I2cChannelFunctionType.Clock ) // or I2C4_SCL
                }
            );
            // D09
            // TIM3_CH1, TIM8_CH1, I2S2_MCK, DFSDM_CKIN3, USART6_TX, FMC_NWAIT, SDMMC2_D6, SDMMC1_D6, DCMI_D0, LCD_HSYNC, EVENTOUT
            public IPin D09 => new Pin(
                "D09", "PC6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC6", interruptGroup: 6),
                    new PwmChannelInfo("TIM8_CH1", 8, 1), // or TIM3_CH1 (see D05)
                }
            );
            // D10
            // TIM3_CH2, TIM8_CH2, I2S3_MCK, DFSDM_DATIN3, USART6_RX, FMC_NE1, SDMMC2_D7, SDMMC1_D7, DCMI_D1, LCD_G6, EVENTOUT
            public IPin D10 => new Pin(
                "D10", "PC7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC7", interruptGroup: 7),
                    new PwmChannelInfo("TIM3_CH2", 3, 2) // or TIM8_CH2
                }
            );
            // D11
            // MCO2, TIM3_CH4, TIM8_CH4, I2C3_SDA, I2S_CKIN, UART5_CTS, QUADSPI_BK1_IO0, LCD_G3, SDMMC1_D1, DCMI_D3, LCD_B2, EVENTOUT   
            public IPin D11 => new Pin(
                "D11", "PC9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC9", interruptGroup: 9),
                    new PwmChannelInfo("TIM8_CH4", 8, 4)
                }
            );
            // D12
            // TIM1_CH2N, TIM8_CH2N, USART1_TX, SPI2_MISO, DFSDM_DATIN2, USART3_RTS, UART4_RTS, TIM12_CH1, SDMMC2_D0, OTG_HS_DM, EVENTOUT
            public IPin D12 => new Pin(
                "D12", "PB14",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB14", interruptGroup: 14),
                    new PwmChannelInfo("TIM12_CH1", 12, 1),
                    new UartChannelInfo("USART1_TX", SerialDirectionType.Transmit)
                }
            );
            // D13
            // RTC_REFIN, TIM1_CH3N, TIM8_CH3N, USART1_RX, SPI2_MOSI/I2S2_SD, DFSDM_CKIN2, UART4_CTS, TIM12_CH2, SDMMC2_D1, OTG_HS_DP, EVENTOUT
            public IPin D13 => new Pin(
                "D13", "PB15",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB15", interruptGroup: 15),
                    new PwmChannelInfo("TIM12_CH2", 12, 2),
                    new UartChannelInfo("USART1_RX", SerialDirectionType.Receive)
                }
            );
            // D14
            // TIM1_BKIN, I2C2_SMBA, SPI2_NSS/I2S2_WS, DFSDM_DATIN1, USART3_CK, UART5_RX, CAN2_RX, OTG_HS_ULPI_D5, ETH_MII_TXD0/ETH_RMII _TXD0, OTG_HS_ID, EVENTOUT
            public IPin D14 => new Pin(
                "D14", "PB12",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB12", interruptGroup: 12),
                    //new PwmChannelInfo("TIM12_CH2", 12, 2),
                }
            );
            // D15
            // F7v2.a
            // I2C2_SMBA, SPI5_SCK, TIM12_CH1, ETH_MII_RXD2, FMC_SDNE1, DCMI_D8, EVENTOUT
            //public IPin D15 => new Pin(
            //    "D15", "PB13",
            //    new List<IChannelInfo> {
            //        new DigitalChannelInfo("PB13", interruptGroup: 13),
            //        new PwmChannelInfo("TIM12_CH1", 12, 1),
            //    }
            //);

            // F7v2.b
            // LPTIM1_IN1, SPI6_MISO, SPDIF_RX1, USART6_RTS, LCD_B4, SDMMC2_D3, FMC_NE4, LCD_B1, EVENTOUT
            public IPin D15 => new Pin(
                "D15", "PG12",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PG12", interruptGroup: 12),
                    //new PwmChannelInfo("TIM12_CH1", 12, 1),
                }
            );

            //==== ESP Coprocessor
            // TODO: if C# ever allows it, it would be nice to make these internal
            // even though they are on the interface

            // ESP_MOSI
            public IPin ESP_COPI => new Pin(
                "ESP_MOSI", "PI3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI3"),
                    new SpiChannelInfo("PI3", SpiLineType.MOSI)
                }
            );
            // ESP_MISO
            public IPin ESP_CIPO => new Pin(
                "ESP_MISO", "PI2",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI2", interruptGroup: 12),
                    new SpiChannelInfo("PI2", SpiLineType.MISO)
                }
            );
            // ESP_CLK
            public IPin ESP_CLK => new Pin(
                "ESP_CLK", "PD3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PD3", interruptGroup: 3),
                    new SpiChannelInfo("PD3", SpiLineType.Clock)
                }
            );
            // ESP_CS
            public IPin ESP_CS => new Pin(
                "ESP_CS", "PI0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI0", interruptGroup: 0),
                    new SpiChannelInfo("PI10", SpiLineType.ChipSelect)
                }
            );
            // ESP_BOOT
            public IPin ESP_BOOT => new Pin(
                "ESP_BOOT", "PI10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI10", interruptCapable: false),
                }
            );
            // ESP_RST
            public IPin ESP_RST => new Pin(
                "ESP_RST", "PF7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PF7", interruptGroup: 7),
                }
            );
            // ESP_UART_RX
            public IPin ESP_UART_RX => new Pin(
                "ESP_UART_RX", "PD2",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PD2", interruptGroup: 2),
                }
            );
            // NOTE: This changed in v2
            // ESP_UART_TX
            public IPin ESP_UART_TX => new Pin(
                "ESP_UART_TX", "PC12",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC12", interruptGroup: 12),
                }
            );

            // TODO: let the interface handle this when we get to .NET Standard 2.1
            public IPin I2C_SDA => D07;
            public IPin I2C_SCL => D08;


            public Pinout()
            {
                //Groups = new F7NamedPinGroups(this); //TODO: This causes a NullReferenceException
            }

            public IEnumerator<IPin> GetEnumerator() => AllPins.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
