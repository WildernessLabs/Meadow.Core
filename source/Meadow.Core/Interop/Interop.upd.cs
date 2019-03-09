using System;
using System.Runtime.InteropServices;

namespace Meadow.Core.Interop
{
    internal static partial class Interop
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
        public const int STM32_GPIO_BSRR_OFFSET = 0x18;

        public struct UpdRegisterValue
        {
            public int Address;
            public uint Value;
        }

    }

}

