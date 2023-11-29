using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Meadow.Devices
{
    public partial class F7CoreComputeV2
    {
        private const int ADCPrecisionBits = 12;

        /// <summary>
        /// Defines the pinout for the Meadow F7 Core Compute v2 module.
        /// </summary>
        public partial class Pinout : IF7CoreComputePinout
        {
            /// <inheritdoc/>
            public IPinController? Controller { get; set; }

            /// <inheritdoc/>
            public IList<IPin> AllPins { get; }

            internal Pinout()
            {
                AllPins = new List<IPin>();

                foreach (var pin in this.GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType is IPin)
                    .Select(p => p.GetValue(this) as IPin))
                {
                    if (pin != null)
                    {
                        AllPins.Add(pin);
                    }
                }
            }

            // ==== DISCRETES ====

            public IPin PA0 => new Pin(
                Controller,
                "PA0", "PA0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA0", interruptGroup: 0),
                    new PwmChannelInfo("TIM2_CH1", 2, 1)
                }
            );

            public IPin PA1_ETH_REF_CLK => new Pin(
                Controller,
                "PA1", "PA1",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA1", interruptGroup: 1),
                }
            );

            public IPin PA2_ETH_MDIO => new Pin(
                Controller,
                "PA2", "PA2",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA2", interruptGroup: 2),
                }
            );

            public IPin PA3 => new Pin(
                Controller,
                "PA3", "PA3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA3", interruptGroup: 3),
                    new AnalogChannelInfo("ADC1_IN3", ADCPrecisionBits, true, false)
                }
            );

            public IPin PA4 => new Pin(
                Controller,
                "PA4", "PA4",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA4", interruptGroup: 4),
                    new AnalogChannelInfo("ADC1_IN4", ADCPrecisionBits, true, false)
                }
            );

            public IPin PA5 => new Pin(
                Controller,
                "PA5", "PA5",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA5", interruptGroup: 5),
                    new AnalogChannelInfo("ADC1_IN5", ADCPrecisionBits, true, false)
                }
            );

            public IPin PA7_ETH_CRS_DV => new Pin(
                Controller,
                "PA7", "PA7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA7", interruptGroup: 7),
                }
            );

            public IPin PA9 => new Pin(
                Controller,
                "PA9", "PA9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA9", interruptGroup: 9),
                }
            );

            public IPin PA10 => new Pin(
                Controller,
                "PA10", "PA10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA10", interruptCapable: false),
                }
            );

            public IPin PA13 => new Pin(
                Controller,
                "PA13", "PA13",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA13", interruptGroup: 13),
                }
            );

            public IPin PA14 => new Pin(
                Controller,
                "PA14", "PA14",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA14", interruptGroup: 14),
                }
            );

            public IPin PA15 => new Pin(
                Controller,
                "PA15", "PA15",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA15", interruptGroup: 15),
                }
            );

            public IPin PB0 => new Pin(
                Controller,
                "PB0", "PB0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB0", interruptGroup: 0),
                    new AnalogChannelInfo("ADC1_IN8", ADCPrecisionBits, true, false)
                }
            );

            public IPin PB1 => new Pin(
                Controller,
                "PB1", "PB1",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB1", interruptGroup: 1),
                    new AnalogChannelInfo("ADC1_IN9", ADCPrecisionBits, true, false)
                }
            );

            public IPin PB3 => new Pin(
                Controller,
                "PB3", "PB3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB3", interruptGroup: 3),
                }
            );

            public IPin PB4 => new Pin(
                Controller,
                "PB4", "PB4",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB4", interruptGroup: 4),
                    new PwmChannelInfo("TIM3_CH1", 3, 1),
                }
            );

            public IPin PB5 => new Pin(
                Controller,
                "PB5", "PB5",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB5", interruptGroup: 5),
                    new SpiChannelInfo("SPI3_COPI", SpiLineType.COPI)
                }
            );

            public IPin PB6 => new Pin(
                Controller,
                "PB6", "PB6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB6", interruptGroup: 6),
                    new PwmChannelInfo("TIM4_CH1", 4, 1),
                    new I2cChannelInfo("I2C1_SCL", I2cChannelFunctionType.Clock)
                }
            );

            public IPin PB7 => new Pin(
                Controller,
                "PB7", "PB7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB7", interruptGroup: 7),
                    new PwmChannelInfo("TIM4_CH2", 4, 2),
                    new I2cChannelInfo("I2C1_SDA", I2cChannelFunctionType.Data)
                }
            );

            public IPin PB8 => new Pin(
                Controller,
                "PB8", "PB8",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB8", interruptGroup: 8),
                    new PwmChannelInfo("TIM4_CH3", 4, 3)
                }
            );

            public IPin PB9 => new Pin(
                Controller,
                "PB9", "PB9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB9", interruptGroup: 9),
                    new PwmChannelInfo("TIM4_CH4", 4, 4),
                }
            );

            public IPin PB11_ETH_TX_EN => new Pin(
                Controller,
                "PB11", "PB11",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB11", interruptGroup: 11),
                }
            );

            public IPin PB12 => new Pin(
                Controller,
                "PB12", "PB12",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB12", interruptGroup: 12),
                }
            );

            public IPin PB13 => new Pin(
                Controller,
                "PB13", "PB13",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB13", interruptGroup: 13),
                }
            );

            public IPin PB14 => new Pin(
                Controller,
                "PB14", "PB14",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB14", interruptGroup: 14),
                    new PwmChannelInfo("TIM12_CH1", 12, 1),
                    new UartChannelInfo("COM1_TX", SerialDirectionType.Transmit)
                }
            );

            public IPin PB15 => new Pin(
                Controller,
                "PB15", "PB15",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB15", interruptGroup: 15),
                    new PwmChannelInfo("TIM12_CH2", 12, 2),
                    new UartChannelInfo("COM11_RX", SerialDirectionType.Receive)
                }
            );

            public IPin PC0 => new Pin(
                Controller,
                "PC0", "PC0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC0", interruptGroup: 0),
                    new AnalogChannelInfo("ADC1_IN10", ADCPrecisionBits, true, false)
                }
            );

            public IPin PC1_ETH_MDC => new Pin(
                Controller,
                "PC1", "PC1",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC1", interruptGroup: 1),
                }
            );

            public IPin PC2 => new Pin(
                Controller,
                "PC2", "PC2",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC2", interruptGroup: 2),
                }
            );

            public IPin PC4_ETH_RXD0 => new Pin(
                Controller,
                "PC4", "PC4",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC4", interruptGroup: 4),
                }
            );

            public IPin PC5_ETH_RXD1 => new Pin(
                Controller,
                "PC5", "PC5",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC5", interruptGroup: 5),
                }
            );

            public IPin PC6 => new Pin(
                Controller,
                "PC6", "PC6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC6", interruptGroup: 6),
                    new PwmChannelInfo("TIM8_CH1", 8, 1), // or TIM3_CH1 (see D05)
                }
            );

            public IPin PC7 => new Pin(
                Controller,
                "PC7", "PC7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC7", interruptGroup: 7),
                    new PwmChannelInfo("TIM3_CH2", 3, 2) // or TIM8_CH2
                }
            );

            public IPin PC8 => new Pin(
                Controller,
                "PC8", "PC8",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC8", interruptGroup: 8),
                }
            );

            public IPin PC9 => new Pin(
                Controller,
                "PC9", "PC9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC9", interruptGroup: 9),
                    new PwmChannelInfo("TIM8_CH4", 8, 4)
                }
            );

            public IPin PC10 => new Pin(
                Controller,
                "PC10", "PC10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC10", inputCapable: false ),
                    new SpiChannelInfo("SPI3_SCK", SpiLineType.Clock)
                }
            );

            public IPin PC11 => new Pin(
                Controller,
                "PC11", "PC11",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC11", interruptGroup: 11),
                    new SpiChannelInfo("SPI3_CIPO", SpiLineType.CIPO)
                }
            );

            public IPin PD5 => new Pin(
                Controller,
                "PD5", "PD5",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PD5", interruptGroup: 5),
                }
            );

            public IPin PD6_SDMMC_CLK => new Pin(
                Controller,
                "PD6", "PD6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PD6", interruptGroup: 6),
                }
            );

            public IPin PD7_SDMMC_CMD => new Pin(
                Controller,
                "PD7", "PD7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PD7", interruptGroup: 7),
                }
            );

            public IPin PF8 => new Pin(
                Controller,
                "PF8", "PF8",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PF8", interruptGroup: 8),
                    new SpiChannelInfo("SPI5_CIPO", SpiLineType.CIPO)
                }
            );

            public IPin PF9 => new Pin(
                Controller,
                "PF9", "PF9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PF9", interruptGroup: 9),
                    new SpiChannelInfo("SPI5_COPI", SpiLineType.COPI)
                }
            );

            public IPin PG6_SDMMC_IN_L => new Pin(
                Controller,
                "PG6", "PG6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PG6", interruptCapable: false),
                }
            );

            public IPin PG9_SDMMC_D0 => new Pin(
                Controller,
                "PG9", "PG9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PG9", interruptGroup: 9),
                }
            );

            public IPin PG10_SDMMC_D1 => new Pin(
                Controller,
                "PG10", "PG10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PG10", interruptCapable: false),
                }
            );

            public IPin PG11_SDMMC_D2 => new Pin(
                Controller,
                "PG11", "PG11",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PG11", interruptGroup: 11),
                }
            );

            public IPin PG12_SDMMC_D3 => new Pin(
                Controller,
                "PG12", "PG12",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PG12", interruptGroup: 12),
                }
            );

            public IPin PG13_ETH_TXD0 => new Pin(
                Controller,
                "PG13", "PG13",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PG13", interruptGroup: 13),
                }
            );

            public IPin PG14_ETH_TXD1 => new Pin(
                Controller,
                "PG14", "PG14",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PG14", interruptGroup: 14),
                }
            );

            public IPin PH6 => new Pin(
                Controller,
                "PH6", "PH6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH6", interruptGroup: 6),
                    new SpiChannelInfo("SPI5_SCK", SpiLineType.Clock)
                }
            );

            public IPin PH7 => new Pin(
                Controller,
                "PH7", "PH7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH7", interruptGroup: 7),
                    new I2cChannelInfo("I2C3_SDA", I2cChannelFunctionType.Data)
                }
            );

            public IPin PH8 => new Pin(
                Controller,
                "PH8", "PH8",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH8", interruptGroup: 8),
                    new I2cChannelInfo("I2C3_SCL", I2cChannelFunctionType.Clock)
                }
            );

            public IPin PH10 => new Pin(
                Controller,
                "PH10", "PH10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH10", interruptCapable: false ),
                    new PwmChannelInfo("TIM5_CH1", 5, 1)
                }
            );

            public IPin PH12 => new Pin(
                Controller,
                "PH12", "PH12",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH12", interruptGroup: 12),
                }
            );

            public IPin PH13 => new Pin(
                Controller,
                "PH13", "PH13",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH13", interruptGroup: 13),
                }
            );

            public IPin PH14_ETH_IRQ => new Pin(
                Controller,
                "PH14", "PH14",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH14", interruptGroup: 14),
                }
            );

            public IPin PI9 => new Pin(
                Controller,
                "PI9", "PI9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI9", interruptGroup: 9),
                    new UartChannelInfo("UART4_RX", SerialDirectionType.Receive)
                }
            );

            public IPin PI11 => new Pin(
                Controller,
                "PI11", "PI11",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI11", interruptGroup: 11),
                }
            );

            // ==== ALIASES ====
            public IPin I2C1_SDA => D07;
            public IPin I2C1_SCL => D08;
            public IPin I2C3_SDA => PH7;
            public IPin I2C3_SCL => PH8;

            public IPin SCK => SPI3_SCK;
            public IPin COPI => SPI3_COPI;
            public IPin CIPO => SPI3_CIPO;

            public IPin SPI3_COPI => PB5;
            public IPin SPI3_SCK => PC10;
            public IPin SPI3_CIPO => PC11;

            public IPin SPI5_CIPO => PF8;
            public IPin SPI5_SCK => PH6;
            public IPin SPI5_COPI => PF9;

            public IPin A00 => PA4;
            public IPin A01 => PA5;
            public IPin A02 => PA3;
            public IPin A03 => PB0;
            public IPin A04 => PB1;
            public IPin A05 => PC0;

            public IPin D00 => PI9;
            public IPin D01 => PH13;
            public IPin D02 => PH10;
            public IPin D03 => PB8;
            public IPin D04 => PB9;
            public IPin D05 => PB4;
            public IPin D06 => PB13;
            public IPin D07 => PB7;
            public IPin D08 => PB6;
            public IPin D09 => PC6;
            public IPin D10 => PC7;
            public IPin D11 => PC9;
            public IPin D12 => PB14;
            public IPin D13 => PB15;
            public IPin D14 => PB12;
            public IPin D15 => PG12_SDMMC_D3;
            public IPin D16 => PI11;
            public IPin D17 => PD5;
            public IPin D18 => PA10;
            public IPin D19 => PC8;
            public IPin D20 => PA0; // BLINKY on dev board

            public IEnumerator<IPin> GetEnumerator() => AllPins.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
