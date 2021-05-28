using System;
namespace Meadow.Hardware
{
    // really wish we could make this internal. 
    public interface IEspCoprocessorPinout
    {
        IPin ESP_MOSI => ESP_COPI;
        IPin ESP_COPI { get; }
        IPin ESP_MISO => ESP_CIPO;
        IPin ESP_CIPO { get; }
        IPin ESP_CLK { get; }
        IPin ESP_CS { get; }
        IPin ESP_BOOT { get; }
        IPin ESP_RST { get; }
        IPin ESP_UART_RX { get; }
        IPin ESP_UART_TX { get; }
    }
}
