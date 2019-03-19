using System;
using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class STM32
        {
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

            public const int STM32_GPIO_MODER_OFFSET = 0x00;
            public const int STM32_GPIO_OTYPER_OFFSET = 0x04;
            public const int STM32_GPIO_OSPEED_OFFSET = 0x08;
            public const int STM32_GPIO_PUPDR_OFFSET = 0x0c;
            public const int STM32_GPIO_IDR_OFFSET = 0x10;
            public const int STM32_GPIO_BSRR_OFFSET = 0x18;

            public const int RCC_BASE = 0x40023800;
            public const int STM32_RCC_APB2RSTR_OFFSET = 0x0024;
            public const int STM32_RCC_APB2ENR_OFFSET = 0x0044;

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

