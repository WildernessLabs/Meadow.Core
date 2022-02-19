using System.Collections.Generic;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public partial class F7CoreCompute
    {
        private const int ADCPrecisionBits = 12;

        public class SerialPortNameDefinitions
        {
            public SerialPortName Com1 { get; } = new SerialPortName("COM1", "ttyS0");
            public SerialPortName Com4 { get; } = new SerialPortName("COM4", "ttyS1");
        }

        public partial class Pinout : IPinDefinitions // IF7MicroPinout
        {
            public IList<IPin> AllPins => new List<IPin>
            {
                A00, A01, A02, A03, A04, A05, 
                SCK, COPI, CIPO, 
                D00, D01, D02, D03, D04, D05, D06, D07, D08, D09, D10, D11, D12, D13, D14, D15,
                ESP_COPI, ESP_CIPO, ESP_CLK, ESP_CS, ESP_BOOT, ESP_RST, ESP_UART5_RX, ESP_UART5_TX,
                LED1, I2C_SCL, I2C_SDA
            };

            // ==== SPI ====
            public IPin SCK => new Pin(
                "SCK", "PC10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC10", interruptGroup: 10),
                    new SpiChannelInfo("PC10", SpiLineType.Clock)
                }
            );

            public IPin CIPO => new Pin(
                "CIPO", "PC11",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC11", interruptGroup: 11),
                    new SpiChannelInfo("PC11", SpiLineType.MISO)
                }
            );

            public IPin COPI => new Pin(
                "COPI", "PB5",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB5", interruptGroup: 5),
                    new SpiChannelInfo("PB5", SpiLineType.MOSI)
                }
            );

            // ==== BLINKY ====
            public IPin LED1 => new Pin(
                "BLINKY", "PA0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA0", inverseLogic: true, interruptGroup: 0) // TODO: check to see if this is still inverted
                }
            );

            // ==== A2D ====
            public IPin A00 => new Pin(
                "A00", "PA4",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA4", interruptGroup: 4),
                    new AnalogChannelInfo("ADC1_IN4", ADCPrecisionBits, true, false)
                }
            );

            public IPin A01 => new Pin(
                "A01", "PA5",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA5", interruptGroup: 5),
                    new AnalogChannelInfo("ADC1_IN5", ADCPrecisionBits, true, false)
                }
            );

            public IPin A02 => new Pin(
                "A02", "PA3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PA3", interruptGroup: 3),
                    new AnalogChannelInfo("ADC1_IN3", ADCPrecisionBits, true, false)
                }
            );

            public IPin A03 => new Pin(
                "A03", "PB0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB0", interruptGroup: 0),
                    new AnalogChannelInfo("ADC1_IN8", ADCPrecisionBits, true, false) // TODO: should we move this to the second ADC?
                }
            );

            public IPin A04 => new Pin(
                "A04", "PB1",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB1", interruptGroup: 1),
                    new AnalogChannelInfo("ADC1_IN9", ADCPrecisionBits, true, false)
                }
            );

            public IPin A05 => new Pin(
                "A05", "PC0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC0", interruptGroup: 0),
                    new AnalogChannelInfo("ADC1_IN10", ADCPrecisionBits, true, false)
                }
            );

            // ==== DISCRETES ====

            public IPin D00 => new Pin(
                "D00", "PI9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI9", interruptGroup: 9)
                }
            );

            // TODO: shared with ETH_IRQ?
            public IPin D01 => new Pin(
                "D01", "PH14",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH14", interruptGroup: 14)
                }
            );

            public IPin D02 => new Pin(
                "D02", "PH10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PH10", interruptGroup: 10)
                }
            );

            public IPin D03 => new Pin(
                "D03", "PB8",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB8", interruptGroup: 8)
                }
            );

            public IPin D04 => new Pin(
                "D04", "PB9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB9", interruptGroup: 9),
                    new CanChannelInfo("CAN1_RX", SerialDirectionType.Receive)
                }
            );

            public IPin D05 => new Pin(
                "D05", "PB4",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB4", interruptGroup: 4)
                }
            );

            public IPin D06 => new Pin(
                "D06", "PB13",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB13", interruptGroup: 13) 
                }
            );

            public IPin D07 => new Pin(
                "D07", "PB7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB7", interruptGroup: 7),
                    new I2cChannelInfo("I2C_SDA", I2cChannelFunctionType.Data)
                }
            );

            public IPin D08 => new Pin(
                "D08", "PB6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB6", interruptGroup: 6),
                    new I2cChannelInfo("I2C_SCL", I2cChannelFunctionType.Clock)
                }
            );

            public IPin D09 => new Pin(
                "D09", "PC6",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC6", interruptGroup: 6)
                }
            );

            public IPin D10 => new Pin(
                "D10", "PC7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC7", interruptGroup: 7)
                }
            );

            public IPin D11 => new Pin(
                "D11", "PC9",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC9", interruptGroup: 9)
                }
            );

            public IPin D12 => new Pin(
                "D12", "PB14",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB14", interruptGroup: 14), 
                    new UartChannelInfo("COM1_TX", SerialDirectionType.Transmit)
                }
            );

            public IPin D13 => new Pin(
                "D13", "PB15",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB15", interruptGroup: 15), 
                    new UartChannelInfo("COM11_RX", SerialDirectionType.Receive)
                }
            );

            public IPin D14 => new Pin(
                "D14", "PB12",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PB12", interruptGroup: 12) 
                }
            );

            // TODO: shared with SDMMC2_D3?
            public IPin D15 => new Pin(
                "D15", "PG12",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PG12", interruptGroup: 12)
                }
            );

            // ==== ESP32 ====
            public IPin ESP_COPI => new Pin(
                "ESP_MOSI", "PI3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI3"),
                    new SpiChannelInfo("PI3", SpiLineType.MOSI)
                }
            );

            public IPin ESP_CIPO => new Pin(
                "ESP_MISO", "PI2",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI2", interruptGroup: 12),
                    new SpiChannelInfo("PI2", SpiLineType.MISO)
                }
            );

            public IPin ESP_CLK => new Pin(
                "ESP_CLK", "PD3",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PD3", interruptGroup: 3),
                    new SpiChannelInfo("PD3", SpiLineType.Clock)
                }
            );

            public IPin ESP_CS => new Pin(
                "ESP_CS", "PI0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI0", interruptGroup: 0),
                    new SpiChannelInfo("PI10", SpiLineType.ChipSelect)
                }
            );

            public IPin ESP_BOOT => new Pin(
                "ESP_BOOT", "PI10",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PI10", interruptGroup: 10),
                }
            );

            public IPin ESP_RST => new Pin(
                "ESP_RST", "PF7",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PF7", interruptGroup: 7),
                }
            );

            public IPin ESP_UART5_RX => new Pin(
                "ESP_UART5_RX", "PD2",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PD2", interruptGroup: 2),
                }
            );

            public IPin ESP_UART5_TX => new Pin(
                "ESP_UART5_TX", "PC12",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("PC12", interruptGroup: 12),
                }
            );

            // ==== ALIASES ====
            public IPin I2C_SDA => D07;
            public IPin I2C_SCL => D08;
        }
    }
}
