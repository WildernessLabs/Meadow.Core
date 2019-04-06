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

        // see reference manual, page 1028-ish
        private const int TIMx_CR1_OFFSET = 0x00;
        private const int TIMx_EGR_OFFSET = 0x14;
        private const int TIMx_CCMR1_OFFSET = 0x18; // capture/compare mode register 1
        private const int TIMx_CCER_OFFSET = 0x20;  // capture/compare enable
        private const int TIMx_ARR_OFFSET = 0x2C;   // auto-load register
        private const int TIMx_CCR1_OFFSET = 0x34;  // capture/compare register 1

        private const int UG_SHIFT = 0;

        private const int OC1PE_SHIFT = 3;
        private const int OC1M_SHIFT = 4;

        private const int CC1E_SHIFT = 0;
        private const int CC1P_SHIFT = 1;

        private const int PWM_MODE_1 = 6;

        private const int ARPE_SHIFT = 7;

        public void EnablePC6PWM()
        {
            // this is purely a hard-coded, straight-line test to generate a PWM on a known channel.  
            // It will get refactored once it's working

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

            // 9) set GPIO to alternate function 2
            ConfigureGpio(STM32.GpioPort.PortC, 6, STM32.GpioMode.AlternateFunction, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false, InterruptMode.None);
        }
    }
}
