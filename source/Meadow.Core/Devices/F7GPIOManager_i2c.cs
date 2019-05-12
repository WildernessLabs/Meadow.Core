using System;
using System.Threading;
using static Meadow.Core.Interop;
using static Meadow.Core.Interop.STM32;

namespace Meadow.Devices
{
    public partial class F7GPIOManager
    {
        /*
    base       = STM32_I2C1_BASE,
      .clk_bit    = RCC_APB1ENR_I2C1EN,
      .reset_bit  = RCC_APB1RSTR_I2C1RST,
      .scl_pin    = GPIO_I2C1_SCL,
      .sda_pin    = GPIO_I2C1_SDA,
    #ifndef CONFIG_I2C_POLLED
      .ev_irq     = STM32_IRQ_I2C1EV,
      .er_irq     = STM32_IRQ_I2C1ER
      */

        private const int SCL_PIN = 6;
        private const int SDA_PIN = 7;

        private void ConfigureI2CGPIOs(bool forI2C)
        {
            if (forI2C)
            {
                // Configure pins
                // #define GPIO_I2C1_SCL_1       (GPIO_ALT|GPIO_AF4 |GPIO_SPEED_50MHz|GPIO_OPENDRAIN|GPIO_PORTB|GPIO_PIN6)
                // #define GPIO_I2C1_SDA_1       (GPIO_ALT|GPIO_AF4 |GPIO_SPEED_50MHz|GPIO_OPENDRAIN|GPIO_PORTB|GPIO_PIN7)
                ConfigureGpio(GpioPort.PortB,
                    SCL_PIN,
                    GpioMode.AlternateFunction,
                    ResistorMode.Float,
                    GPIOSpeed.Speed_50MHz,
                    OutputType.OpenDrain,
                    false,
                    Hardware.InterruptMode.None);

                ConfigureGpio(STM32.GpioPort.PortB,
                    SDA_PIN,
                    GpioMode.AlternateFunction,
                    ResistorMode.Float,
                    GPIOSpeed.Speed_50MHz,
                    OutputType.OpenDrain,
                    false,
                    Hardware.InterruptMode.None);
            }
            else
            { // make them outputs (we'll drive them manually, e.g. in a reset)
                ConfigureGpio(GpioPort.PortB,
                    SCL_PIN,
                    GpioMode.Output,
                    ResistorMode.Float,
                    GPIOSpeed.Speed_50MHz,
                    OutputType.OpenDrain,
                    false,
                    Hardware.InterruptMode.None);

                ConfigureGpio(STM32.GpioPort.PortB,
                    SDA_PIN,
                    GpioMode.Output,
                    ResistorMode.Float,
                    GPIOSpeed.Speed_50MHz,
                    OutputType.OpenDrain,
                    false,
                    Hardware.InterruptMode.None);
            }
        }

        /// <summary>
        /// Initializes the I2C1 interface of the Meadow
        /// </summary>
        private void I2CInit()
        {
            // Power-up and configure GPIOs

            //Enable power and reset the peripheral
            Nuttx.UpdateRegister(DriverHandle, RCC_BASE + RCC_APB1ENR_OFFSET, 0, RCC_APB1ENR_I2C1EN);
            Nuttx.UpdateRegister(DriverHandle, RCC_BASE + RCC_APB1RSTR_OFFSET, 0, RCC_APB1RSTR_I2C1RST);
            Nuttx.UpdateRegister(DriverHandle, RCC_BASE + RCC_APB1RSTR_OFFSET, RCC_APB1RSTR_I2C1RST, 0);

            ConfigureI2CGPIOs(true);


            /*
            // Attach ISRs

            irq_attach(priv->config->ev_irq, stm32_i2c_isr, priv);
            irq_attach(priv->config->er_irq, stm32_i2c_isr, priv);
            up_enable_irq(priv->config->ev_irq);
            up_enable_irq(priv->config->er_irq);
            */

            // Set peripheral frequency, where it must be at least 2 MHz  for 100 kHz
            // or 4 MHz for 400 kHz.  This also disables all I2C interrupts.
            Nuttx.SetRegister(DriverHandle, MEADOW_I2C1_BASE + I2C_CR2_OFFSET, PCLK1_FREQUENCY / 1000000);

            // Force a frequency update
            I2CSetClock(100000);
        }

