using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow.Devices;

public partial class F7CoreComputeV2
{
    private const int ADCPrecisionBits = 12;

    /// <summary>
    /// Defines the pinout for the Meadow F7 Core Compute v2 module.
    /// </summary>
    public partial class Pinout : PinDefinitionBase, IF7CoreComputePinout
    {
        private IPin? _pa0;
        private IPin? _pa1;
        private IPin? _pa2;
        private IPin? _pa3;
        private IPin? _pa4;
        private IPin? _pa5;
        private IPin? _pa7;
        private IPin? _pa9;
        private IPin? _pa10;
        private IPin? _pa13;
        private IPin? _pa14;
        private IPin? _pa15;
        private IPin? _pb0;
        private IPin? _pb1;
        private IPin? _pb3;
        private IPin? _pb4;
        private IPin? _pb5;
        private IPin? _pb6;
        private IPin? _pb7;
        private IPin? _pb8;
        private IPin? _pb9;
        private IPin? _pb11;
        private IPin? _pb12;
        private IPin? _pb13;
        private IPin? _pb14;
        private IPin? _pb15;
        private IPin? _pc0;
        private IPin? _pc1;
        private IPin? _pc2;
        private IPin? _pc4;
        private IPin? _pc5;
        private IPin? _pc6;
        private IPin? _pc7;
        private IPin? _pc8;
        private IPin? _pc9;
        private IPin? _pc10;
        private IPin? _pc11;
        private IPin? _pd5;
        private IPin? _pd6;
        private IPin? _pd7;
        private IPin? _pf8;
        private IPin? _pf9;
        private IPin? _pg6;
        private IPin? _pg9;
        private IPin? _pg10;
        private IPin? _pg11;
        private IPin? _pg12;
        private IPin? _pg13;
        private IPin? _pg14;
        private IPin? _ph6;
        private IPin? _ph7;
        private IPin? _ph8;
        private IPin? _ph10;
        private IPin? _ph12;
        private IPin? _ph13;
        private IPin? _ph14;
        private IPin? _pi9;
        private IPin? _pi11;

        internal Pinout() { }

        // ==== DISCRETES ====
        /// <inheritdoc/>
        public IPin PA0 => _pa0 ??= new Pin(
            Controller,
            "PA0", "PA0",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PA0", interruptGroup: 0),
                new PwmChannelInfo("TIM2_CH1", 2, 1)
            }
        );

