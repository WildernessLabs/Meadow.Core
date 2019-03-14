using System;
using System.Collections.Generic;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public partial class F7Micro
    {
        public class F7MicroPinDefinitions : IPinDefinitions
        {
            public IList<IPin> AllPins { get; } = new List<IPin>();
            public IDictionary<string, IPin[]> PinGroups { get; } = new Dictionary<string, IPin[]>();

            // OnboardLEDBlue
            // TIM2_CH1/TIM2_ETR, TIM5_CH1, TIM8_ETR, USART2_CTS, UART4_TX, SAI2_SD_B, ETH_MII_CRS, EVENTOUT
            // ADC1_IN0, ADC2_IN0, ADC3_IN0, WKUP1
            public readonly IPin OnboardLEDBlue = new Pin(
                "OnboardLEDBlue", "PA0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA0"),
                    new PwmChannelInfo("TIM2_CH1"), //?? many to choose from
                }
            );
            // OnboardLEDGreen
            // TIM2_CH2, TIM5_CH2, USART2_RTS, UART4_RX, QUADSPI_BK1_IO3, SAI2_MCLK_B, ETH_MII_RX_CLK/ETH_R MII_REF_CLK, LCD_R2, EVENTOUT
            // ADC1_IN1, ADC2_IN1, ADC3_IN1
            public readonly IPin OnboardLEDGreen = new Pin(
                "OnboardLEDGreen", "PA1",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA1"),
                    new PwmChannelInfo("TIM2_CH2"), //?? many to choose from
                }
            );
            // OnboardLEDRed
            // TIM2_CH3, TIM5_CH3, TIM9_CH1, USART2_TX, SAI2_SCK_B, ETH_MDIO, MDIOS_MDIO, LCD_R1, EVENTOUT
            // ADC1_IN2, ADC2_IN2, ADC3_IN2, WKUP2
            public readonly IPin OnboardLEDRed = new Pin(
                "OnboardLEDGreen", "PA2",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA2"),
                    new PwmChannelInfo("TIM2_CH3"), //?? many to choose from
                }
            );

            // D00
            // UART4_RX, CAN1_RX, FMC_D30, LCD_VSYNC, EVENTOUT
            public readonly IPin D00 = new Pin(
                "D00", "PI9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI9"),
                    new UartChannelInfo("UART4_RX", SerialDirectionType.Receive)
                }
            );
            // D01
            // TIM8_CH1N, UART4_TX, CAN1_TX, FMC_D21, LCD_G2, EVENTOUT
            public readonly IPin D01 = new Pin(
                "D01", "PH13",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH13"),
                    new PwmChannelInfo("TIM8_CH1N"),
                    new UartChannelInfo("UART4_TX", SerialDirectionType.Transmit)
                }
            );
            // D02
            // TIM3_CH1, TIM8_CH1, I2S2_MCK, DFSDM_CKIN3, USART6_TX, FMC_NWAIT, SDMMC2_D6, SDMMC1_D6, DCMI_D0, LCD_HSYNC, EVENTOUT
            public readonly IPin D02 = new Pin(
                "D02", "PC6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC6"),
                    new PwmChannelInfo("TIM3_CH1"), // OR TIM8_CH1. Not sure which we're using
                }
            );
            // D03
            // I2C4_SCL, TIM4_CH3, TIM10_CH1, I2C1_SCL, DFSDM_CKIN7, UART5_RX, CAN1_RX, SDMMC2_D4, ETH_MII_TXD3, SDMMC1_D4, DCMI_D6, LCD_B6, EVENTOUT
            public readonly IPin D03 = new Pin(
                "D03", "PB8",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB8"),
                    new PwmChannelInfo("TIM4_CH3"), // OR TIM10_CH1. Not sure which we're using
                    new CanChannelInfo("CAN1_RX", SerialDirectionType.Receive)
                }
            );
            // D04
            // I2C4_SDA, TIM4_CH4, TIM11_CH1, I2C1_SDA, SPI2_NSS/I2S2_WS, DFSDM_DATIN7, UART5_TX, CAN1_TX, SDMMC2_D5, I2C4_SMBA, SDMMC1_D5, DCMI_D7, LCD_B7, EVENTOUT
            public readonly IPin D04 = new Pin(
                "D04", "PB9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB9"),
                    new PwmChannelInfo("TIM4_CH3"), // OR TIM10_CH1. Not sure which we're using
                    new CanChannelInfo("CAN1_RX", SerialDirectionType.Receive)
                }
            );
            // D05
            // TIM3_CH2, TIM8_CH2, I2S3_MCK, DFSDM_DATIN3, USART6_RX, FMC_NE1, SDMMC2_D7, SDMMC1_D7, DCMI_D1, LCD_G6, EVENTOUT
            public readonly IPin D05 = new Pin(
                "D05", "PC7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB8"),
                    new PwmChannelInfo("TIM3_CH2"), // OR TIM8_CH2. Not sure which we're using
                }
            );
            // D06
            // TIM1_CH2N, TIM3_CH3, TIM8_CH2N, DFSDM_CKOUT, UART4_CTS, LCD_R3, OTG_HS_ULPI_D1, ETH_MII_RXD2, LCD_G1, EVENTOUT
            // ADC1_IN8, ADC2_IN8
            public readonly IPin D06 = new Pin(
                "D06", "PB0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB0"),
                    new PwmChannelInfo("TIM1_CH2N"), // OR TIM3_CH3, TIM8_CH2N. Not sure which we're using
                    new AnalogChannelInfo("ADC1_IN8", 12) // or ADC2_IN8?
                }
            );
            // D07
            // TIM4_CH2, I2C1_SDA, DFSDM_CKIN5, USART1_RX, I2C4_SDA, FMC_NL, DCMI_VSYNC, EVENTOUT
            public readonly IPin D07 = new Pin(
                "D07", "PB7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB7"),
                    new PwmChannelInfo("TIM4_CH2"), // OR TIM3_CH3, TIM8_CH2N. Not sure which we're using
                    new I2cChannelInfo("I2C1_SDA", I2cChannelFunctionType.Data) // or I2C4_SDA
                }
            );
            // D08
            // UART5_TX, TIM4_CH1, HDMI_CEC, I2C1_SCL, DFSDM_DATIN5, USART1_TX, CAN2_TX, QUADSPI_BK1_NCS, I2C4_SCL, FMC_SDNE1, DCMI_D5, EVENTOUT
            public readonly IPin D08 = new Pin(
                "D08", "PB6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB6"),
                    new PwmChannelInfo("TIM4_CH1"),
                    new I2cChannelInfo("I2C1_SCL", I2cChannelFunctionType.Clock ) // or I2C4_SCL
                }
            );
            // D09
            // TIM1_CH3N, TIM3_CH4, TIM8_CH3N, DFSDM_DATIN1, LCD_R6, OTG_HS_ULPI_D2, ETH_MII_RXD3, LCD_G0, EVENTOUT
            // ADC1_IN9, ADC2_IN9
            public readonly IPin D09 = new Pin(
                "D09", "PB1",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB1"),
                    new PwmChannelInfo("TIM1_CH3N"),
                    new AnalogChannelInfo("ADC1_IN9", 12) // or ADC2_IN9
                }
            );
            // D10
            // TIM5_CH1, I2C4_SMBA, FMC_D18, DCMI_D1, LCD_R4, EVENTOUT
            public readonly IPin D10 = new Pin(
                "D10", "PH10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH10"),
                    new PwmChannelInfo("TIM5_CH1"),
                }
            );
            // D11
            // MCO2, TIM3_CH4, TIM8_CH4, I2C3_SDA, I2S_CKIN, UART5_CTS, QUADSPI_BK1_IO0, LCD_G3, SDMMC1_D1, DCMI_D3, LCD_B2, EVENTOUT   
            public readonly IPin D11 = new Pin(
                "D11", "PC9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC9"),
                    new PwmChannelInfo("TIM3_CH4"), //or TIM8_CH4
                }
            );
            // D12
            // TIM1_CH2N, TIM8_CH2N, USART1_TX, SPI2_MISO, DFSDM_DATIN2, USART3_RTS, UART4_RTS, TIM12_CH1, SDMMC2_D0, OTG_HS_DM, EVENTOUT
            public readonly IPin D12 = new Pin(
                "D12", "PB14",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB14"),
                    new PwmChannelInfo("TIM1_CH2N"), //or TIM8_CH2N
                    new UartChannelInfo("USART1_TX", SerialDirectionType.Transmit)
                }
            );
            // D13
            // RTC_REFIN, TIM1_CH3N, TIM8_CH3N, USART1_RX, SPI2_MOSI/I2S2_SD, DFSDM_CKIN2, UART4_CTS, TIM12_CH2, SDMMC2_D1, OTG_HS_DP, EVENTOUT
            public readonly IPin D13 = new Pin(
                "D13", "PB15",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB15"),
                    new PwmChannelInfo("TIM1_CH3N"), //or TIM8_CH3N
                    new UartChannelInfo("USART1_RX", SerialDirectionType.Receive)
                }
            );
            // D14
            // FMC_A13, EVENTOUT
            public readonly IPin D14 = new Pin(
                "D14", "PG3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PG3"),
                }
            );
            // D15
            // TRACED0, SAI1_SD_B, FMC_A19, EVENTOUT
            public readonly IPin D15 = new Pin(
                "D15", "PE3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PE3"),
                }
            );

            // A00
            // SPI1_NSS/I2S1_WS, SPI3_NSS/I2S3_WS, USART2_CK, SPI6_NSS, OTG_HS_SOF, DCMI_HSYNC, LCD_VSYNC, EVENTOUT
            // ADC1_IN4, ADC2_IN4, DAC_OUT1
            public readonly IPin A00 = new Pin(
                "A00", "PA4",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA4"),
                    new AnalogChannelInfo("ADC1_IN4", 12)
                }
            );
            // A01
            // TIM2_CH1/TIM2_ETR, TIM8_CH1N, SPI1_SCK/I2S1_CK, SPI6_SCK, OTG_HS_ULPI_CK, LCD_R4, EVENTOUT
            // ADC1_IN5, ADC2_IN5, DAC_OUT2
            public readonly IPin A01 = new Pin(
                "A01", "PA5",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA5"),
                    new AnalogChannelInfo("ADC1_IN5", 12)
                }
            );
            // A02
            // TIM2_CH4, TIM5_CH4, TIM9_CH2, USART2_RX, LCD_B2, OTG_HS_ULPI_D0, ETH_MII_COL, LCD_B5, EVENTOUT
            // ADC1_IN3, ADC2_IN3, ADC3_IN3
            public readonly IPin A02 = new Pin(
                "A02", "PA3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA3"),
                    new AnalogChannelInfo("ADC1_IN3", 12)
                }
            );
            // A03
            // TIM1_CH1N, TIM3_CH2, TIM8_CH1N, SPI1_MOSI/I2S1_SD, SPI6_MOSI, TIM14_CH1, ETH_MII_RX_DV/ETH_RM II_CRS_DV, FMC_SDNWE, EVENTOUT
            // ADC1_IN7, ADC2_IN7
            public readonly IPin A03 = new Pin(
                "A03", "PA7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA7"),
                    new AnalogChannelInfo("ADC1_IN7", 12)
                }
            );
            // A04
            // DFSDM_CKIN0, DFSDM_DATIN4, SAI2_FS_B, OTG_HS_ULPI_STP, FMC_SDNWE, LCD_R5, EVENTOUT
            // ADC1_IN10, ADC2_IN10, ADC3_IN10
            public readonly IPin A04 = new Pin(
                "A04", "PC0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC0"),
                    new AnalogChannelInfo("ADC1_IN10", 12)
                }
            );
            // A05
            // TRACED0, DFSDM_DATIN0, SPI2_MOSI/I2S2_SD, SAI1_SD_A, DFSDM_CKIN4, ETH_MDC, MDIOS_MDC, EVENTOUT
            // ADC1_IN11, ADC2_IN11, ADC3_IN11, RTC_TAMP3/ WKUP3
            public readonly IPin A05 = new Pin(
                "A05", "PC1",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC1"),
                    new AnalogChannelInfo("ADC1_IN11", 12)
                }
            );
            // SCK
            // SPI3_CLK
            public readonly IPin SCK = new Pin(
                "SCK", "PC10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC10")
                }
            );
            // MOSI
            // SPI3_MOSI
            public readonly IPin MOSI = new Pin(
                "MOSI", "PB5",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB5")
                }
            );
            // MISO
            // SPI3_MISO
            public readonly IPin MISO = new Pin(
                "MISO", "PC11",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC11")
                }
            );

            // ESP_MOSI
            internal readonly IPin ESP_MOSI = new Pin(
                "ESP_MOSI", "PI3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI3"),
                }
            );
            // ESP_MISO
            internal readonly IPin ESP_MISO = new Pin(
                "ESP_MISO", "PI2",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI2"),
                }
            );
            // ESP_CLK
            internal readonly IPin ESP_CLK = new Pin(
                "ESP_CLK", "PD3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PD3"),
                }
            );
            // ESP_CS
            internal readonly IPin ESP_CS = new Pin(
                "ESP_CS", "PI0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI0"),
                }
            );
            // ESP_BOOT
            internal readonly IPin ESP_BOOT = new Pin(
                "ESP_BOOT", "PI10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI10"),
                }
            );
            // ESP_RST
            internal readonly IPin ESP_RST = new Pin(
                "ESP_RST", "PF7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PF7"),
                }
            );
            // ESP_UART_RX
            internal readonly IPin ESP_UART_RX = new Pin(
                "ESP_UART_RX", "PD2",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PD2"),
                }
            );
            // ESP_UART_TX
            internal readonly IPin ESP_UART_TX = new Pin(
                "ESP_UART_TX", "PB13",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB13"),
                }
            );

            public F7MicroPinDefinitions()
            {
                // add all our pins to the collection
                AllPins.Add(this.A01);
                AllPins.Add(this.A02);
                AllPins.Add(this.A03);
                AllPins.Add(this.A04);
                AllPins.Add(this.A05);
                AllPins.Add(this.SCK);
                AllPins.Add(this.MOSI);
                AllPins.Add(this.MISO);
                AllPins.Add(this.D00);
                AllPins.Add(this.D01);
                AllPins.Add(this.D02);
                AllPins.Add(this.D03);
                AllPins.Add(this.D04);
                AllPins.Add(this.D05);
                AllPins.Add(this.D06);
                AllPins.Add(this.D07);
                AllPins.Add(this.D08);
                AllPins.Add(this.D09);
                AllPins.Add(this.D10);
                AllPins.Add(this.D11);
                AllPins.Add(this.D12);
                AllPins.Add(this.D13);
                AllPins.Add(this.D14);
                AllPins.Add(this.D15);
                AllPins.Add(this.OnboardLEDRed);
                AllPins.Add(this.OnboardLEDGreen);
                AllPins.Add(this.OnboardLEDBlue);
                AllPins.Add(this.ESP_MOSI);
                AllPins.Add(this.ESP_MISO);
                AllPins.Add(this.ESP_CLK);
                AllPins.Add(this.ESP_CS);
                AllPins.Add(this.ESP_BOOT);
                AllPins.Add(this.ESP_RST);
                AllPins.Add(this.ESP_UART_RX);
                AllPins.Add(this.ESP_UART_TX);

                // TODO: no magic strings, also need lookup
                this.PinGroups.Add("COM1", new IPin[] { this.D12, this.D13 });
            }

        }

    }
}
