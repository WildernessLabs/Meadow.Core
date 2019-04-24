using System;
using System.Collections.Generic;
using Meadow.Core;
using Meadow.Hardware;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    public partial class F7GPIOManager
    {
        private const int TIM3_BASE_ADDRESS = 0x40000400;
        private const int TIM4_BASE_ADDRESS = 0x40000800;
        private const int TIM5_BASE_ADDRESS = 0x40000C00;
        private const int RCC_BASE_ADDRESS = 0x40023800;

        // see reference manual, page 1028-ish
        private const int TIMx_CR1_OFFSET = 0x00;
        private const int TIMx_CR2_OFFSET = 0x04;
        private const int TIMx_SMCR_OFFSET = 0x08;
        private const int TIMx_EGR_OFFSET = 0x14;
        private const int TIMx_CCMR1_OFFSET = 0x18; // capture/compare mode register 1
        private const int TIMx_CCER_OFFSET = 0x20;  // capture/compare enable
        private const int TIMx_ARR_OFFSET = 0x2C;   // auto-load register
        private const int TIMx_PSC_OFFSET = 0x28;
        private const int TIMx_CCR1_OFFSET = 0x34;  // capture/compare register 1

        private const int RCC_AHB1RSTR_OFFSET = 0x10;
        private const int RCC_AHB1ENR_OFFSET = 0x30;

        private const uint RCC_AHB1ENR_GPIOAEN = 1 << 0;
        private const uint RCC_AHB1ENR_GPIOBEN = 1 << 1;
        private const uint RCC_AHB1ENR_GPIOCEN = 1 << 2;

        private const int UG_SHIFT = 0;

        private const int OC1PE_SHIFT = 3;
        private const int OC1M_SHIFT = 4;

        private const int CC1E_SHIFT = 0;
        private const int CC1P_SHIFT = 1;

        private const int PWM_MODE_1 = 6;

        private const int ARPE_SHIFT = 7;

        private const uint TIM_SMCR_SMS = ((1 << 16) | 7);
        private const uint TIM_SMCR_TS = 0x70;
        private const uint TIM_SMCR_ETF = 0x0f << 8;
        private const uint TIM_SMCR_ETPS = 0x03 << 12;
        private const uint TIM_SMCR_ECE = 1 << 14;  // external clock enable
        private const uint TIM_SMCR_ETP = 1 << 15;  // external trigger polarity

        private const uint TIM_CCER_CC1E = 1 << 0;
        private const uint TIM_CCER_CC1P = 1 << 1;

        private const uint TIM_CCMR1_CC1S = 0x03 << 0;
        private const uint TIM_CCMR1_OC1FE = 1 << 2;
        private const uint TIM_CCMR1_OC1PE = 1 << 3;
        private const uint TIM_CCMR1_OC1M = 0x07 << 4;

        private const uint TIM_CR1_CEN = 1 << 0;
        private const uint TIM_CR1_DIR = 1 << 4;
        private const uint TIM_CR1_CMS = 3 << 5;
        private const uint TIM_CR1_ARPE = 1 << 7;
        private const uint TIM_CR1_CKD = 3 << 8;

        private const uint TIM_CR2_MMS = 0x07 << 4;

        private const uint TIM_EGR_UG = 1 << 0;

        private const uint TIM_COUNTERMODE_UP = 0;
        private const uint TIM_CLOCKDIVISION_DIV1 = 0;
        private const uint TIM_AUTORELOAD_PRELOAD_DISABLE = 0;
        private const uint TIM_OCMODE_PWM1 = 0x06;
        private const uint TIM_OCPOLARITY_HIGH = 0;
        private const uint TIM_OCFAST_DISABLE = 0;

        private void TIMxInit(uint timerBaseAddress, uint prescaler, uint period, uint counterMode, uint autoReloadPreload)
        {
            // time base config
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CR1_OFFSET, out uint cr1);

            cr1 &= ~(TIM_CR1_DIR | TIM_CR1_CMS);
            cr1 |= counterMode;

            // Set tcr1_dirhe auto-reload preload
            cr1 &= ~(TIM_CR1_ARPE);
            cr1 |= autoReloadPreload;

            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CR1_OFFSET, cr1);

            // Set the Auto-reload value
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_ARR_OFFSET, period);

            // Set the Prescaler value
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_PSC_OFFSET, prescaler);

            // Generate an update event to reload the Prescaler 
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_EGR_OFFSET, TIM_EGR_UG);
        }

        private void TIMxConfigClockSource(uint timerBaseAddress)
        {
            //  Reset the SMS, TS, ECE, ETPS and ETRF bits
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_SMCR_OFFSET, out uint smcr);
            smcr &= ~(TIM_SMCR_SMS | TIM_SMCR_TS);
            smcr &= ~(TIM_SMCR_ETF | TIM_SMCR_ETPS | TIM_SMCR_ECE | TIM_SMCR_ETP);
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_SMCR_OFFSET, smcr);

            // NOTE: this assumes clock source is TIM_CLOCKSOURCE_INTERNAL
            // Disable slave mode to clock the prescaler directly with the internal clock
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_SMCR_OFFSET, out smcr);
            smcr &= ~TIM_SMCR_SMS;
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_SMCR_OFFSET, smcr);
        }

        public void EnablePC6PWM()
        {
            Console.WriteLine("+EnablePC6PWM");

            // this is purely a hard-coded, straight-line test to generate a PWM on a known channel.  
            // It will get refactored once it's working

            var gpioPort = 3;
            uint gpioPin = 6;
            uint period = 4095;
            uint prescaler = 0;
            var counterMode = TIM_COUNTERMODE_UP;
            var clockDivision = TIM_CLOCKDIVISION_DIV1;
            var autoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;

            uint pulse = 3000;

            // enable GPIO port C clock
            Nuttx.TryGetRegister(DriverHandle, RCC_BASE_ADDRESS + RCC_AHB1ENR_OFFSET, out uint rcc);
            rcc |= RCC_AHB1ENR_GPIOCEN;
            Nuttx.SetRegister(DriverHandle, RCC_BASE_ADDRESS + RCC_AHB1ENR_OFFSET, rcc);

            // do a reset on the GPIO
            Nuttx.SetRegister(DriverHandle, STM32.GPIOC_BASE + STM32.STM32_GPIO_BSRR_OFFSET, gpioPin << 16);

            // set the alternate function
            ConfigureGpio(STM32.GpioPort.PortC, 6, STM32.GpioMode.AlternateFunction, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false, InterruptMode.None);


            //// Base initialization
            TIMxInit(TIM3_BASE_ADDRESS, prescaler, period, counterMode, autoReloadPreload);





            /*
            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CR1_OFFSET, out uint cr1);
            cr1 &= ~TIM_CR1_CKD;
            cr1 |= TIM_CLOCKDIVISION_DIV1;

            //MODIFY_REG(tmpcr1, TIM_CR1_ARPE, Structure->AutoReloadPreload);

            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CR1_OFFSET, cr1);

            // Set the Auto-reload value
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_ARR_OFFSET, period);

            // Set the Prescaler value
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_PSC_OFFSET, prescaler);
            */


            //// Configure Clock Source
            TIMxConfigClockSource(TIM3_BASE_ADDRESS);
            /*
            //  Reset the SMS, TS, ECE, ETPS and ETRF bits
            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_SMCR_OFFSET, out uint smcr);
            smcr &= ~(TIM_SMCR_SMS | TIM_SMCR_TS);
            smcr &= ~(TIM_SMCR_ETF | TIM_SMCR_ETPS | TIM_SMCR_ECE | TIM_SMCR_ETP);
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_SMCR_OFFSET, smcr);
            */
            //// MasterConfigSynchronization
            /* these are all NOP (zeros)
            /* Reset the MMS Bits 
            cr2 &= ~TIM_CR2_MMS;
            /* Select the TRGO source 
            tcr2 |= sMasterConfig->MasterOutputTrigger;     

            /* Reset the MSM Bit 
            tmpsmcr &= ~TIM_SMCR_MSM;
            /* Set master mode 
            tmpsmcr |= sMasterConfig->MasterSlaveMode;
            */

            //// HAL_TIM_PWM_ConfigChannel
            // Get the TIMx CCER register value
            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCER_OFFSET, out uint ccer);

            // Get the TIMx CR2 register value
            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CR2_OFFSET, out uint cr2);

            // Get the TIMx CCMR1 register value
            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR1_OFFSET, out uint ccmr1);

            // Disable the Channel 1: Reset the CC1E Bit
            ccer &= ~TIM_CCER_CC1E;

            // Reset the Output Compare Mode Bits
            ccmr1 &= ~TIM_CCMR1_OC1M;
            ccmr1 &= ~TIM_CCMR1_CC1S;
            // Select the Output Compare Mode 
            ccmr1 |= TIM_OCMODE_PWM1;

            // Reset the Output Polarity level 
            ccer &= ~TIM_CCER_CC1P;
            // Set the Output Compare Polarity 
            ccer |= TIM_OCPOLARITY_HIGH;


            /* Write to TIMx CR2 */
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CR2_OFFSET, cr2);

            /* Write to TIMx CCMR1 */
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR1_OFFSET, ccmr1);

            /* Set the Capture Compare Register value */
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCR1_OFFSET, pulse);

            /* Write to TIMx CCER */
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCER_OFFSET, ccer);



            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR1_OFFSET, out ccmr1);
            ccmr1 |= TIM_CCMR1_OC1PE;
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR1_OFFSET, ccmr1);

            /* Configure the Output Fast mode */
            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR1_OFFSET, out ccmr1);
            ccmr1 &= ~TIM_CCMR1_OC1FE;
            ccmr1 |= TIM_OCFAST_DISABLE;
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR1_OFFSET, ccmr1);




            /*

            // 1) set the PWM frequency in TIMx_ARR
            // right now 0xffff is just some unknown frequency.  I *think* slowest possible?
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_ARR_OFFSET, 0xffff);

            // 2) set the duty cycle in TIMxCCR1
            // right now 0xffff is just some unknown duty cycle.
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCR1_OFFSET, 0xffff);

            // 3) Set mode to PWM mode 1 by setting OC1M[2:0] (output compare mode bits) in TIMx_CCMR1 to '110'
            uint update = PWM_MODE_1 << OC1M_SHIFT;
            Nuttx.UpdateRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR1_OFFSET, 0, update);

            // 4) Set polarity to active high in CCER by setting CC1P to 0
            update = 1 << CC1P_SHIFT;
            Nuttx.UpdateRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR1_OFFSET, update, 0);

            // 5) Enable preload in CCMR1 by setting OC1PE to 1
            update = 1 << OC1PE_SHIFT;
            Nuttx.UpdateRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR1_OFFSET, 0, update);

            // 6) Initialize registers to 0 by setting UG to 1 in TIMxEGR
            update = 1 << UG_SHIFT;
            Nuttx.UpdateRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_EGR_OFFSET, 0, update);

            // 7) Turn on auto-preload in TIMx_CCR1 by setting ARPE to 1
            update = 1 << ARPE_SHIFT;
            Nuttx.UpdateRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CR1_OFFSET, 0, update);

            // 8) Enable output in TIMx_CCER by setting CC1E to 1
            update = 1 << CC1E_SHIFT;
            Nuttx.UpdateRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCER_OFFSET, 0, update);
            */

            // 9) set GPIO to alternate function 2
            ConfigureGpio(STM32.GpioPort.PortC, 6, STM32.GpioMode.AlternateFunction, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false, InterruptMode.None);

            Console.WriteLine("-EnablePC6PWM");
        }
    }
}
