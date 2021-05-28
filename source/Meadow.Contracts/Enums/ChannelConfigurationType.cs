namespace Meadow.Hardware
{
    /// <summary>
    /// TODO: revisit this structure. Ultimately, it would be nice to know, specifically
    /// what a channel is cofigured for, i.e. DigitalInput, I2C TX, UART RX, etc.
    /// </summary>
    public enum ChannelConfigurationType
    {
        None,
        DigitalOutput,
        DigitalInput,
        AnalogOutput,
        AnalogInput,
        PWM,
        SPI,
        I2C,
        CAN,
        UART
    }

}

