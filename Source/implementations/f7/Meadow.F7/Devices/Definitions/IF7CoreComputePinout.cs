namespace Meadow.Hardware;

/// <summary>
/// Defines the contract for a Meadow IF7CoreCompute device's pinout
/// </summary>
public interface IF7CoreComputePinout : I32PinFeatherBoardPinout, IPinDefinitions
{
    /// <summary>
    /// The pin connected to I2C1's SCL line
    /// </summary>
    IPin I2C1_SCL { get; }
    /// <summary>
    /// The pin connected to I2C1's SDA line
    /// </summary>
    IPin I2C1_SDA { get; }
    /// <summary>
    /// The pin connected to I2C3's SCL line
    /// </summary>
    IPin I2C3_SCL { get; }
    /// <summary>
    /// The pin connected to I2C3's SDA line
    /// </summary>
    IPin I2C3_SDA { get; }
    /// <summary>
    /// The pin connected to SPI3's SCK line
    /// </summary>
    IPin SPI3_SCK { get; }
    /// <summary>
    /// The pin connected to SPI3's COPI (MOSI) line
    /// </summary>
    IPin SPI3_COPI { get; }
    /// <summary>
    /// The pin connected to SPI3's CIPO (MISO) line
    /// </summary>
    IPin SPI3_CIPO { get; }
    /// <summary>
    /// The pin connected to SPI5's SCK line
    /// </summary>
    IPin SPI5_SCK { get; }
    /// <summary>
    /// The pin connected to SPI5's COPI (MOSI) line
    /// </summary>
    IPin SPI5_COPI { get; }
    /// <summary>
    /// The pin connected to SPI5's CIPO (MISO) line
    /// </summary>
    IPin SPI5_CIPO { get; }
    /// <summary>
    /// The pin connected to the D16 line
    /// </summary>
    IPin D16 { get; }
    /// <summary>
    /// The pin connected to the D17 line
    /// </summary>
    IPin D17 { get; }
    /// <summary>
    /// The pin connected to the D18 line
    /// </summary>
    IPin D18 { get; }
    /// <summary>
    /// The pin connected to the D19 line
    /// </summary>
    IPin D19 { get; }
    /// <summary>
    /// The pin connected to the D20 line
    /// </summary>
    IPin D20 { get; }

