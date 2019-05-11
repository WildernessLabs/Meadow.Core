using System;
using System.Collections.Generic;
using Meadow.Core;
using Meadow.Hardware;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    public partial class F7GPIOManager
    {
        private const int TIM1_BASE_ADDRESS = 0x40010000;
        private const int TIM8_BASE_ADDRESS = 0x40010400;
        private const int TIM2_BASE_ADDRESS = 0x40000000;
        private const int TIM3_BASE_ADDRESS = 0x40000400;
        private const int TIM4_BASE_ADDRESS = 0x40000800;
        private const int TIM5_BASE_ADDRESS = 0x40000C00;
        private const int TIM6_BASE_ADDRESS = 0x40001000;
        private const int TIM7_BASE_ADDRESS = 0x40001400;
        private const int RCC_BASE_ADDRESS = 0x40023800;

        // see reference manual, page 1028-ish
        private const int TIMx_CR1_OFFSET = 0x00;
        private const int TIMx_CR2_OFFSET = 0x04;
        private const int TIMx_SMCR_OFFSET = 0x08;
        private const int TIMx_EGR_OFFSET = 0x14;
        private const int TIMx_CCMR1_OFFSET = 0x18; // capture/compare mode register 1
        private const int TIMx_CCMR2_OFFSET = 0x1c;
        private const int TIMx_CCER_OFFSET = 0x20;  // capture/compare enable
        private const int TIMx_ARR_OFFSET = 0x2C;   // auto-load register
        private const int TIMx_PSC_OFFSET = 0x28;
        private const int TIMx_CCR1_OFFSET = 0x34;  // capture/compare register 1
        private const int TIMx_CCR2_OFFSET = 0x38;
        private const int TIMx_CCR3_OFFSET = 0x3c;
        private const int TIMx_CCR4_OFFSET = 0x40;
        private const int TIMx_BDTR_OFFSET = 0x44;

        private const int RCC_AHB1RSTR_OFFSET = 0x10;
        private const int RCC_AHB1ENR_OFFSET = 0x30;
        private const int RCC_APB1ENR_OFFSET = 0x40;
        private const int RCC_APB2ENR_OFFSET = 0x44;

        private const uint RCC_AHB1ENR_GPIOAEN = 1 << 0;
        private const uint RCC_AHB1ENR_GPIOBEN = 1 << 1;
        private const uint RCC_AHB1ENR_GPIOCEN = 1 << 2;

        private const uint RCC_APB1ENR_TIM2EN = 1 << 0;
        private const uint RCC_APB1ENR_TIM3EN = 1 << 1;
        private const uint RCC_APB1ENR_TIM4EN = 1 << 2;
        private const uint RCC_APB1ENR_TIM5EN = 1 << 3;
        private const uint RCC_APB1ENR_TIM6EN = 1 << 4;
        private const uint RCC_APB1ENR_TIM7EN = 1 << 5;
        private const uint RCC_APB1ENR_TIM12EN = 1 << 6;
        private const uint RCC_APB1ENR_TIM13EN = 1 << 7;
        private const uint RCC_APB1ENR_TIM14EN = 1 << 8;

        private const uint RCC_APB2ENR_TIM1EN = 1 << 0;
        private const uint RCC_APB2ENR_TIM8EN = 1 << 1;
        private const uint RCC_APB2ENR_ADC1EN = 1 << 8;
        private const uint RCC_APB2ENR_ADC2EN = 1 << 9;
        private const uint RCC_APB2ENR_ADC3EN = 1 << 10;

        private const int UG_SHIFT = 0;

        private const int OC1PE_SHIFT = 3;
        private const int OC1M_SHIFT = 4;

        private const int CC1E_SHIFT = 0;
        private const int CC1P_SHIFT = 1;

        private const int PWM_MODE_1 = 6;

        private const int ARPE_SHIFT = 7;

        private const uint TIM_SMCR_SMS = ((1 << 16) | 7);
        private const uint TIM_SMCR_TS = 7 << 4;
        private const uint TIM_SMCR_MSM = 1 << 7;
        private const uint TIM_SMCR_ETF = 0x0f << 8;
        private const uint TIM_SMCR_ETPS = 0x03 << 12;
        private const uint TIM_SMCR_ECE = 1 << 14;  // external clock enable
        private const uint TIM_SMCR_ETP = 1 << 15;  // external trigger polarity

        private const uint TIM_CCER_CC1E = 1 << 0;
        private const uint TIM_CCER_CC1P = 1 << 1;
        private const uint TIM_CCER_CC1NP = 1 << 3;
        private const uint TIM_CCER_CC1NE = 1 << 2;

        private const uint TIM_CR2_OIS1 = 1 << 8;
        private const uint TIM_CR2_OIS1N = 1 << 9;

        private const uint TIM_CCMR1_CC1S = 0x03 << 0;
        private const uint TIM_CCMR1_CC2S = 0x03 << 8;

        private const uint TIM_CCMR1_OC1FE = 1 << 2;
        private const uint TIM_CCMR1_OC1PE = 1 << 3;
        private const uint TIM_CCMR1_OC1M = 0x1007 << 4;
        private const uint TIM_CCMR1_OC2M = 0x1007 << 12;
        private const uint TIM_CCMR1_OC2S = 0x03 << 8;
        private const uint TIM_CCMR1_OC2PE = 1 << 11;
        private const uint TIM_CCMR1_OC2FE = 1 << 10;

        private const uint TIM_CCMR2_CC3S = 0x03 << 0;
        private const uint TIM_CCMR2_OC3M = 0x1007 << 4;
        private const uint TIM_CCMR2_OC3PE = 1 << 3;
        private const uint TIM_CCMR2_OC3FE = 1 << 2;

        private const uint TIM_CCMR2_CC4S = 0x03 << 8;
        private const uint TIM_CCMR2_OC4M = 0x1007 << 12;
        private const uint TIM_CCMR2_OC4PE = 1 << 11;
        private const uint TIM_CCMR2_OC4FE = 1 << 10;

        private const uint TIM_CR1_CEN = 1 << 0;
        private const uint TIM_CR1_DIR = 1 << 4;
        private const uint TIM_CR1_CMS = 3 << 5;
        private const uint TIM_CR1_ARPE = 1 << 7;
        private const uint TIM_CR1_CKD = 3 << 8;

        private const uint TIM_CR2_MMS = 0x07 << 4;
        private const uint TIM_CR2_MMS2 = 1 << 6;
        private const uint TIM_CR2_OIS2 = 1 << 10;
        private const uint TIM_CR2_OIS2N = 1 << 11;
        private const uint TIM_CR2_OIS3 = 1 << 12;
        private const uint TIM_CR2_OIS3N = 1 << 13;

        private const uint TIM_CCER_CC2E = 1 << 4;
        private const uint TIM_CCER_CC2P = 1 << 5;
        private const uint TIM_CCER_CC2NE = 1 << 6;
        private const uint TIM_CCER_CC2NP = 1 << 7;
        private const uint TIM_CCER_CC3E = 1 << 8;
        private const uint TIM_CCER_CC3P = 1 << 9;
        private const uint TIM_CCER_CC3NE = 1 << 10;
        private const uint TIM_CCER_CC3NP = 1 << 11;
        private const uint TIM_CCER_CC4E = 1 << 12;
        private const uint TIM_CCER_CC4P = 1 << 13;
        private const uint TIM_CCER_CC4NP = 1 << 15;

        private const uint TIM_BDTR_MOE = 1 << 15;

        private const uint TIM_EGR_UG = 1 << 0;

        private const uint TIM_COUNTERMODE_UP = 0;
        private const uint TIM_CLOCKDIVISION_DIV1 = 0;
        private const uint TIM_AUTORELOAD_PRELOAD_DISABLE = 0;
        private const uint TIM_OCMODE_PWM1 = 0x06;
        private const uint TIM_OCPOLARITY_HIGH = 0;
        private const uint TIM_OCFAST_DISABLE = 0;
        private const uint TIM_MASTERSLAVEMODE_DISABLE = 0;
        private const uint TIM_OCIDLESTATE_RESET = 0;
        private const uint TIM_OCNIDLESTATE_RESET = 0;
        private const uint TIM_OCNPOLARITY_HIGH = 0;
        private const uint TIM_CCx_ENABLE = 1;
        private const uint TIM_CCx_DISABLE = 0;

        private const uint TIM_TRGO_RESET = 0;
        private const uint TIM_TRGO2_RESET = 0;

        private const uint TIM_CHANNEL_1 = 0;
        private const uint TIM_CHANNEL_2 = 4;
        private const uint TIM_CHANNEL_3 = 8;

        private void TIMxInit(uint timerBaseAddress, uint prescaler, uint period, uint counterMode, uint autoReloadPreload)
        {
            // time base config
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CR1_OFFSET, out uint cr1);

            // Select the Counter Mode
            cr1 &= ~(TIM_CR1_DIR | TIM_CR1_CMS);
            cr1 |= counterMode;

            // Set the clock division
            cr1 &= ~TIM_CR1_CKD;
            cr1 |= TIM_CLOCKDIVISION_DIV1;

            // Set the auto-reload preload
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

        private void TIMxMasterConfigSynchronization(uint timerBaseAddress, bool isTIM1or8)
        {
            // Get the TIMx CR2 register value
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CR2_OFFSET, out uint cr2);

            // Get the TIMx SMCR register value
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_SMCR_OFFSET, out uint smcr);

            // If the timer supports ADC synchronization through TRGO2, set the master mode selection 2 
            if (isTIM1or8) // IS_TIM_TRGO2_INSTANCE // this is for TIM1 and TIM8 - TIM8 *is* used by Meadow
            {
                // Clear the MMS2 bits
                cr2 &= ~TIM_CR2_MMS2;
                // Select the TRGO2 source
                cr2 |= TIM_TRGO2_RESET;
            }

            // Reset the MMS Bits
            cr2 &= ~TIM_CR2_MMS;
            // Select the TRGO source
            cr2 |= TIM_TRGO_RESET;

            // Reset the MSM Bit
            smcr &= ~TIM_SMCR_MSM;
            // Set master mode
            smcr |= TIM_MASTERSLAVEMODE_DISABLE;

            // Update TIMx CR2
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CR2_OFFSET, cr2);

            // Update TIMx SMCR
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_SMCR_OFFSET, smcr);
        }

        void TIM_OC1_SetConfig(uint timerBaseAddress, bool isTIM1or8, uint outputCompareMode, uint outputComparePolarity, uint pulse)
        {
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, out uint ccer);

            // Disable the Channel 1: Reset the CC1E Bit
            ccer &= ~TIM_CCER_CC1E;
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, ccer);

            // Get the TIMx CCER register value
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, out ccer);
            // Get the TIMx CR2 register value
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CR2_OFFSET, out uint cr2);

            // Get the TIMx CCMR1 register value 
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCMR1_OFFSET, out uint ccmrx);

            // Reset the Output Compare Mode Bits
            ccmrx &= ~TIM_CCMR1_OC1M;
            ccmrx &= ~TIM_CCMR1_CC1S;
            // Select the Output Compare Mode
            ccmrx |= outputCompareMode;

            // Reset the Output Polarity level
            ccer &= ~TIM_CCER_CC1P;
            // Set the Output Compare Polarity 
            ccer |= outputComparePolarity;

            if (isTIM1or8)
            {
                // Reset the Output N Polarity level
                ccer &= ~TIM_CCER_CC1NP;
                // Set the Output N Polarity
                ccer |= TIM_OCNPOLARITY_HIGH;
                // Reset the Output N State
                ccer &= ~TIM_CCER_CC1NE;

                // Reset the Output Compare and Output Compare N IDLE State
                cr2 &= ~TIM_CR2_OIS1;
                cr2 &= ~TIM_CR2_OIS1N;
                // Set the Output Idle state
                cr2 |= TIM_OCIDLESTATE_RESET;
                // Set the Output N Idle state
                cr2 |= TIM_OCIDLESTATE_RESET;
            }
            // Write to TIMx CR2
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CR2_OFFSET, cr2);

            // Write to TIMx CCMR1
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCMR1_OFFSET, ccmrx);

            /* Set the Capture Compare Register value */
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCR1_OFFSET, pulse);

            // Write to TIMx CCER
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, ccer);
        }

        void TIM_OC2_SetConfig(uint timerBaseAddress, bool isTIM1or8, uint outputCompareMode, uint outputComparePolarity, uint pulse)
        {
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, out uint ccer);

            // Disable the Channel 1: Reset the CC1E Bit
            ccer &= ~TIM_CCER_CC2E;
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, ccer);

            // Get the TIMx CCER register value
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, out ccer);
            // Get the TIMx CR2 register value
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CR2_OFFSET, out uint cr2);

            // Get the TIMx CCMR1 register value 
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCMR1_OFFSET, out uint ccmrx);

            // Reset the Output Compare Mode Bits
            ccmrx &= ~TIM_CCMR1_OC2M;
            ccmrx &= ~TIM_CCMR1_CC2S;
            // Select the Output Compare Mode
            ccmrx |= (outputCompareMode << 8);

            // Reset the Output Polarity level
            ccer &= ~TIM_CCER_CC2P;
            // Set the Output Compare Polarity 
            ccer |= (outputComparePolarity << 4);

            if (isTIM1or8)
            {
                // Reset the Output N Polarity level
                ccer &= ~TIM_CCER_CC2NP;
                // Set the Output N Polarity
                ccer |= (TIM_OCNPOLARITY_HIGH << 4);
                // Reset the Output N State
                ccer &= ~TIM_CCER_CC2NE;

                // Reset the Output Compare and Output Compare N IDLE State
                cr2 &= ~TIM_CR2_OIS2;
                cr2 &= ~TIM_CR2_OIS2N;
                // Set the Output Idle state
                cr2 |= (TIM_OCIDLESTATE_RESET << 2);
                // Set the Output N Idle state
                cr2 |= (TIM_OCIDLESTATE_RESET << 2);
            }
            // Write to TIMx CR2
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CR2_OFFSET, cr2);

            // Write to TIMx CCMR1
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCMR1_OFFSET, ccmrx);

            /* Set the Capture Compare Register value */
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCR2_OFFSET, pulse);

            // Write to TIMx CCER
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, ccer);
        }

        void TIM_OC3_SetConfig(uint timerBaseAddress, bool isTIM1or8, uint outputCompareMode, uint outputComparePolarity, uint pulse)
        {
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, out uint ccer);

            // Disable the Channel 3: Reset the CC1E Bit
            ccer &= ~TIM_CCER_CC3E;
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, ccer);

            // Get the TIMx CCER register value
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, out ccer);
            // Get the TIMx CR2 register value
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CR2_OFFSET, out uint cr2);

            // Get the TIMx CCMR2 register value 
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCMR2_OFFSET, out uint ccmrx);

            // Reset the Output Compare Mode Bits
            ccmrx &= ~TIM_CCMR2_OC3M;
            ccmrx &= ~TIM_CCMR2_CC3S;
            // Select the Output Compare Mode
            ccmrx |= outputCompareMode;

            // Reset the Output Polarity level
            ccer &= ~TIM_CCER_CC3P;
            // Set the Output Compare Polarity 
            ccer |= (outputComparePolarity << 8);

            if (isTIM1or8)
            {
                // Reset the Output N Polarity level
                ccer &= ~TIM_CCER_CC3NP;
                // Set the Output N Polarity
                ccer |= (TIM_OCNPOLARITY_HIGH << 8);
                // Reset the Output N State
                ccer &= ~TIM_CCER_CC3NE;

                // Reset the Output Compare and Output Compare N IDLE State
                cr2 &= ~TIM_CR2_OIS3;
                cr2 &= ~TIM_CR2_OIS3N;
                // Set the Output Idle state
                cr2 |= (TIM_OCIDLESTATE_RESET << 4);
                // Set the Output N Idle state
                cr2 |= (TIM_OCIDLESTATE_RESET << 4);
            }
            // Write to TIMx CR2
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CR2_OFFSET, cr2);

            // Write to TIMx CCMR1
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCMR2_OFFSET, ccmrx);

            /* Set the Capture Compare Register value */
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCR3_OFFSET, pulse);

            // Write to TIMx CCER
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, ccer);
        }

        private void TIMxPWMConfigChannel(uint timerBaseAddress, uint channel)
        {
            var ocMode = TIM_OCMODE_PWM1;
            var pulse = 3000u;
            var ocPolarity = TIM_OCPOLARITY_HIGH;
            var fastMode = TIM_OCFAST_DISABLE;

            var isTIM1or8 = timerBaseAddress == TIM8_BASE_ADDRESS;

            switch (channel)
            {
                case TIM_CHANNEL_1:
                    {
                        // Configure the Channel 1 in PWM mode
                        TIM_OC1_SetConfig(timerBaseAddress, isTIM1or8, ocMode, ocPolarity, pulse);

                        Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCMR1_OFFSET, out uint ccmrx);

                        // Set the Preload enable bit for channel1
                        ccmrx |= TIM_CCMR1_OC1PE;

                        // Configure the Output Fast mode
                        ccmrx &= ~TIM_CCMR1_OC1FE;
                        ccmrx |= fastMode;

                        Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCMR1_OFFSET, ccmrx);
                    }
                    break;

                case TIM_CHANNEL_2:
                    {
                        /* Configure the Channel 2 in PWM mode */
                        TIM_OC2_SetConfig(timerBaseAddress, isTIM1or8, ocMode, ocPolarity, pulse);

                        Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCMR1_OFFSET, out uint ccmrx);

                        /* Set the Preload enable bit for channel2 */
                        ccmrx |= TIM_CCMR1_OC2PE;

                        /* Configure the Output Fast mode */
                        ccmrx &= ~TIM_CCMR1_OC2FE;
                        ccmrx |= fastMode << 8;

                        Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCMR1_OFFSET, ccmrx);
                    }
                    break;

                case TIM_CHANNEL_3:
                    {
                        /* Configure the Channel 3 in PWM mode */
                        TIM_OC3_SetConfig(timerBaseAddress, isTIM1or8, ocMode, ocPolarity, pulse);

                        Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCMR2_OFFSET, out uint ccmrx);

                        /* Set the Preload enable bit for channel3 */
                        ccmrx |= TIM_CCMR2_OC3PE;

                        /* Configure the Output Fast mode */
                        ccmrx &= ~TIM_CCMR2_OC3FE;
                        ccmrx |= fastMode;

                        Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CCMR2_OFFSET, ccmrx);
                    }
                    break;

                default:
                    break;
            }
        }

        private void TIM_PWM_Start(uint timerBaseAddress, uint channel, bool enable)
        {
            uint tmp = TIM_CCER_CC1E << (int)channel;

            // Enable the Capture compare channel
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CCER_OFFSET, out uint ccer);
            // Reset the CCxE Bit
            ccer &= ~tmp;
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCER_OFFSET, ccer);

            // Set or reset the CCxE Bit
            if (enable)
            {
                ccer |= (TIM_CCx_ENABLE << (int)channel);
            }
            else
            {
                ccer |= (TIM_CCx_DISABLE << (int)channel);
            }
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCER_OFFSET, ccer);

            if (timerBaseAddress == TIM8_BASE_ADDRESS)
            {
                // Enable the main output
                Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_BDTR_OFFSET, out uint bdtr);
                bdtr |= TIM_BDTR_MOE;
                Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_BDTR_OFFSET, bdtr);
            }
            // Enable the Peripheral
            Nuttx.TryGetRegister(DriverHandle, timerBaseAddress + TIMx_CR1_OFFSET, out uint cr1);
            cr1 |= TIM_CR1_CEN;
            Nuttx.SetRegister(DriverHandle, timerBaseAddress + TIMx_CR1_OFFSET, cr1);
        }

        private const uint RCC_CFGR_OFFSET = 0x08;

        private const uint RCC_CFGR_PPRE1 = 0x07 << 10;
        private const uint RCC_CFGR_PPRE2 = 0x07 << 13;

        public void EnablePC6PWM2()
        {
            Console.WriteLine("+EnablePC6PWM");

            // MODIFY_REG(RCC->CFGR, RCC_CFGR_PPRE1, RCC_HCLK_DIV16);
            // MODIFY_REG(RCC->CFGR, RCC_CFGR_PPRE2, (RCC_HCLK_DIV16 << 3));
            // MODIFY_REG(RCC->CFGR, RCC_CFGR_HPRE, RCC_ClkInitStruct->AHBCLKDivider);
            // __HAL_RCC_SYSCLK_CONFIG(RCC_ClkInitStruct->SYSCLKSource);
            // MODIFY_REG(RCC->CFGR, RCC_CFGR_PPRE1, RCC_ClkInitStruct->APB1CLKDivider);
            // MODIFY_REG(RCC->CFGR, RCC_CFGR_PPRE2, ((RCC_ClkInitStruct->APB2CLKDivider) << 3));


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
            Nuttx.TryGetRegister(DriverHandle, RCC_BASE_ADDRESS + RCC_AHB1ENR_OFFSET, out uint ahb1);
            ahb1 |= RCC_AHB1ENR_GPIOCEN;
            Nuttx.SetRegister(DriverHandle, RCC_BASE_ADDRESS + RCC_AHB1ENR_OFFSET, ahb1);

            // do a reset on the GPIO
            Nuttx.SetRegister(DriverHandle, STM32.GPIOC_BASE + STM32.STM32_GPIO_BSRR_OFFSET, gpioPin << 16);

            // set the alternate functions
            ConfigureGpio(STM32.GpioPort.PortC, 2, STM32.GpioMode.Output, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_100MHz, STM32.OutputType.PushPull, false, InterruptMode.None);
            ConfigureGpio(STM32.GpioPort.PortC, 5, STM32.GpioMode.Output, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_100MHz, STM32.OutputType.PushPull, false, InterruptMode.None);
            ConfigureGpio(STM32.GpioPort.PortC, 6, STM32.GpioMode.Output, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_100MHz, STM32.OutputType.PushPull, false, InterruptMode.None);

            // enable peripheral clock
            Nuttx.TryGetRegister(DriverHandle, RCC_BASE_ADDRESS + RCC_APB1ENR_OFFSET, out uint apb1);
            apb1 |= RCC_APB1ENR_TIM3EN;
            Nuttx.SetRegister(DriverHandle, RCC_BASE_ADDRESS + RCC_APB1ENR_OFFSET, apb1);

            //// Base initialization
            TIMxInit(TIM3_BASE_ADDRESS, prescaler, period, counterMode, autoReloadPreload);

            //// Configure Clock Source
            TIMxConfigClockSource(TIM3_BASE_ADDRESS);

            //// MasterConfigSynchronization
            TIMxMasterConfigSynchronization(TIM3_BASE_ADDRESS, false);

            //// HAL_TIM_PWM_ConfigChannel
            // TODO: different configs per channel?
            TIMxPWMConfigChannel(TIM3_BASE_ADDRESS, TIM_CHANNEL_1);
            TIMxPWMConfigChannel(TIM3_BASE_ADDRESS, TIM_CHANNEL_2);
            TIMxPWMConfigChannel(TIM3_BASE_ADDRESS, TIM_CHANNEL_3);

            TIM_PWM_Start(TIM3_BASE_ADDRESS, TIM_CHANNEL_1, true);
            TIM_PWM_Start(TIM3_BASE_ADDRESS, TIM_CHANNEL_2, true);
            TIM_PWM_Start(TIM3_BASE_ADDRESS, TIM_CHANNEL_3, true);

            Console.WriteLine("-EnablePC6PWM");
        }

        private const uint ATIM_CCMR_CCS_CCOUT = 0x00;
        private const int ATIM_CCMR1_CC1S_SHIFT = 0;
        private const int ATIM_CCMR1_CC2S_SHIFT = 8;
        private const int ATIM_CCMR2_CC3S_SHIFT = 0;
        private const int ATIM_CCMR2_CC4S_SHIFT = 8;
        private const int ATIM_CCMR1_OC1M_SHIFT = 4;
        private const int ATIM_CCMR1_OC2M_SHIFT = 12;
        private const int ATIM_CCMR2_OC3M_SHIFT = 4;
        private const int ATIM_CCMR2_OC4M_SHIFT = 12;

        public void EnablePC6PWM()
        {
            Console.WriteLine("+EnablePC6PWM new");

            // enable GPIO port C clock
            Nuttx.TryGetRegister(DriverHandle, RCC_BASE_ADDRESS + RCC_AHB1ENR_OFFSET, out uint ahb1);
            ahb1 |= RCC_AHB1ENR_GPIOCEN;
            Nuttx.SetRegister(DriverHandle, RCC_BASE_ADDRESS + RCC_AHB1ENR_OFFSET, ahb1);

            // do a reset on the GPIO
            Nuttx.SetRegister(DriverHandle, STM32.GPIOC_BASE + STM32.STM32_GPIO_BSRR_OFFSET, 0xff << 16);

            // set the alternate functions
            ConfigureGpio(STM32.GpioPort.PortC, 2, STM32.GpioMode.Output, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_100MHz, STM32.OutputType.PushPull, false, InterruptMode.None);
            ConfigureGpio(STM32.GpioPort.PortC, 5, STM32.GpioMode.Output, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_100MHz, STM32.OutputType.PushPull, false, InterruptMode.None);
            ConfigureGpio(STM32.GpioPort.PortC, 6, STM32.GpioMode.Output, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_100MHz, STM32.OutputType.PushPull, false, InterruptMode.None);

            // enable peripheral clock
            Nuttx.TryGetRegister(DriverHandle, RCC_BASE_ADDRESS + RCC_APB1ENR_OFFSET, out uint apb1);
            apb1 |= RCC_APB1ENR_TIM3EN;
            Nuttx.SetRegister(DriverHandle, RCC_BASE_ADDRESS + RCC_APB1ENR_OFFSET, apb1);


            // DEV NOTE: Ported from nuttx STM32F7 PWM driver code
            uint prescaler = 7;
            uint timclk = 6000000;
            uint reload = 60000;

            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CR1_OFFSET, out uint cr1);
            // disable the timer
            cr1 &= ~TIM_CR1_CEN;

            cr1 &= ~(TIM_CR1_DIR | TIM_CR1_CMS);
            cr1 |= TIM_COUNTERMODE_UP;
            cr1 &= ~TIM_CR1_CKD;
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CR1_OFFSET, cr1);


            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_ARR_OFFSET, reload);
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_PSC_OFFSET, prescaler - 1);

            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_EGR_OFFSET, TIM_EGR_UG);

            var channelCount = 4;
            uint ccr;
            uint chanmode;
            uint ccenable = 0;
            uint ocmode1 = 0;
            uint ocmode2 = 0;

            for (var i = 0; i < channelCount; i++)
            {
                // duty cycle = ccr / reload (fractional value)
                ccr = 3000;  // b16toi(duty * reload + b16HALF);
                chanmode = PWM_MODE_1;

                var channel = i + 1; // channel is 1-based
                switch (channel)
                {
                    case 1:
                        // Select the CCER enable bit for this channel
                        ccenable |= TIM_CCER_CC1E;

                        // Set the CCMR1 mode values (leave CCMR2 zero)
                        ocmode1 |= (ATIM_CCMR_CCS_CCOUT << ATIM_CCMR1_CC1S_SHIFT) |
                                    (chanmode << ATIM_CCMR1_OC1M_SHIFT) |
                                    TIM_CCMR1_OC1PE;

                        // Set the duty cycle by writing to the CCR register for this channel
                        Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCR1_OFFSET, ccr);
                        break;                    
                    case 2:
                        // Select the CCER enable bit for this channel 
                        ccenable |= TIM_CCER_CC2E;

                        // Set the CCMR1 mode values (leave CCMR2 zero)
                        ocmode1 |= (ATIM_CCMR_CCS_CCOUT << ATIM_CCMR1_CC2S_SHIFT) |
                                    (chanmode << ATIM_CCMR1_OC2M_SHIFT) |
                                    TIM_CCMR1_OC2PE;

                        // Set the duty cycle by writing to the CCR register for this channel
                        Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCR2_OFFSET, ccr);
                        break;
                    case 3:
                        // Select the CCER enable bit for this channel
                        ccenable |= TIM_CCER_CC3E;

                        // Set the CCMR2 mode values (leave CCMR1 zero)
                        ocmode2 |= (ATIM_CCMR_CCS_CCOUT << ATIM_CCMR2_CC3S_SHIFT) |
                                    (chanmode << ATIM_CCMR2_OC3M_SHIFT) |
                                    TIM_CCMR2_OC3PE;

                        // Set the duty cycle by writing to the CCR register for this channel
                        Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCR3_OFFSET, ccr);
                        break;
                    case 4:
                        // Select the CCER enable bit for this channel
                        ccenable |= TIM_CCER_CC4E;

                        // Set the CCMR2 mode values (leave CCMR1 zero)
                        ocmode2 |= (ATIM_CCMR_CCS_CCOUT << ATIM_CCMR2_CC4S_SHIFT) |
                                    (chanmode << ATIM_CCMR2_OC4M_SHIFT) |
                                    TIM_CCMR2_OC4PE;

                        // Set the duty cycle by writing to the CCR register for this channel
                        Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCR4_OFFSET, ccr);
                        break;
                    default:
                        continue;
                }
            }

            // Disable the Channel by resetting the CCxE Bit in the CCER register
            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCER_OFFSET, out uint ccer);
            ccer &= ~ccenable;
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCER_OFFSET, ccer);

            // Fetch the CR2, CCMR1, and CCMR2 register (already have cr1 and ccer)
            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CR2_OFFSET, out uint cr2);
            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR1_OFFSET, out uint ccmr1);
            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR2_OFFSET, out uint ccmr2);

            // Reset the Output Compare Mode Bits and set the select output compare mode
            ccmr1 &= ~(TIM_CCMR1_CC1S | TIM_CCMR1_OC1M | TIM_CCMR1_OC1PE |
                       TIM_CCMR2_CC3S | TIM_CCMR1_OC2M | TIM_CCMR1_OC2PE);
            ccmr2 &= ~(TIM_CCMR2_CC3S | TIM_CCMR2_OC3M | TIM_CCMR2_OC3PE |
                       TIM_CCMR2_CC4S | TIM_CCMR2_OC4M | TIM_CCMR2_OC4PE);
            ccmr1 |= ocmode1;
            ccmr2 |= ocmode2;

            // Reset the output polarity level of all channels (selects high polarity)
            ccer &= ~(TIM_CCER_CC1P | TIM_CCER_CC2P | TIM_CCER_CC3P | TIM_CCER_CC4P);

            // Enable the output state of the selected channels
            ccer &= ~(TIM_CCER_CC1E | TIM_CCER_CC2E | TIM_CCER_CC3E | TIM_CCER_CC4E);
            ccer |= ccenable;

            // CCxNP must be cleared in any case
            ccer &= ~(TIM_CCER_CC1NP | TIM_CCER_CC2NP | TIM_CCER_CC3NP | TIM_CCER_CC4NP);

            // Save the modified register values
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CR2_OFFSET, cr2);
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR1_OFFSET, ccmr1);
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCMR2_OFFSET, ccmr2);
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CCER_OFFSET, ccer);

            // Set the ARR Preload Bit
            Nuttx.TryGetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CR1_OFFSET, out cr1);
            cr1 |= TIM_CR1_ARPE;
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CR1_OFFSET, cr1);

            // Just enable the timer, leaving all interrupts disabled
            cr1 |= TIM_CR1_CEN;
            Nuttx.SetRegister(DriverHandle, TIM3_BASE_ADDRESS + TIMx_CR1_OFFSET, cr1);

            Console.WriteLine("-EnablePC6PWM");
        }
    }
}