        private SemaphoreSlim m_i2cSemaphore = new SemaphoreSlim(1, 1);
        private uint m_i2cFrequency = 0;

        private void I2CSetClock(uint frequency)
        {
            uint presc, scl_delay, sda_delay, scl_h_period, scl_l_period;

            if (m_i2cFrequency == frequency) return;

            if (frequency == 100000)
            {
                presc = 0;
                scl_delay = 5;
                sda_delay = 0;
                scl_h_period = 61;
                scl_l_period = 89;

            }
            else if (frequency == 400000)
            {
                presc = 0;
                scl_delay = 3;
                sda_delay = 0;
                scl_h_period = 6;
                scl_l_period = 24;
            }
            else if (frequency == 1000000)
            {
                presc = 0;
                scl_delay = 2;
                sda_delay = 0;
                scl_h_period = 1;
                scl_l_period = 5;
            }
            else
            {
                presc = 7;
                scl_delay = 0;
                sda_delay = 0;
                scl_h_period = 35;
                scl_l_period = 162;
            }

            uint timingr =
              (presc << I2C_TIMINGR_PRESC_SHIFT) |
              (scl_delay << I2C_TIMINGR_SCLDEL_SHIFT) |
              (sda_delay << I2C_TIMINGR_SDADEL_SHIFT) |
              (scl_h_period << I2C_TIMINGR_SCLH_SHIFT) |
              (scl_l_period << I2C_TIMINGR_SCLL_SHIFT);

            Nuttx.SetRegister(DriverHandle, MEADOW_I2C1_BASE + I2C_TIMINGR_OFFSET, timingr);

            m_i2cFrequency = frequency;

            // Enable I2C peripheral
            Nuttx.UpdateRegister(DriverHandle, MEADOW_I2C1_BASE + I2C_CR1_OFFSET, 0, I2C_CR1_PE);
        }

        internal bool I2CReset()
        {
            uint clock_count;
            uint stretch_count;
            uint frequency; 

            if(!m_i2cSemaphore.Wait(1000))
            {
                return false;
            }

            try
            {
                // Save the current frequency
                frequency = m_i2cFrequency;

                // TODO: De-init the port
                //  stm32_i2c_deinit(priv);

                // Use GPIO configuration to un-wedge the bus
                ConfigureI2CGPIOs(false);

                // Let SDA go high
                SetDiscrete(GPIOB_BASE, GpioPort.PortB, SDA_PIN, true);

                // Clock the bus until any slaves currently driving it let it go.
                clock_count = 0;

                while (!GetDiscrete(GPIOB_BASE, GpioPort.PortB, SDA_PIN))
                {
                    // Give up if we have tried too hard
                    if (clock_count++ > 10)
                    {
                        return false;
                    }

                    // Sniff to make sure that clock stretching has finished.
                    // If the bus never relaxes, the reset has failed.
                    stretch_count = 0;
                    while (!GetDiscrete(GPIOB_BASE, GpioPort.PortB, SCL_PIN))
                    {
                        // Give up if we have tried too hard
                        if (stretch_count++ > 10)
                        {
                            return false;
                        }

                        Thread.Sleep(10);
                    }

                    // Drive SCL low
                    SetDiscrete(GPIOB_BASE, GpioPort.PortB, SCL_PIN, false);
                    Thread.Sleep(10);

                    // Drive SCL high again
                    SetDiscrete(GPIOB_BASE, GpioPort.PortB, SCL_PIN, true);
                    Thread.Sleep(10);

                }

                // Generate a start followed by a stop to reset slave
                // state machines
                SetDiscrete(GPIOB_BASE, GpioPort.PortB, SDA_PIN, false);
                Thread.Sleep(10);
                SetDiscrete(GPIOB_BASE, GpioPort.PortB, SCL_PIN, false);
                Thread.Sleep(10);
                SetDiscrete(GPIOB_BASE, GpioPort.PortB, SCL_PIN, true);
                Thread.Sleep(10);
                SetDiscrete(GPIOB_BASE, GpioPort.PortB, SDA_PIN, true);
                Thread.Sleep(10);

                // Revert the GPIO configuration.
                ConfigureI2CGPIOs(true);

                // Re-init the port
                I2CInit();

                // Restore the frequency
                I2CSetClock(frequency);

                return true;

            }
            finally
            {
                m_i2cSemaphore.Release();
            }
        }
    }
}