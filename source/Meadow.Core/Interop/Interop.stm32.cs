using System;
using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class STM32
        {
            public const uint BOARD_XTAL = 25000000u;
            public const uint HSE_FREQUENCY = BOARD_XTAL;
            public const uint VCO_FREQUENCY = ((HSE_FREQUENCY / 25) * 432);
            public const uint SYSCLK_FREQUENCY = (VCO_FREQUENCY / 2);
            public const uint HCLK_FREQUENCY = SYSCLK_FREQUENCY;
            public const uint PCLK1_FREQUENCY = (HCLK_FREQUENCY / 4);

            public const int GPIOA_BASE = 0x40020000;
            public const int GPIOB_BASE = 0x40020400;
            public const int GPIOC_BASE = 0x40020800;
            public const int GPIOD_BASE = 0x40020c00;
            public const int GPIOE_BASE = 0x40021000;
            public const int GPIOF_BASE = 0x40021400;
            public const int GPIOG_BASE = 0x40021800;
            public const int GPIOH_BASE = 0x40021c00;
            public const int GPIOI_BASE = 0x40022000;
            public const int GPIOJ_BASE = 0x40022400;
            public const int GPIOK_BASE = 0x40022800;

            public const int GPIO_MODER_OFFSET = 0x00;
            public const int GPIO_OTYPER_OFFSET = 0x04;
            public const int GPIO_OSPEED_OFFSET = 0x08;
            public const int GPIO_PUPDR_OFFSET = 0x0c;
            public const int GPIO_IDR_OFFSET = 0x10;
            public const int GPIO_BSRR_OFFSET = 0x18;
            public const int GPIO_AFRL_OFFSET = 0x20;
            public const int GPIO_AFRH_OFFSET = 0x24;

            public const int MEADOW_ADC1_BASE = 0x40012000;
            public const int ADC_SR_OFFSET = 0x00;
            public const int ADC_CR1_OFFSET = 0x04;
            public const int ADC_CR2_OFFSET = 0x08;
            public const int ADC_SMPR1_OFFSET = 0x0c;
            public const int ADC_SMPR2_OFFSET = 0x10;
            public const int ADC_SQR1_OFFSET = 0x2c;
            public const int ADC_SQR2_OFFSET = 0x30;
            public const int ADC_SQR3_OFFSET = 0x34;
            public const int ADC_DR_OFFSET = 0x4c;
            public const int ADC_CCR_OFFSET = 0x0304;

            public const int ADC_CR1_NON_RESERVED_MASK = 0x07c0ffff;
            public const int ADC_CR1_OVRIE = (1 << 26);     // overrun interrupt enable
            public const int ADC_CR1_RES_SHIFT = 24;        // resolution
            public const int ADC_CR1_AWDEN = (1 << 23);     // watchdog enable
            public const int ADC_CR1_JAWDEN = (1 << 22);    // injected watchdog enable
            public const int ADC_CR1_DISCNUM_SHIFT = 13;    // discontinuous channel count
            public const int ADC_CR1_JDISCEN = (1 << 12);   // injected discontinuous mode enable
            public const int ADC_CR1_DISCEN = (1 << 11);    // discontinuous mode enable
            public const int ADC_CR1_JAUTO = (1 << 10);     // automatic injected conversion
            public const int ADC_CR1_AWDSGL = (1 << 9);     // single channel, scan mode watchdog
            public const int ADC_CR1_SCAN = (1 << 8);       // scan mode
            public const int ADC_CR1_JEOCIE = (1 << 7);
            public const int ADC_CR1_AWDIE = (1 << 6);
            public const int ADC_CR1_EOCIE = (1 << 5);
            public const int ADC_CR1_AWDCH_SHIFT = (1 << 0);

            public const int ADC_CR2_NON_RESERVED_MASK = 0x3f7f0f03;
            public const int ADC_CR2_SWSTART = (1 << 30); // starts regulare conversion.  ADON must be 1 otherwise it is ignored
            public const int ADC_CR2_EOCS = (1 << 10); // set EOC (end of conversion) bit
            public const int ADC_CR2_CONT = (1 << 1);
            public const int ADC_CR2_ADON = (1 << 0);

            public const int ADC_SMPR1_NON_RESERVED_MASK = 0x07FFFFFF;
            public const int ADC_SMPR1_CH10_SHIFT = 0;
            public const int ADC_SMPR1_CH11_SHIFT = 3;
            public const int ADC_SMPR2_CH3_SHIFT = 9;
            public const int ADC_SMPR2_CH4_SHIFT = 12;
            public const int ADC_SMPR2_CH5_SHIFT = 15;
            public const int ADC_SMPR2_CH7_SHIFT = 21;

            public const int ADC_SMPx_SAMPLING_112_CYCLES = 5; // (101 binary)

            public const int ADC_SMPR2_NON_RESERVED_MASK = 0x3fffffff;
            public const int ADC_SQR1_NON_RESERVED_MASK = 0x00FFFFFF;
            public const int ADC_SQR2_NON_RESERVED_MASK = 0x3FFFFFFF;

            public const int ADC_SQR3_NON_RESERVED_MASK = 0x3FFFFFFF;
            public const int ADC_SQR3_SQ1_SHIFT = 0;

            public const int ADC_SR_OVR = (1 << 5); // overrun
            public const int ADC_SR_STRT = (1 << 4); // regular channel start flag
            public const int ADC_SR_EOC = (1 << 1); // end of regular conversion
            public const int ADC_SR_AWD = (1 << 0); // analog watchdog

            public const int ADC_CCR_NON_RESERVED_MASK = 0x00c3ef1f;
            public const int ADC_CCR_ADCPRE_SHIFT = 16;
            public const int ADC_CCR_PRESCALER_DIV2 = 0;
            public const int ADC_CCR_PRESCALER_DIV4 = 1;

            public const int MEADOW_I2C1_BASE = 0x40005400;
            public const int MEADOW_I2C2_BASE = 0x40005800;
            public const int MEADOW_I2C3_BASE = 0x40005C00;
            public const int MEADOW_I2C4_BASE = 0x40006000;

            public const int I2C_CR1_OFFSET = 0x00;
            public const int I2C_CR2_OFFSET = 0x04;
            public const int I2C_OAR1_OFFSET = 0x08;
            public const int I2C_OAR2_OFFSET = 0x0C;
            public const int I2C_TIMINGR_OFFSET = 0x10;
            public const int I2C_TIMEOUTR_OFFSET = 0x14;
            public const int I2C_ISR_OFFSET = 0x18;
            public const int I2C_ICR_OFFSET = 0x1c;
            public const int I2C_PECR_OFFSET = 0x20;
            public const int I2C_RXDR_OFFSET = 0x28;
            public const int I2C_TXDR_OFFSET = 0x28;

            public const int I2C_TIMINGR_PRESC_SHIFT = 31;
            public const int I2C_TIMINGR_SCLDEL_SHIFT = 20;
            public const int I2C_TIMINGR_SDADEL_SHIFT = 16;
            public const int I2C_TIMINGR_SCLH_SHIFT = 8;
            public const int I2C_TIMINGR_SCLL_SHIFT = 0;

            public const uint I2C_CR1_PE = 1 << 0;

            public const int I2C_CR2_SADD10_SHIFT = (0);       /* Bits 0-9: Slave 10-bit address (master) */
            public const uint I2C_CR2_SADD10_MASK = (0x3ff << I2C_CR2_SADD10_SHIFT);
            public const int I2C_CR2_SADD7_SHIFT = (1);       /* Bits 1-7: Slave 7-bit address (master) */
            public const uint I2C_CR2_SADD7_MASK = (0x7f << I2C_CR2_SADD7_SHIFT);
            public const uint I2C_CR2_RD_WRN = (1 << 10); /* Bit 10: Transfer direction (master) */
            public const uint I2C_CR2_ADD10 = (1 << 11); /* Bit 11: 10-bit addressing mode (master) */
            public const uint I2C_CR2_HEAD10R = (1 << 12); /* Bit 12: 10-bit address header only read direction (master) */
            public const uint I2C_CR2_START = (1 << 13); /* Bit 13: Start generation */
            public const uint I2C_CR2_STOP = (1 << 14); /* Bit 14: Stop generation (master) */
            public const uint I2C_CR2_NACK = (1 << 15); /* Bit 15: NACK generation (slave) */
            public const int I2C_CR2_NBYTES_SHIFT = (16);      /* Bits 16-23: Number of bytes */
            public const uint I2C_CR2_NBYTES_MASK = (0xff << I2C_CR2_NBYTES_SHIFT);
            public const uint I2C_CR2_RELOAD = (1 << 24); /* Bit 24: NBYTES reload mode */
            public const uint I2C_CR2_AUTOEND = (1 << 25); /* Bit 25: Automatic end mode (master) */
            public const uint I2C_CR2_PECBYTE = (1 << 26); /* Bit 26: Packet error checking byte */

            public const uint I2C_ISR_TXE = 1 << 0;
            public const uint I2C_ISR_TXIS = 1 << 1;
            public const uint I2C_ISR_RXNE = 1 << 2;
            public const uint I2C_ISR_ADDR = 1 << 3;
            public const uint I2C_ISR_NACKF = 1 << 4;
            public const uint I2C_ISR_STOPF = 1 << 5;
            public const uint I2C_ISR_TC = 1 << 6;
            public const uint I2C_ISR_TCR = 1 << 7;
            public const uint I2C_ISR_BERR = 1 << 8;
            public const uint I2C_ISR_ARLO = 1 << 9;
            public const uint I2C_ISR_TIMEOUT = 1 << 12;
            public const uint I2C_ISR_BUSY = 1 << 15;

            public const int MEADOW_SPI1_BASE = 0x40013000;
            public const int MEADOW_SPI2_BASE = 0x40003800;
            public const int MEADOW_SPI3_BASE = 0x40003C00;
            public const int SPI_CR1_OFFSET = 0x00;
            public const int SPI_CR1_SPE = (1 << 6);
            public const int SPI_BR_MASK = 0x07;
            public const int SPI_BR_SHIFT = 0x03;
            public const int SPI_SR_OFFSET = 0x08;
            public const int SPI_DR_OFFSET = 0x0c;

            public const uint RCC_CFGR_PPRE1 = 0x07 << 10;
            public const uint RCC_CFGR_PPRE2 = 0x07 << 13;
            public const uint RCC_CFGR_HPRE = 0x0F << 4;
            public const uint RCC_CFGR_SW = 0x03 << 0;

            public const int RCC_HCLK_DIV16 = 0x00001C00;
            public const int RCC_SYSCLK_DIV1 = 0x00;
            public const int RCC_SYSCLKSOURCE_HSI = 0x00;
            public const int RCC_HCLK_DIV1 = 0x00000000;

            public const int RCC_APB1RSTR_TIM2RST = (1 << 0);/* Bit 0:  TIM2 reset */
            public const int RCC_APB1RSTR_TIM3RST = (1 << 1);/* Bit 1:  TIM3 reset */
            public const int RCC_APB1RSTR_TIM4RST = (1 << 2);/* Bit 2:  TIM4 reset */
            public const int RCC_APB1RSTR_TIM5RST = (1 << 3);/* Bit 3:  TIM5 reset */
            public const int RCC_APB1RSTR_TIM6RST = (1 << 4);/* Bit 4:  TIM6 reset */
            public const int RCC_APB1RSTR_TIM7RST = (1 << 5);/* Bit 5:  TIM7 reset */
            public const int RCC_APB1RSTR_TIM12RST = (1 << 6);/* Bit 6:  TIM12 reset */
            public const int RCC_APB1RSTR_TIM13RST = (1 << 7);/* Bit 7:  TIM13 reset */
            public const int RCC_APB1RSTR_TIM14RST = (1 << 8);/* Bit 8:  TIM14 reset */
            public const int RCC_APB1RSTR_I2C1RST = (1 << 21); /* Bit 21: I2C 1 reset */
            public const int RCC_APB1RSTR_I2C2RST = (1 << 22);/* Bit 22: I2C 2 reset */
            public const int RCC_APB1RSTR_I2C3RST = (1 << 23);/* Bit 23: I2C 3 reset */
            public const int RCC_APB1RSTR_I2C4RST = (1 << 24);/* Bit 24: I2C 4 reset */

            public const int RCC_APB1ENR_TIM2EN = (1 << 0);/* Bit 0:  TIM 2 clock enable */
            public const int RCC_APB1ENR_TIM3EN = (1 << 1);/* Bit 1:  TIM 3 clock enable */
            public const int RCC_APB1ENR_TIM4EN = (1 << 2);/* Bit 2:  TIM 4 clock enable */
            public const int RCC_APB1ENR_TIM5EN = (1 << 3);/* Bit 3:  TIM 5 clock enable */
            public const int RCC_APB1ENR_TIM6EN = (1 << 4);/* Bit 4:  TIM 6 clock enable */
            public const int RCC_APB1ENR_TIM7EN = (1 << 5);/* Bit 5:  TIM 7 clock enable */
            public const int RCC_APB1ENR_TIM12EN = (1 << 6);/* Bit 6:  TIM 12 clock enable */
            public const int RCC_APB1ENR_TIM13EN = (1 << 7);/* Bit 7:  TIM 13 clock enable */
            public const int RCC_APB1ENR_TIM14EN = (1 << 8);/* Bit 8:  TIM 14 clock enable */
            public const int RCC_APB1ENR_LPTIM1EN = (1 << 9);/* Bit 9:  LPTIM 1 clock enable */
            public const int RCC_APB1ENR_WWDGEN = (1 << 11);/* Bit 11: Window watchdog clock enable */
            public const int RCC_APB1ENR_CAN3EN = (1 << 13);/* Bit 13: CAN 3 clock enable */
            public const int RCC_APB1ENR_SPI2EN = (1 << 14);/* Bit 14: SPI 2 clock enable */
            public const int RCC_APB1ENR_SPI3EN = (1 << 15);/* Bit 15: SPI 3 clock enable */
            public const int RCC_APB1ENR_SPDIFRXEN = (1 << 16);/* Bit 16: SPDIFRX clock enable */
            public const int RCC_APB1ENR_USART2EN = (1 << 17);/* Bit 17: USART 2 clock enable */
            public const int RCC_APB1ENR_USART3EN = (1 << 18);/* Bit 18: USART 3 clock enable */
            public const int RCC_APB1ENR_UART4EN = (1 << 19);/* Bit 19: UART 4 clock enable */
            public const int RCC_APB1ENR_UART5EN = (1 << 20);/* Bit 20: UART 5 clock enable */
            public const int RCC_APB1ENR_I2C1EN = (1 << 21);/* Bit 21: I2C 1 clock enable */
            public const int RCC_APB1ENR_I2C2EN = (1 << 22);/* Bit 22: I2C 2 clock enable */
            public const int RCC_APB1ENR_I2C3EN = (1 << 23);/* Bit 23: I2C 3 clock enable */
            public const int RCC_APB1ENR_I2C4EN = (1 << 24);/* Bit 24: I2C 4 clock enable */
            public const int RCC_APB1ENR_CAN1EN = (1 << 25);/* Bit 25: CAN 1 clock enable */
            public const int RCC_APB1ENR_CAN2EN = (1 << 26);/* Bit 26: CAN 2 clock enable */
            public const int RCC_APB1ENR_CECEN = (1 << 27);/* Bit 27: CEC clock enable */
            public const int RCC_APB1ENR_PWREN = (1 << 28);/* Bit 28: Power interface clock enable */
            public const int RCC_APB1ENR_DACEN = (1 << 29);/* Bit 29: DAC interface clock enable */
            public const int RCC_APB1ENR_UART7EN = (1 << 30);/* Bit 30: UART7 clock enable */
            public const int RCC_APB1ENR_UART8EN = (1 << 31);/* Bit 31: UART8 clock enable */


            public const int RCC_BASE = 0x40023800;
            public const int RCC_CR_OFFSET = 0x00;
            public const int RCC_PLLCFGR_OFFSET = 0x04;
            public const int RCC_CFGR_OFFSET = 0x08;
            public const int RCC_CIR_OFFSET = 0x0C;
            public const int RCC_AHB1RSTR_OFFSET = 0x10;  /* AHB1 peripheral reset register */
            public const int RCC_AHB2RSTR_OFFSET = 0x14;  /* AHB2 peripheral reset register */
            public const int RCC_AHB3RSTR_OFFSET = 0x18;  /* AHB3 peripheral reset register */
            public const int RCC_APB1RSTR_OFFSET = 0x20;  /* APB1 Peripheral reset register */
            public const int RCC_APB2RSTR_OFFSET = 0x24;  /* APB2 Peripheral reset register */
            public const int RCC_AHB1ENR_OFFSET = 0x30;  /* AHB1 Peripheral Clock enable register */
            public const int RCC_AHB2ENR_OFFSET = 0x34;  /* AHB2 Peripheral Clock enable register */
            public const int RCC_AHB3ENR_OFFSET = 0x38;  /* AHB3 Peripheral Clock enable register */
            public const int RCC_APB1ENR_OFFSET = 0x40;  /* APB1 Peripheral Clock enable register */
            public const int RCC_APB2ENR_OFFSET = 0x44;  /* APB2 Peripheral Clock enable register */
            public const int RCC_AHB1LPENR_OFFSET = 0x50;
            public const int RCC_AHB2LPENR_OFFSET = 0x54;
            public const int RCC_AHB3LPENR_OFFSET = 0x58;
            public const int RCC_APB1LPENR_OFFSET = 0x60;
            public const int RCC_APB2LPENR_OFFSET = 0x64;
            public const int RCC_BDCR_OFFSET = 0x70;
            public const int RCC_CSR_OFFSET = 0x74;
            public const int RCC_SSCGR_OFFSET = 0x80;
            public const int RCC_PLLI2SCFG_OFFSET = 0x84;
            public const int RCC_PLLSAICFGR_OFFSET = 0x88;
            public const int RCC_DCKCFGR1_OFFSET = 0x8C;
            public const int RCC_DCKCFGR2_OFFSET = 0x90;

            public const int RCC_APB2ENR_ADC1EN = (1 << 8); // adc1 enable
            public const int RCC_APB2RSTR_ADCRST = (1 << 8); // adc reset

            public const uint I2C1_SEL_PCLK1_CLK = 0 << 16;
            public const uint I2C1_SEL_SYS_CLK = 1 << 16;
            public const uint I2C1_SEL_PLL_CLK = 2 << 16;

            public const int IRQ_FIRST = 16;
            public const int IRQ_WWDG = (IRQ_FIRST + 0);/* 0:  Window Watchdog interrupt */
            public const int IRQ_PVD = (IRQ_FIRST + 1);/* 1:  PVD through EXTI Line detection interrupt */
            public const int IRQ_TAMPER = (IRQ_FIRST + 2);/* 2:  Tamper and time stamp interrupts */
            public const int IRQ_TIMESTAMP = (IRQ_FIRST + 2);/* 2:  Tamper and time stamp interrupts */
            public const int IRQ_RTC_WKUP = (IRQ_FIRST + 3);/* 3:  RTC global interrupt */
            public const int IRQ_FLASH = (IRQ_FIRST + 4);/* 4:  Flash global interrupt */
            public const int IRQ_RCC = (IRQ_FIRST + 5);/* 5:  RCC global interrupt */
            public const int IRQ_EXTI0 = (IRQ_FIRST + 6);/* 6:  EXTI Line 0 interrupt */
            public const int IRQ_EXTI1 = (IRQ_FIRST + 7);/* 7:  EXTI Line 1 interrupt */
            public const int IRQ_EXTI2 = (IRQ_FIRST + 8);/* 8:  EXTI Line 2 interrupt */
            public const int IRQ_EXTI3 = (IRQ_FIRST + 9);/* 9:  EXTI Line 3 interrupt */
            public const int IRQ_EXTI4 = (IRQ_FIRST + 10);/* 10: EXTI Line 4 interrupt */
            public const int IRQ_DMA1S0 = (IRQ_FIRST + 11);/* 11: DMA1 Stream 0 global interrupt */
            public const int IRQ_DMA1S1 = (IRQ_FIRST + 12);/* 12: DMA1 Stream 1 global interrupt */
            public const int IRQ_DMA1S2 = (IRQ_FIRST + 13);/* 13: DMA1 Stream 2 global interrupt */
            public const int IRQ_DMA1S3 = (IRQ_FIRST + 14);/* 14: DMA1 Stream 3 global interrupt */
            public const int IRQ_DMA1S4 = (IRQ_FIRST + 15);/* 15: DMA1 Stream 4 global interrupt */
            public const int IRQ_DMA1S5 = (IRQ_FIRST + 16);/* 16: DMA1 Stream 5 global interrupt */
            public const int IRQ_DMA1S6 = (IRQ_FIRST + 17);/* 17: DMA1 Stream 6 global interrupt */
            public const int IRQ_ADC = (IRQ_FIRST + 18);/* 18: ADC1, ADC2, and ADC3 global interrupt */
            public const int IRQ_CAN1TX = (IRQ_FIRST + 19);/* 19: CAN1 TX interrupts */
            public const int IRQ_CAN1RX0 = (IRQ_FIRST + 20);/* 20: CAN1 RX0 interrupts */
            public const int IRQ_CAN1RX1 = (IRQ_FIRST + 21);/* 21: CAN1 RX1 interrupt */
            public const int IRQ_CAN1SCE = (IRQ_FIRST + 22);/* 22: CAN1 SCE interrupt */
            public const int IRQ_EXTI95 = (IRQ_FIRST + 23);/* 23: EXTI Line[9:5] interrupts */
            public const int IRQ_TIM1BRK = (IRQ_FIRST + 24);/* 24: TIM1 Break interrupt */
            public const int IRQ_TIM9 = (IRQ_FIRST + 24);/* 24: TIM9 global interrupt */
            public const int IRQ_TIM1UP = (IRQ_FIRST + 25);/* 25: TIM1 Update interrupt */
            public const int IRQ_TIM10 = (IRQ_FIRST + 25);/* 25: TIM10 global interrupt */
            public const int IRQ_TIM1TRGCOM = (IRQ_FIRST + 26);/* 26: TIM1 Trigger and Commutation interrupts */
            public const int IRQ_TIM11 = (IRQ_FIRST + 26);/* 26: TIM11 global interrupt */
            public const int IRQ_TIM1CC = (IRQ_FIRST + 27);/* 27: TIM1 Capture Compare interrupt */
            public const int IRQ_TIM2 = (IRQ_FIRST + 28);/* 28: TIM2 global interrupt */
            public const int IRQ_TIM3 = (IRQ_FIRST + 29);/* 29: TIM3 global interrupt */
            public const int IRQ_TIM4 = (IRQ_FIRST + 30);/* 30: TIM4 global interrupt */
            public const int IRQ_I2C1EV = (IRQ_FIRST + 31);/* 31: I2C1 event interrupt */
            public const int IRQ_I2C1ER = (IRQ_FIRST + 32);/* 32: I2C1 error interrupt */
            public const int IRQ_I2C2EV = (IRQ_FIRST + 33);/* 33: I2C2 event interrupt */
            public const int IRQ_I2C2ER = (IRQ_FIRST + 34);/* 34: I2C2 error interrupt */
            public const int IRQ_SPI1 = (IRQ_FIRST + 35);/* 35: SPI1 global interrupt */
            public const int IRQ_SPI2 = (IRQ_FIRST + 36);/* 36: SPI2 global interrupt */
            public const int IRQ_USART1 = (IRQ_FIRST + 37);/* 37: USART1 global interrupt */
            public const int IRQ_USART2 = (IRQ_FIRST + 38);/* 38: USART2 global interrupt */
            public const int IRQ_USART3 = (IRQ_FIRST + 39);/* 39: USART3 global interrupt */
            public const int IRQ_EXTI1510 = (IRQ_FIRST + 40);/* 40: EXTI Line[15:10] interrupts */
            public const int IRQ_RTCALRM = (IRQ_FIRST + 41);/* 41: RTC alarm through EXTI line interrupt */
            public const int IRQ_OTGFSWKUP = (IRQ_FIRST + 42);/* 42: USB On-The-Go FS Wakeup through EXTI line interrupt */
            public const int IRQ_TIM8BRK = (IRQ_FIRST + 43);/* 43: TIM8 Break interrupt */
            public const int IRQ_TIM12 = (IRQ_FIRST + 43);/* 43: TIM12 global interrupt */
            public const int IRQ_TIM8UP = (IRQ_FIRST + 44);/* 44: TIM8 Update interrupt */
            public const int IRQ_TIM13 = (IRQ_FIRST + 44);/* 44: TIM13 global interrupt */
            public const int IRQ_TIM8TRGCOM = (IRQ_FIRST + 45);/* 45: TIM8 Trigger and Commutation interrupts */
            public const int IRQ_TIM14 = (IRQ_FIRST + 45);/* 45: TIM14 global interrupt */
            public const int IRQ_TIM8CC = (IRQ_FIRST + 46);/* 46: TIM8 Capture Compare interrupt */
            public const int IRQ_DMA1S7 = (IRQ_FIRST + 47);/* 47: DMA1 Stream 7 global interrupt */
            public const int IRQ_FMC = (IRQ_FIRST + 48);/* 48: FMC global interrupt */
            public const int IRQ_SDMMC1 = (IRQ_FIRST + 49);/* 49: SDMMC1 global interrupt */
            public const int IRQ_TIM5 = (IRQ_FIRST + 50);/* 50: TIM5 global interrupt */
            public const int IRQ_SPI3 = (IRQ_FIRST + 51);/* 51: SPI3 global interrupt */
            public const int IRQ_UART4 = (IRQ_FIRST + 52);/* 52: UART4 global interrupt */
            public const int IRQ_UART5 = (IRQ_FIRST + 53);/* 53: UART5 global interrupt */
            public const int IRQ_TIM6 = (IRQ_FIRST + 54);/* 54: TIM6 global interrupt */
            public const int IRQ_DAC = (IRQ_FIRST + 54);/* 54: DAC1 and DAC2 underrun error interrupts */
            public const int IRQ_TIM7 = (IRQ_FIRST + 55);/* 55: TIM7 global interrupt */
            public const int IRQ_DMA2S0 = (IRQ_FIRST + 56);/* 56: DMA2 Stream 0 global interrupt */
            public const int IRQ_DMA2S1 = (IRQ_FIRST + 57);/* 57: DMA2 Stream 1 global interrupt */
            public const int IRQ_DMA2S2 = (IRQ_FIRST + 58);/* 58: DMA2 Stream 2 global interrupt */
            public const int IRQ_DMA2S3 = (IRQ_FIRST + 59);/* 59: DMA2 Stream 3 global interrupt */
            public const int IRQ_DMA2S4 = (IRQ_FIRST + 60);/* 60: DMA2 Stream 4 global interrupt */
            public const int IRQ_ETH = (IRQ_FIRST + 61);/* 61: Ethernet global interrupt */
            public const int IRQ_ETHWKUP = (IRQ_FIRST + 62);/* 62: Ethernet Wakeup through EXTI line interrupt */
            public const int IRQ_CAN2TX = (IRQ_FIRST + 63);/* 63: CAN2 TX interrupts */
            public const int IRQ_CAN2RX0 = (IRQ_FIRST + 64);/* 64: CAN2 RX0 interrupts */
            public const int IRQ_CAN2RX1 = (IRQ_FIRST + 65);/* 65: CAN2 RX1 interrupt */
            public const int IRQ_CAN2SCE = (IRQ_FIRST + 66);/* 66: CAN2 SCE interrupt */
            public const int IRQ_OTGFS = (IRQ_FIRST + 67);/* 67: USB On The Go FS global interrupt */
            public const int IRQ_DMA2S5 = (IRQ_FIRST + 68);/* 68: DMA2 Stream 5 global interrupt */
            public const int IRQ_DMA2S6 = (IRQ_FIRST + 69);/* 69: DMA2 Stream 6 global interrupt */
            public const int IRQ_DMA2S7 = (IRQ_FIRST + 70);/* 70: DMA2 Stream 7 global interrupt */
            public const int IRQ_USART6 = (IRQ_FIRST + 71);/* 71: USART6 global interrupt */
            public const int IRQ_I2C3EV = (IRQ_FIRST + 72);/* 72: I2C3 event interrupt */
            public const int IRQ_I2C3ER = (IRQ_FIRST + 73);/* 73: I2C3 error interrupt */
            public const int IRQ_OTGHSEP1OUT = (IRQ_FIRST + 74);/* 74: USB On The Go HS End Point 1 Out global interrupt */
            public const int IRQ_OTGHSEP1IN = (IRQ_FIRST + 75);/* 75: USB On The Go HS End Point 1 In global interrupt */
            public const int IRQ_OTGHSWKUP = (IRQ_FIRST + 76);/* 76: USB On The Go HS Wakeup through EXTI interrupt */
            public const int IRQ_OTGHS = (IRQ_FIRST + 77);/* 77: USB On The Go HS global interrupt */
            public const int IRQ_DCMI = (IRQ_FIRST + 78);/* 78: DCMI global interrupt */
            public const int IRQ_CRYP = (IRQ_FIRST + 79);/* 79: CRYP crypto global interrupt */
            public const int IRQ_HASH = (IRQ_FIRST + 80);/* 80: Hash and Rng global interrupt */
            public const int IRQ_RNG = (IRQ_FIRST + 80);/* 80: Hash and Rng global interrupt */
            public const int IRQ_FPU = (IRQ_FIRST + 81);/* 81: FPU global interrupt */
            public const int IRQ_UART7 = (IRQ_FIRST + 82);/* 82: UART7 global interrupt */
            public const int IRQ_UART8 = (IRQ_FIRST + 83);/* 83: UART8 global interrupt */
            public const int IRQ_SPI4 = (IRQ_FIRST + 84);/* 84: SPI4 global interrupt */
            public const int IRQ_SPI5 = (IRQ_FIRST + 85);/* 85: SPI5 global interrupt */
            public const int IRQ_SPI6 = (IRQ_FIRST + 86);/* 86: SPI6 global interrupt */
            public const int IRQ_SAI1 = (IRQ_FIRST + 87);/* 87: SAI1 global interrupt */
            public const int IRQ_LTDCINT = (IRQ_FIRST + 88);/* 88: LCD-TFT global interrupt */
            public const int IRQ_LTDCERRINT = (IRQ_FIRST + 89);/* 89: LCD-TFT global Error interrupt */
            public const int IRQ_DMA2D = (IRQ_FIRST + 90);/* 90: DMA2D global interrupt */
            public const int IRQ_SAI2 = (IRQ_FIRST + 91);/* 91: SAI2 global interrupt */
            public const int IRQ_QUADSPI = (IRQ_FIRST + 92);/* 92: QuadSPI global interrupt */
            public const int IRQ_LPTIMER1 = (IRQ_FIRST + 93);/* 93: LP Timer1 global interrupt */
            public const int IRQ_HDMICEC = (IRQ_FIRST + 94);/* 94: HDMI-CEC global interrupt */
            public const int IRQ_I2C4EV = (IRQ_FIRST + 95);/* 95: I2C4 event interrupt */
            public const int IRQ_I2C4ER = (IRQ_FIRST + 96);/* 96: I2C4 Error interrupt */
            public const int IRQ_SPDIFRX = (IRQ_FIRST + 97);/* 97: SPDIFRX global interrupt */
            public const int IRQ_DSIHOST = (IRQ_FIRST + 98);/* 98: DSI host global interrupt */
            public const int IRQ_DFSDM1FLT0 = (IRQ_FIRST + 99);/* 99: DFSDM1 Filter 0 global interrupt */
            public const int IRQ_DFSDM1FLT1 = (IRQ_FIRST + 100); /* 100: DFSDM1 Filter 1 global interrupt */
            public const int IRQ_DFSDM1FLT2 = (IRQ_FIRST + 101); /* 101: DFSDM1 Filter 2 global interrupt */
            public const int IRQ_DFSDM1FLT3 = (IRQ_FIRST + 102); /* 102: DFSDM1 Filter 3 global interrupt */
            public const int IRQ_SDMMC2 = (IRQ_FIRST + 103); /* 103: SDMMC2 global interrupt */
            public const int IRQ_CAN3TX = (IRQ_FIRST + 104); /* 104: CAN3 TX interrupt */
            public const int IRQ_CAN3RX0 = (IRQ_FIRST + 105); /* 105: CAN3 RX0 interrupt */
            public const int IRQ_CAN3RX1 = (IRQ_FIRST + 106); /* 106: CAN3 RX1 interrupt */
            public const int IRQ_CAN3SCE = (IRQ_FIRST + 107); /* 107: CAN3 SCE interrupt */
            public const int IRQ_JPEG = (IRQ_FIRST + 108); /* 108: JPEG global interrupt */
            public const int IRQ_MDIOS = (IRQ_FIRST + 109); /* 109: MDIO slave global interrupt */



            public enum GpioPort
            {
                PortA,
                PortB,
                PortC,
                PortD,
                PortE,
                PortF,
                PortG,
                PortH,
                PortI,
                PortJ,
                PortK,
            }

            public enum ResistorMode
            {
                Float = 0,
                PullUp = 1,
                PullDown = 2
            }

            public enum GPIOSpeed
            {
                Speed_2MHz = 0,
                Speed_25MHz = 1,
                Speed_50MHz = 2,
                Speed_100MHz = 3
            }

            public enum GpioMode
            {
                Input = 0,
                Output = 1,
                AlternateFunction = 2,
                Analog = 3
            }

            public enum OutputType
            {
                PushPull = 0,
                OpenDrain = 1
            }
        }
    }
}