        /// <inheritdoc/>
        public IPin PA1_ETH_REF_CLK => _pa1 ??= new Pin(
            Controller,
            "PA1", "PA1",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PA1", interruptGroup: 1),
            }
        );

        /// <inheritdoc/>
        public IPin PA2_ETH_MDIO => _pa2 ??= new Pin(
            Controller,
            "PA2", "PA2",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PA2", interruptGroup: 2),
            }
        );

        /// <inheritdoc/>
        public IPin PA3 => _pa3 ??= new Pin(
            Controller,
            "PA3", "PA3",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PA3", interruptGroup: 3),
                new AnalogChannelInfo("ADC1_IN3", ADCPrecisionBits, true, false)
            }
        );

        /// <inheritdoc/>
        public IPin PA4 => _pa4 ??= new Pin(
            Controller,
            "PA4", "PA4",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PA4", interruptGroup: 4),
                new AnalogChannelInfo("ADC1_IN4", ADCPrecisionBits, true, false)
            }
        );

        /// <inheritdoc/>
        public IPin PA5 => _pa5 ??= new Pin(
            Controller,
            "PA5", "PA5",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PA5", interruptGroup: 5),
                new AnalogChannelInfo("ADC1_IN5", ADCPrecisionBits, true, false)
            }
        );

        /// <inheritdoc/>
        public IPin PA7_ETH_CRS_DV => _pa7 ??= new Pin(
            Controller,
            "PA7", "PA7",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PA7", interruptGroup: 7),
            }
        );

        /// <inheritdoc/>
        public IPin PA9 => _pa9 ??= new Pin(
            Controller,
            "PA9", "PA9",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PA9", interruptGroup: 9),
            }
        );

        /// <inheritdoc/>
        public IPin PA10 => _pa10 ??= new Pin(
            Controller,
            "PA10", "PA10",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PA10", interruptCapable: false),
            }
        );

        /// <inheritdoc/>
        public IPin PA13 => _pa13 ??= new Pin(
            Controller,
            "PA13", "PA13",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PA13", interruptGroup: 13),
            }
        );

        /// <inheritdoc/>
        public IPin PA14 => _pa14 ??= new Pin(
            Controller,
            "PA14", "PA14",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PA14", interruptGroup: 14),
            }
        );

        /// <inheritdoc/>
        public IPin PA15 => _pa15 ??= new Pin(
            Controller,
            "PA15", "PA15",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PA15", interruptGroup: 15),
            }
        );

        /// <inheritdoc/>
        public IPin PB0 => _pb0 ??= new Pin(
            Controller,
            "PB0", "PB0",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB0", interruptGroup: 0),
                new AnalogChannelInfo("ADC1_IN8", ADCPrecisionBits, true, false)
            }
        );

        /// <inheritdoc/>
        public IPin PB1 => _pb1 ??= new Pin(
            Controller,
            "PB1", "PB1",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB1", interruptGroup: 1),
                new AnalogChannelInfo("ADC1_IN9", ADCPrecisionBits, true, false)
            }
        );

        /// <inheritdoc/>
        public IPin PB3 => _pb3 ??= new Pin(
            Controller,
            "PB3", "PB3",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB3", interruptGroup: 3),
            }
        );

        /// <inheritdoc/>
        public IPin PB4 => _pb4 ??= new F7Pin(
            Controller,
            "PB4", "PB4",
            Core.Interop.STM32.GpioPort.PortB, 4,
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB4", interruptGroup: 4),
                new PwmChannelInfo("TIM3_CH1", 3, 1),
            }
        );

        /// <inheritdoc/>
        public IPin PB5 => _pb5 ??= new Pin(
            Controller,
            "PB5", "PB5",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB5", interruptGroup: 5),
                new SpiChannelInfo("SPI3_COPI", SpiLineType.COPI)
            }
        );

        /// <inheritdoc/>
        public IPin PB6 => _pb6 ??= new F7Pin(
            Controller,
            "PB6", "PB6",
            Core.Interop.STM32.GpioPort.PortB, 6,
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB6", interruptGroup: 6),
                new PwmChannelInfo("TIM4_CH1", 4, 1),
                new I2cChannelInfo("I2C1_SCL", I2cChannelFunctionType.Clock)
            }
        );

        /// <inheritdoc/>
        public IPin PB7 => _pb7 ??= new F7Pin(
            Controller,
            "PB7", "PB7",
            Core.Interop.STM32.GpioPort.PortB, 7,
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB7", interruptGroup: 7),
                new PwmChannelInfo("TIM4_CH2", 4, 2),
                new I2cChannelInfo("I2C1_SDA", I2cChannelFunctionType.Data)
            }
        );

        /// <inheritdoc/>
        public IPin PB8 => _pb8 ??= new F7Pin(
            Controller,
            "PB8", "PB8",
            Core.Interop.STM32.GpioPort.PortB, 8,
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB8", interruptGroup: 8),
                new PwmChannelInfo("TIM4_CH3", 4, 3)
            }
        );

        /// <inheritdoc/>
        public IPin PB9 => _pb9 ??= new F7Pin(
            Controller,
            "PB9", "PB9",
            Core.Interop.STM32.GpioPort.PortB, 9,
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB9", interruptGroup: 9),
                new PwmChannelInfo("TIM4_CH4", 4, 4),
            }
        );

        /// <inheritdoc/>
        public IPin PB11_ETH_TX_EN => _pb11 ??= new Pin(
            Controller,
            "PB11", "PB11",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB11", interruptGroup: 11),
            }
        );

        /// <inheritdoc/>
        public IPin PB12 => _pb12 ??= new Pin(
            Controller,
            "PB12", "PB12",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB12", interruptGroup: 12),
            }
        );

        /// <inheritdoc/>
        public IPin PB13 => _pb13 ??= new Pin(
            Controller,
            "PB13", "PB13",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB13", interruptGroup: 13),
            }
        );

        /// <inheritdoc/>
        public IPin PB14 => _pb14 ??= new F7Pin(
            Controller,
            "PB14", "PB14",
            Core.Interop.STM32.GpioPort.PortB, 14,
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB14", interruptGroup: 14),
                new PwmChannelInfo("TIM12_CH1", 12, 1),
                new UartChannelInfo("COM1_TX", SerialDirectionType.Transmit)
            }
        );

        /// <inheritdoc/>
        public IPin PB15 => _pb15 ??= new F7Pin(
            Controller,
            "PB15", "PB15",
            Core.Interop.STM32.GpioPort.PortB, 15,
            new List<IChannelInfo> {
                new DigitalChannelInfo("PB15", interruptGroup: 15),
                new PwmChannelInfo("TIM12_CH2", 12, 2),
                new UartChannelInfo("COM11_RX", SerialDirectionType.Receive)
            }
        );

        /// <inheritdoc/>
        public IPin PC0 => _pc0 ??= new Pin(
            Controller,
            "PC0", "PC0",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PC0", interruptGroup: 0),
                new AnalogChannelInfo("ADC1_IN10", ADCPrecisionBits, true, false)
            }
        );

        /// <inheritdoc/>
        public IPin PC1_ETH_MDC => _pc1 ??= new Pin(
            Controller,
            "PC1", "PC1",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PC1", interruptGroup: 1),
            }
        );

        /// <inheritdoc/>
        public IPin PC2 => _pc2 ??= new Pin(
            Controller,
            "PC2", "PC2",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PC2", interruptGroup: 2),
            }
        );

        /// <inheritdoc/>
        public IPin PC4_ETH_RXD0 => _pc4 ??= new Pin(
            Controller,
            "PC4", "PC4",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PC4", interruptGroup: 4),
            }
        );

        /// <inheritdoc/>
        public IPin PC5_ETH_RXD1 => _pc5 ??= new Pin(
            Controller,
            "PC5", "PC5",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PC5", interruptGroup: 5),
            }
        );

        /// <inheritdoc/>
        public IPin PC6 => _pc6 ??= new F7Pin(
            Controller,
            "PC6", "PC6",
            Core.Interop.STM32.GpioPort.PortC, 6,
            new List<IChannelInfo> {
                new DigitalChannelInfo("PC6", interruptGroup: 6),
                new PwmChannelInfo("TIM8_CH1", 8, 1), // or TIM3_CH1 (see D05)
            }
        );

        /// <inheritdoc/>
        public IPin PC7 => _pc7 ??= new F7Pin(
            Controller,
            "PC7", "PC7",
            Core.Interop.STM32.GpioPort.PortC, 7,
            new List<IChannelInfo> {
                new DigitalChannelInfo("PC7", interruptGroup: 7),
                new PwmChannelInfo("TIM3_CH2", 3, 2) // or TIM8_CH2
            }
        );

        /// <inheritdoc/>
        public IPin PC8 => _pc8 ??= new Pin(
            Controller,
            "PC8", "PC8",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PC8", interruptGroup: 8),
            }
        );

        /// <inheritdoc/>
        public IPin PC9 => _pc9 ??= new F7Pin(
            Controller,
            "PC9", "PC9",
            Core.Interop.STM32.GpioPort.PortC, 9,
            new List<IChannelInfo> {
                new DigitalChannelInfo("PC9", interruptGroup: 9),
                new PwmChannelInfo("TIM8_CH4", 8, 4)
            }
        );

        /// <inheritdoc/>
        public IPin PC10 => _pc10 ??= new Pin(
            Controller,
            "PC10", "PC10",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PC10", inputCapable: false ),
                new SpiChannelInfo("SPI3_SCK", SpiLineType.Clock)
            }
        );

        /// <inheritdoc/>
        public IPin PC11 => _pc11 ??= new Pin(
            Controller,
            "PC11", "PC11",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PC11", interruptGroup: 11),
                new SpiChannelInfo("SPI3_CIPO", SpiLineType.CIPO)
            }
        );

        /// <inheritdoc/>
        public IPin PD5 => _pd5 ??= new Pin(
            Controller,
            "PD5", "PD5",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PD5", interruptGroup: 5),
            }
        );

        /// <inheritdoc/>
        public IPin PD6_SDMMC_CLK => _pd6 ??= new Pin(
            Controller,
            "PD6", "PD6",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PD6", interruptGroup: 6),
            }
        );

        /// <inheritdoc/>
        public IPin PD7_SDMMC_CMD => _pd7 ??= new Pin(
            Controller,
            "PD7", "PD7",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PD7", interruptGroup: 7),
            }
        );

        /// <inheritdoc/>
        public IPin PF8 => _pf8 ??= new Pin(
            Controller,
            "PF8", "PF8",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PF8", interruptGroup: 8),
                new SpiChannelInfo("SPI5_CIPO", SpiLineType.CIPO)
            }
        );

        /// <inheritdoc/>
        public IPin PF9 => _pf9 ??= new Pin(
            Controller,
            "PF9", "PF9",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PF9", interruptGroup: 9),
                new SpiChannelInfo("SPI5_COPI", SpiLineType.COPI)
            }
        );

        /// <inheritdoc/>
        public IPin PG6_SDMMC_IN_L => _pg6 ??= new Pin(
            Controller,
            "PG6", "PG6",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PG6", interruptCapable: false),
            }
        );

        /// <inheritdoc/>
        public IPin PG9_SDMMC_D0 => _pg9 ??= new Pin(
            Controller,
            "PG9", "PG9",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PG9", interruptGroup: 9),
            }
        );

        /// <inheritdoc/>
        public IPin PG10_SDMMC_D1 => _pg10 ??= new Pin(
            Controller,
            "PG10", "PG10",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PG10", interruptCapable: false),
            }
        );

        /// <inheritdoc/>
        public IPin PG11_SDMMC_D2 => _pg11 ??= new Pin(
            Controller,
            "PG11", "PG11",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PG11", interruptGroup: 11),
            }
        );

        /// <inheritdoc/>
        public IPin PG12_SDMMC_D3 => _pg12 ??= new Pin(
            Controller,
            "PG12", "PG12",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PG12", interruptGroup: 12),
            }
        );

        /// <inheritdoc/>
        public IPin PG13_ETH_TXD0 => _pg13 ??= new Pin(
            Controller,
            "PG13", "PG13",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PG13", interruptGroup: 13),
            }
        );

        /// <inheritdoc/>
        public IPin PG14_ETH_TXD1 => _pg14 ??= new Pin(
            Controller,
            "PG14", "PG14",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PG14", interruptGroup: 14),
            }
        );

        /// <inheritdoc/>
        public IPin PH6 => _ph6 ??= new Pin(
            Controller,
            "PH6", "PH6",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PH6", interruptGroup: 6),
                new SpiChannelInfo("SPI5_SCK", SpiLineType.Clock)
            }
        );

        /// <inheritdoc/>
        public IPin PH7 => _ph7 ??= new Pin(
            Controller,
            "PH7", "PH7",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PH7", interruptGroup: 7),
                new I2cChannelInfo("I2C3_SDA", I2cChannelFunctionType.Data)
            }
        );

        /// <inheritdoc/>
        public IPin PH8 => _ph8 ??= new Pin(
            Controller,
            "PH8", "PH8",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PH8", interruptGroup: 8),
                new I2cChannelInfo("I2C3_SCL", I2cChannelFunctionType.Clock)
            }
        );

        /// <inheritdoc/>
        public IPin PH10 => _ph10 ??= new F7Pin(
            Controller,
            "PH10", "PH10",
            Core.Interop.STM32.GpioPort.PortH, 10,
            new List<IChannelInfo> {
                new DigitalChannelInfo("PH10", interruptCapable: false ),
                new PwmChannelInfo("TIM5_CH1", 5, 1)
            }
        );

        /// <inheritdoc/>
        public IPin PH12 => _ph12 ??= new Pin(
            Controller,
            "PH12", "PH12",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PH12", interruptGroup: 12),
            }
        );

        /// <inheritdoc/>
        public IPin PH13 => _ph13 ??= new Pin(
            Controller,
            "PH13", "PH13",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PH13", interruptGroup: 13),
            }
        );

        /// <inheritdoc/>
        public IPin PH14_ETH_IRQ => _ph14 ??= new Pin(
            Controller,
            "PH14", "PH14",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PH14", interruptGroup: 14),
            }
        );

        /// <inheritdoc/>
        public IPin PI9 => _pi9 ??= new Pin(
            Controller,
            "PI9", "PI9",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PI9", interruptGroup: 9),
                new UartChannelInfo("UART4_RX", SerialDirectionType.Receive)
            }
        );

        /// <inheritdoc/>
        public IPin PI11 => _pi11 ??= new Pin(
            Controller,
            "PI11", "PI11",
            new List<IChannelInfo> {
                new DigitalChannelInfo("PI11", interruptGroup: 11),
            }
        );

        // ==== ALIASES ====
        /// <inheritdoc/>
        public IPin I2C1_SDA => D07;
        /// <inheritdoc/>
        public IPin I2C1_SCL => D08;
        /// <inheritdoc/>
        public IPin I2C3_SDA => PH7;
        /// <inheritdoc/>
        public IPin I2C3_SCL => PH8;

        /// <inheritdoc/>
        public IPin SCK => SPI3_SCK;
        /// <inheritdoc/>
        public IPin COPI => SPI3_COPI;
        /// <inheritdoc/>
        public IPin CIPO => SPI3_CIPO;

        /// <inheritdoc/>
        public IPin SPI3_COPI => PB5;
        /// <inheritdoc/>
        public IPin SPI3_SCK => PC10;
        /// <inheritdoc/>
        public IPin SPI3_CIPO => PC11;
        /// <inheritdoc/>
        public IPin SPI5_CIPO => PF8;
        /// <inheritdoc/>
        public IPin SPI5_SCK => PH6;
        /// <inheritdoc/>
        public IPin SPI5_COPI => PF9;
        /// <inheritdoc/>
        public IPin A00 => PA4;
        /// <inheritdoc/>
        public IPin A01 => PA5;
        /// <inheritdoc/>
        public IPin A02 => PA3;
        /// <inheritdoc/>
        public IPin A03 => PB0;
        /// <inheritdoc/>
        public IPin A04 => PB1;
        /// <inheritdoc/>
        public IPin A05 => PC0;
        /// <inheritdoc/>
        public IPin D00 => PI9;
        /// <inheritdoc/>
        public IPin D01 => PH13;
        /// <inheritdoc/>
        public IPin D02 => PH10;
        /// <inheritdoc/>
        public IPin D03 => PB8;
        /// <inheritdoc/>
        public IPin D04 => PB9;
        /// <inheritdoc/>
        public IPin D05 => PB4;
        /// <inheritdoc/>
        public IPin D06 => PB13;
        /// <inheritdoc/>
        public IPin D07 => PB7;
        /// <inheritdoc/>
        public IPin D08 => PB6;
        /// <inheritdoc/>
        public IPin D09 => PC6;
        /// <inheritdoc/>
        public IPin D10 => PC7;
        /// <inheritdoc/>
        public IPin D11 => PC9;
        /// <inheritdoc/>
        public IPin D12 => PB14;
        /// <inheritdoc/>
        public IPin D13 => PB15;
        /// <inheritdoc/>
        public IPin D14 => PB12;
        /// <inheritdoc/>
        public IPin D15 => PG12_SDMMC_D3;
        /// <inheritdoc/>
        public IPin D16 => PI11;
        /// <inheritdoc/>
        public IPin D17 => PD5;
        /// <inheritdoc/>
        public IPin D18 => PA10;
        /// <inheritdoc/>
        public IPin D19 => PC8;
        /// <inheritdoc/>
        public IPin D20 => PA0; // BLINKY on dev board
    }
}