    /// <summary>
    /// The pin connected to the PA0 line
    /// </summary>
    IPin PA0 { get; }
    /// <summary>
    /// The pin connected to the PA1_ETH_REF_CLK line
    /// </summary>
    IPin PA1_ETH_REF_CLK { get; }
    /// <summary>
    /// The pin connected to the PA2_ETH_MDIO line
    /// </summary>
    IPin PA2_ETH_MDIO { get; }
    /// <summary>
    /// The pin connected to the PA3 line
    /// </summary>
    IPin PA3 { get; }
    /// <summary>
    /// The pin connected to the PA4 line
    /// </summary>
    IPin PA4 { get; }
    /// <summary>
    /// The pin connected to the PA5 line
    /// </summary>
    IPin PA5 { get; }
    /// <summary>
    /// The pin connected to the PA7_ETH_CRS_DV line
    /// </summary>
    IPin PA7_ETH_CRS_DV { get; }
    /// <summary>
    /// The pin connected to the PA9 line
    /// </summary>
    IPin PA9 { get; }
    /// <summary>
    /// The pin connected to the PA10 line
    /// </summary>
    IPin PA10 { get; }
    /// <summary>
    /// The pin connected to the PA13 line
    /// </summary>
    IPin PA13 { get; }
    /// <summary>
    /// The pin connected to the PA14 line
    /// </summary>
    IPin PA14 { get; }
    /// <summary>
    /// The pin connected to the PA15 line
    /// </summary>
    IPin PA15 { get; }
    /// <summary>
    /// The pin connected to the PB0 line
    /// </summary>
    IPin PB0 { get; }
    /// <summary>
    /// The pin connected to the PB1 line
    /// </summary>
    IPin PB1 { get; }
    /// <summary>
    /// The pin connected to the PB3 line
    /// </summary>
    IPin PB3 { get; }
    /// <summary>
    /// The pin connected to the PB4 line
    /// </summary>
    IPin PB4 { get; }
    /// <summary>
    /// The pin connected to the PB5 line
    /// </summary>
    IPin PB5 { get; }
    /// <summary>
    /// The pin connected to the PB6 line
    /// </summary>
    IPin PB6 { get; }
    /// <summary>
    /// The pin connected to the PB7 line
    /// </summary>
    IPin PB7 { get; }
    /// <summary>
    /// The pin connected to the PB8 line
    /// </summary>
    IPin PB8 { get; }
    /// <summary>
    /// The pin connected to the PB9 line
    /// </summary>
    IPin PB9 { get; }
    /// <summary>
    /// The pin connected to the PB11_ETH_TX_EN line
    /// </summary>
    IPin PB11_ETH_TX_EN { get; }
    /// <summary>
    /// The pin connected to the PB12 line
    /// </summary>
    IPin PB12 { get; }
    /// <summary>
    /// The pin connected to the PB13 line
    /// </summary>
    IPin PB13 { get; }
    /// <summary>
    /// The pin connected to the PB14 line
    /// </summary>
    IPin PB14 { get; }
    /// <summary>
    /// The pin connected to the PB15 line
    /// </summary>
    IPin PB15 { get; }
    /// <summary>
    /// The pin connected to the PC0 line
    /// </summary>
    IPin PC0 { get; }
    /// <summary>
    /// The pin connected to the PC1_ETH_MDC line
    /// </summary>
    IPin PC1_ETH_MDC { get; }
    /// <summary>
    /// The pin connected to the PC2 line
    /// </summary>
    IPin PC2 { get; }
    /// <summary>
    /// The pin connected to the PC4_ETH_RXD0 line
    /// </summary>
    IPin PC4_ETH_RXD0 { get; }
    /// <summary>
    /// The pin connected to the PC5_ETH_RXD1 line
    /// </summary>
    IPin PC5_ETH_RXD1 { get; }
    /// <summary>
    /// The pin connected to the PC6 line
    /// </summary>
    IPin PC6 { get; }
    /// <summary>
    /// The pin connected to the PC7 line
    /// </summary>
    IPin PC7 { get; }
    /// <summary>
    /// The pin connected to the PC8 line
    /// </summary>
    IPin PC8 { get; }
    /// <summary>
    /// The pin connected to the PC9 line
    /// </summary>
    IPin PC9 { get; }
    /// <summary>
    /// The pin connected to the PC10 line
    /// </summary>
    IPin PC10 { get; }
    /// <summary>
    /// The pin connected to the PC11 line
    /// </summary>
    IPin PC11 { get; }
    /// <summary>
    /// The pin connected to the PD5 line
    /// </summary>
    IPin PD5 { get; }
    /// <summary>
    /// The pin connected to the PD6_SDMMC_CLK line
    /// </summary>
    IPin PD6_SDMMC_CLK { get; }
    /// <summary>
    /// The pin connected to the PD7_SDMMC_CMD line
    /// </summary>
    IPin PD7_SDMMC_CMD { get; }
    /// <summary>
    /// The pin connected to the PF8 line
    /// </summary>
    IPin PF8 { get; }
    /// <summary>
    /// The pin connected to the PF9 line
    /// </summary>
    IPin PF9 { get; }
    /// <summary>
    /// The pin connected to the PG6_SDMMC_IN_L line
    /// </summary>
    IPin PG6_SDMMC_IN_L { get; }
    /// <summary>
    /// The pin connected to the PG9_SDMMC_D0 line
    /// </summary>
    IPin PG9_SDMMC_D0 { get; }
    /// <summary>
    /// The pin connected to the PG10_SDMMC_D1 line
    /// </summary>
    IPin PG10_SDMMC_D1 { get; }
    /// <summary>
    /// The pin connected to the PG11_SDMMC_D2 line
    /// </summary>
    IPin PG11_SDMMC_D2 { get; }
    /// <summary>
    /// The pin connected to the PG12_SDMMC_D3 line
    /// </summary>
    IPin PG12_SDMMC_D3 { get; }
    /// <summary>
    /// The pin connected to the PG13_ETH_TXD0 line
    /// </summary>
    IPin PG13_ETH_TXD0 { get; }
    /// <summary>
    /// The pin connected to the PG14_ETH_TXD1 line
    /// </summary>
    IPin PG14_ETH_TXD1 { get; }
    /// <summary>
    /// The pin connected to the PH6 line
    /// </summary>
    IPin PH6 { get; }
    /// <summary>
    /// The pin connected to the PH7 line
    /// </summary>
    IPin PH7 { get; }
    /// <summary>
    /// The pin connected to the PH8 line
    /// </summary>
    IPin PH8 { get; }
    /// <summary>
    /// The pin connected to the PH10 line
    /// </summary>
    IPin PH10 { get; }
    /// <summary>
    /// The pin connected to the PH12 line
    /// </summary>
    IPin PH12 { get; }
    /// <summary>
    /// The pin connected to the PH13 line
    /// </summary>
    IPin PH13 { get; }
    /// <summary>
    /// The pin connected to the PH14_ETH_IRQ line
    /// </summary>
    IPin PH14_ETH_IRQ { get; }
    /// <summary>
    /// The pin connected to the PI9 line
    /// </summary>
    IPin PI9 { get; }
    /// <summary>
    /// The pin connected to the PI11 line
    /// </summary>
    IPin PI11 { get; }
}
