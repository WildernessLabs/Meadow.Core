using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow.Devices
{
    public partial class F7FeatherV2
    {
        private const int ADCPrecisionBits = 12;
        /// <summary>
        /// Defines the pinout for the Meadow F7 v2 device.
        /// </summary>
        public partial class Pinout : PinDefinitionBase, IF7FeatherPinout
        {
            private IPin? _blue;
            private IPin? _green;
            private IPin? _red;
            private IPin? _a00;
            private IPin? _a01;
            private IPin? _a02;
            private IPin? _a03;
            private IPin? _a04;
            private IPin? _a05;
            private IPin? _sck;
            private IPin? _copi;
            private IPin? _cipo;
            private IPin? _d00;
            private IPin? _d01;
            private IPin? _d02;
            private IPin? _d03;
            private IPin? _d04;
            private IPin? _d05;
            private IPin? _d06;
            private IPin? _d07;
            private IPin? _d08;
            private IPin? _d09;
            private IPin? _d10;
            private IPin? _d11;
            private IPin? _d12;
            private IPin? _d13;
            private IPin? _d14;
            private IPin? _d15;

            internal Pinout() { }

            //==== LED

            // OnboardLedBlue
            // TIM2_CH1/TIM2_ETR, TIM5_CH1, TIM8_ETR, USART2_CTS, UART4_TX, SAI2_SD_B, ETH_MII_CRS, EVENTOUT
            // ADC1_IN0, ADC2_IN0, ADC3_IN0, WKUP1
            /// <inheritdoc/>
            public IPin OnboardLedBlue => _blue ??= new Pin(
                Controller,
                "OnboardLedBlue", "PA0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA0", inverseLogic: true, interruptGroup: 0),
                    new PwmChannelInfo("TIM2_CH1", 2, 1)
                }
            );

            // OnboardLedGreen
            // TIM2_CH2, TIM5_CH2, USART2_RTS, UART4_RX, QUADSPI_BK1_IO3, SAI2_MCLK_B, ETH_MII_RX_CLK/ETH_R MII_REF_CLK, LCD_R2, EVENTOUT
            // ADC1_IN1, ADC2_IN1, ADC3_IN1
            /// <inheritdoc/>
            public IPin OnboardLedGreen => _green ??= new Pin(
                Controller,
                "OnboardLedGreen", "PA1",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA1", inverseLogic: true, interruptGroup: 1),
                    new PwmChannelInfo("TIM2_CH2", 2, 2)
                }
            );
            // OnboardLedRed
            // TIM2_CH3, TIM5_CH3, TIM9_CH1, USART2_TX, SAI2_SCK_B, ETH_MDIO, MDIOS_MDIO, LCD_R1, EVENTOUT
            // ADC1_IN2, ADC2_IN2, ADC3_IN2, WKUP2
            /// <inheritdoc/>
            public IPin OnboardLedRed => _red ??= new Pin(
                Controller,
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
            /// <inheritdoc/>
            public IPin A00 => _a00 ??= new Pin(
                Controller,
                "A00", "PA4",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA4", interruptGroup: 4),
                    new AnalogChannelInfo("ADC1_IN4", ADCPrecisionBits, true, false)
                }
            );
            // A01
            // TIM2_CH1/TIM2_ETR, TIM8_CH1N, SPI1_SCK/I2S1_CK, SPI6_SCK, OTG_HS_ULPI_CK, LCD_R4, EVENTOUT
            // ADC1_IN5, ADC2_IN5, DAC_OUT2
            /// <inheritdoc/>
            public IPin A01 => _a01 ??= new Pin(
                Controller,
                "A01", "PA5",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA5", interruptGroup: 5),
                    new AnalogChannelInfo("ADC1_IN5", ADCPrecisionBits, true, false)
                }
            );
            // A02
            // TIM2_CH4, TIM5_CH4, TIM9_CH2, USART2_RX, LCD_B2, OTG_HS_ULPI_D0, ETH_MII_COL, LCD_B5, EVENTOUT
            // ADC1_IN3, ADC2_IN3, ADC3_IN3
            /// <inheritdoc/>
            public IPin A02 => _a02 ??= new Pin(
                Controller,
                "A02", "PA3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA3", interruptGroup: 3),
                    new AnalogChannelInfo("ADC1_IN3", ADCPrecisionBits, true, false)
                }
            );
            // A03
            // TIM1_CH2N, TIM3_CH3, TIM8_CH2N, DFSDM_CKOUT, UART4_CTS, LCD_R3, OTG_HS_ULPI_D1, ETH_MII_RXD2, LCD_G1, EVENTOUT
            // ADC1_IN8, ADC2_IN8
            /// <inheritdoc/>
            public IPin A03 => _a03 ??= new Pin(
                Controller,
                "A03", "PB0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB0", interruptGroup: 0),
                    new AnalogChannelInfo("ADC1_IN8", ADCPrecisionBits, true, false) // TODO: should we move this to the second ADC?
                }
            );
            // A04
            // TIM1_CH3N, TIM3_CH4, TIM8_CH3N, DFSDM_DATIN1, LCD_R6, OTG_HS_ULPI_D2, ETH_MII_RXD3, LCD_G0, EVENTOUT
            // ADC1_IN9, ADC2_IN9
            /// <inheritdoc/>
            public IPin A04 => _a04 ??= new Pin(
                Controller,
                "A04", "PB1",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB1", interruptGroup: 1),
                    new AnalogChannelInfo("ADC1_IN9", ADCPrecisionBits, true, false)
                }
            );
            // A05
            // DFSDM_CKIN0, DFSDM_DATIN4, SAI2_FS_B, OTG_HS_ULPI_STP, FMC_SDNWE, LCD_R5, EVENTOUT
            // ADC1_IN10, ADC2_IN10, ADC3_IN10
            /// <inheritdoc/>
            public IPin A05 => _a05 ??= new Pin(
                Controller,
                "A05", "PC0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC0", interruptGroup: 0),
                    new AnalogChannelInfo("ADC1_IN10", ADCPrecisionBits, true, false)
                }
            );
            // SCK
            // SPI3_CLK
            // DFSDM_CKIN5, SPI3_SCK/I2S3_CK, USART3_TX, UART4_TX, QUADSPI_BK1_IO1, SDMMC1_D2, DCMI_D8, LCD_R2, EVENTOUT
            /// <inheritdoc/>
            public IPin SCK => _sck ??= new Pin(
                Controller,
                "SCK", "PC10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC10", interruptCapable: false),
                    new SpiChannelInfo("PC10", SpiLineType.Clock)
                }
            );
            // COPI
            // SPI3_MOSI
            // UART5_RX, TIM3_CH2, I2C1_SMBA, SPI1_MOSI/I2S1_SD, SPI3_MOSI/I2S3_SD, SPI6_MOSI, CAN2_RX, OTG_HS_ULPI_D7, ETH_PPS_OUT, FMC_SDCKE1, DCMI_D10, LCD_G7, EVENTOUT
            /// <inheritdoc/>
            public IPin MOSI => COPI; // TODO: let the interface handle this when we get to .NET Standard 2.1
            /// <inheritdoc/>
            public IPin COPI => _copi ??= new Pin(
                Controller,
                "COPI", "PB5",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB5", interruptGroup: 5),
                    new SpiChannelInfo("PB5", SpiLineType.COPI)
                }
            );
            // CIPO
            // SPI3_MISO
            /// <inheritdoc/>
            public IPin MISO => CIPO; // TODO: let the interface handle this when we get to .NET Standard 2.1
            /// <inheritdoc/>
            public IPin CIPO => _cipo ??= new Pin(
                Controller,
                "CIPO", "PC11",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC11", interruptGroup: 11),
                    new SpiChannelInfo("PC11", SpiLineType.CIPO)
                }
            );

            // D00
            // UART4_RX, CAN1_RX, FMC_D30, LCD_VSYNC, EVENTOUT
            /// <inheritdoc/>
            public IPin D00 => _d00 ??= new Pin(
                Controller,
                "D00", "PI9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI9", interruptGroup: 9),
                    new UartChannelInfo("UART4_RX", SerialDirectionType.Receive)
                }
            );
            // D01
            // TIM8_CH1N, UART4_TX, CAN1_TX, FMC_D21, LCD_G2, EVENTOUT
            /// <inheritdoc/>
            public IPin D01 => _d01 ??= new Pin(
                Controller,
                "D01", "PH13",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH13", interruptGroup: 13),
                    new UartChannelInfo("UART4_TX", SerialDirectionType.Transmit)
                }
            );
            // D02
            // TIM5_CH1, I2C4_SMBA, FMC_D18, DCMI_D1, LCD_R4, EVENTOUT
            /// <inheritdoc/>
            public IPin D02 => _d02 ??= new Pin(
                Controller,
                "D02", "PH10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH10", interruptCapable: false),
                    new PwmChannelInfo("TIM5_CH1", 5, 1)
                }
            );
            // D03
            // I2C4_SCL, TIM4_CH3, TIM10_CH1, I2C1_SCL, DFSDM_CKIN7, UART5_RX, CAN1_RX, SDMMC2_D4, ETH_MII_TXD3, SDMMC1_D4, DCMI_D6, LCD_B6, EVENTOUT
            /// <inheritdoc/>
            public IPin D03 => _d03 ??= new Pin(
                Controller,
                "D03", "PB8",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB8", interruptGroup: 8),
                    new PwmChannelInfo("TIM4_CH3", 4, 3),
                    new CanChannelInfo("CAN1_RX", SerialDirectionType.Receive)
                }
            );
            // D04
            // I2C4_SDA, TIM4_CH4, TIM11_CH1, I2C1_SDA, SPI2_NSS/I2S2_WS, DFSDM_DATIN7, UART5_TX, CAN1_TX, SDMMC2_D5, I2C4_SMBA, SDMMC1_D5, DCMI_D7, LCD_B7, EVENTOUT
            /// <inheritdoc/>
            public IPin D04 => _d04 ??= new Pin(
                Controller,
                "D04", "PB9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB9", interruptGroup: 9),
                    new PwmChannelInfo("TIM4_CH4", 4, 4),
                    new CanChannelInfo("CAN1_TX", SerialDirectionType.Transmit)
                }
            );

            //==== Right Header

            // D05 //NOTE: we can expose UART_7 if we want here
            // NJTRST, TIM3_CH1, SPI1_MISO, SPI3_MISO, SPI2_NSS/I2S2_WS, SPI6_MISO, SDMMC2_D3, CAN3_TX, UART7_TX, EVENTOUT
            /// <inheritdoc/>
            public IPin D05 => _d05 ??= new Pin(
                Controller,
                "D05", "PB4",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB4", interruptGroup: 4)
                }
            );

            // D06
            // I2C2_SMBA, SPI5_SCK, TIM12_CH1, ETH_MII_RXD2, FMC_SDNE1, DCMI_D8, EVENTOUT
            /// <inheritdoc/>
            public IPin D06 => _d06 ??= new Pin(
                Controller,
                "D06", "PB13",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB13", interruptGroup: 13),
                }
            );

            // D07
            // TIM4_CH2, I2C1_SDA, DFSDM_CKIN5, USART1_RX, I2C4_SDA, FMC_NL, DCMI_VSYNC, EVENTOUT
            /// <inheritdoc/>
            public IPin D07 => _d07 ??= new Pin(
                Controller,
                "D07", "PB7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB7", interruptGroup: 7),
                    new PwmChannelInfo("TIM4_CH2", 4, 2),
                    new I2cChannelInfo("I2C1_SDA", I2cChannelFunctionType.Data) // or I2C4_SDA
                }
            );


            // D08
            // UART5_TX, TIM4_CH1, HDMI_CEC, I2C1_SCL, DFSDM_DATIN5, USART1_TX, CAN2_TX, QUADSPI_BK1_NCS, I2C4_SCL, FMC_SDNE1, DCMI_D5, EVENTOUT
            /// <inheritdoc/>
            public IPin D08 => _d08 ??= new Pin(
                Controller,
                "D08", "PB6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB6", interruptGroup: 6),
                    new PwmChannelInfo("TIM4_CH1", 4, 1),
                    new I2cChannelInfo("I2C1_SCL", I2cChannelFunctionType.Clock ) // or I2C4_SCL
                }
            );
            // D09
            // TIM3_CH1, TIM8_CH1, I2S2_MCK, DFSDM_CKIN3, USART6_TX, FMC_NWAIT, SDMMC2_D6, SDMMC1_D6, DCMI_D0, LCD_HSYNC, EVENTOUT
            /// <inheritdoc/>
            public IPin D09 => _d09 ??= new Pin(
                Controller,
                "D09", "PC6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC6", interruptGroup: 6),
                    new PwmChannelInfo("TIM8_CH1", 8, 1), // or TIM3_CH1 (see D05)
                }
            );
            // D10
            // TIM3_CH2, TIM8_CH2, I2S3_MCK, DFSDM_DATIN3, USART6_RX, FMC_NE1, SDMMC2_D7, SDMMC1_D7, DCMI_D1, LCD_G6, EVENTOUT
            /// <inheritdoc/>
            public IPin D10 => _d10 ??= new Pin(
                Controller,
                "D10", "PC7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC7", interruptGroup: 7),
                    new PwmChannelInfo("TIM3_CH2", 3, 2) // or TIM8_CH2
                }
            );
            // D11
            // MCO2, TIM3_CH4, TIM8_CH4, I2C3_SDA, I2S_CKIN, UART5_CTS, QUADSPI_BK1_IO0, LCD_G3, SDMMC1_D1, DCMI_D3, LCD_B2, EVENTOUT   
            /// <inheritdoc/>
            public IPin D11 => _d11 ??= new Pin(
                Controller,
                "D11", "PC9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC9", interruptGroup: 9),
                    new PwmChannelInfo("TIM8_CH4", 8, 4)
                }
            );
            // D12
            // TIM1_CH2N, TIM8_CH2N, USART1_TX, SPI2_MISO, DFSDM_DATIN2, USART3_RTS, UART4_RTS, TIM12_CH1, SDMMC2_D0, OTG_HS_DM, EVENTOUT
            /// <inheritdoc/>
            public IPin D12 => _d12 ??= new Pin(
                Controller,
                "D12", "PB14",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB14", interruptGroup: 14),
                    new PwmChannelInfo("TIM12_CH1", 12, 1),
                    new UartChannelInfo("USART1_TX", SerialDirectionType.Transmit)
                }
            );
            // D13
            // RTC_REFIN, TIM1_CH3N, TIM8_CH3N, USART1_RX, SPI2_MOSI/I2S2_SD, DFSDM_CKIN2, UART4_CTS, TIM12_CH2, SDMMC2_D1, OTG_HS_DP, EVENTOUT
            /// <inheritdoc/>
            public IPin D13 => _d13 ??= new Pin(
                Controller,
                "D13", "PB15",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB15", interruptGroup: 15),
                    new PwmChannelInfo("TIM12_CH2", 12, 2),
                    new UartChannelInfo("USART1_RX", SerialDirectionType.Receive)
                }
            );
            // D14
            // TIM1_BKIN, I2C2_SMBA, SPI2_NSS/I2S2_WS, DFSDM_DATIN1, USART3_CK, UART5_RX, CAN2_RX, OTG_HS_ULPI_D5, ETH_MII_TXD0/ETH_RMII _TXD0, OTG_HS_ID, EVENTOUT
            /// <inheritdoc/>
            public IPin D14 => _d14 ??= new Pin(
                Controller,
                "D14", "PB12",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB12", interruptCapable: false),
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
            /// <inheritdoc/>
            public IPin D15 => _d15 ??= new Pin(
                Controller,
                "D15", "PG12",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PG12", interruptCapable: false),
                    //new PwmChannelInfo("TIM12_CH1", 12, 1),
                }
            );

            //==== ESP Coprocessor
            // TODO: if C# ever allows it, it would be nice to make these internal
            // even though they are on the interface

            // ESP_MOSI
            /// <inheritdoc/>
            public IPin ESP_COPI => new Pin(
                Controller,
                "ESP_MOSI", "PI3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI3"),
                    new SpiChannelInfo("PI3", SpiLineType.COPI)
                }
            );
            // ESP_MISO
            /// <inheritdoc/>
            public IPin ESP_CIPO => new Pin(
                Controller,
                "ESP_MISO", "PI2",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI2", interruptCapable: false),
                    new SpiChannelInfo("PI2", SpiLineType.CIPO)
                }
            );
            // ESP_CLK
            /// <inheritdoc/>
            public IPin ESP_CLK => new Pin(
                Controller,
                "ESP_CLK", "PD3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PD3", interruptGroup: 3),
                    new SpiChannelInfo("PD3", SpiLineType.Clock)
                }
            );
            // ESP_CS
            /// <inheritdoc/>
            public IPin ESP_CS => new Pin(
                Controller,
                "ESP_CS", "PI0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI0", interruptGroup: 0),
                    new SpiChannelInfo("PI10", SpiLineType.ChipSelect)
                }
            );
            // ESP_BOOT
            /// <inheritdoc/>
            public IPin ESP_BOOT => new Pin(
                Controller,
                "ESP_BOOT", "PI10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI10", interruptCapable: false),
                }
            );
            // ESP_RST
            /// <inheritdoc/>
            public IPin ESP_RST => new Pin(
                Controller,
                "ESP_RST", "PF7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PF7", interruptGroup: 7),
                }
            );
            // ESP_UART_RX
            /// <inheritdoc/>
            public IPin ESP_UART_RX => new Pin(
                Controller,
                "ESP_UART_RX", "PD2",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PD2", interruptGroup: 2),
                }
            );
            // NOTE: This changed in v2
            // ESP_UART_TX
            /// <inheritdoc/>
            public IPin ESP_UART_TX => new Pin(
                Controller,
                "ESP_UART_TX", "PC12",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC12", interruptCapable: false),
                }
            );

            internal IPin BAT => new Pin(
                Controller,
                "BAT", "PC3",
                new List<IChannelInfo> {
                    new AnalogChannelInfo("ADC1_IN13", ADCPrecisionBits, true, false)
                }
            );

            // TODO: let the interface handle this when we get to .NET Standard 2.1
            /// <inheritdoc/>
            public IPin I2C_SDA => D07;
            /// <inheritdoc/>
            public IPin I2C_SCL => D08;
        }
    }
}
