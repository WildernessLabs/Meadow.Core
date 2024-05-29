﻿using Meadow.Core;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using static Meadow.Core.Interop;
using static Meadow.Core.Interop.Nuttx;

namespace Meadow.Devices;

/// <summary>
/// A wrapper around the Meadow Generic Platform Driver
/// </summary>
/// <remarks>
/// There is one and only one instance of this driver, and it lives effectively whenever the app is up, so it's
/// used as a static singleton
/// </remarks>
internal static class UPD
{
    private const string GPDDriverName = "/dev/upd";

    private static IntPtr DriverHandle { get; set; }

    static UPD()
    {
        DriverHandle = Nuttx.open(GPDDriverName, Interop.Nuttx.DriverFlags.DontCare);
        if (DriverHandle == IntPtr.Zero || DriverHandle.ToInt32() == -1)
        {
            throw new NativeException("Failed to open UPD driver");
        }
    }

    public static void ReOpen()
    {
        if (DriverHandle != IntPtr.Zero)
        {
            Nuttx.close(DriverHandle);
            DriverHandle = IntPtr.Zero;
        }
        DriverHandle = Nuttx.open(GPDDriverName, Interop.Nuttx.DriverFlags.DontCare);
        if (DriverHandle == IntPtr.Zero || DriverHandle.ToInt32() == -1)
        {
            throw new NativeException("Failed to re-open UPD driver");
        }
    }

    public static WakeSource GetLastWakeSource()
    {
        try
        {
            return (WakeSource)Nuttx.pwrmgmt_most_recent_wakeup_reason();
        }
        catch
        {
            return WakeSource.Unknown;
        }
    }

    public static void DumpClockRegisters()
    {
        var cr = GetRegister(STM32.RCC_BASE + STM32.RCC_CR_OFFSET);
        var cfg = GetRegister(STM32.RCC_BASE + STM32.RCC_CFGR_OFFSET);
        var pllcfg = GetRegister(STM32.RCC_BASE + STM32.RCC_PLLCFGR_OFFSET);
        var ahb1rst = GetRegister(STM32.RCC_BASE + STM32.RCC_AHB1RSTR_OFFSET);
        var ahb1en = GetRegister(STM32.RCC_BASE + STM32.RCC_AHB1ENR_OFFSET);
        var apb1rst = GetRegister(STM32.RCC_BASE + STM32.RCC_APB1RSTR_OFFSET);
        var apb1en = GetRegister(STM32.RCC_BASE + STM32.RCC_APB1ENR_OFFSET);
        var apb2rst = GetRegister(STM32.RCC_BASE + STM32.RCC_APB2RSTR_OFFSET);
        var apb2en = GetRegister(STM32.RCC_BASE + STM32.RCC_APB2ENR_OFFSET);
        var dckcfg1 = GetRegister(STM32.RCC_BASE + STM32.RCC_DCKCFGR1_OFFSET);
        var dckcfg2 = GetRegister(STM32.RCC_BASE + STM32.RCC_DCKCFGR2_OFFSET);

        Resolver.Log.Info("Clock Registers");
        Resolver.Log.Info($"\tRCC_CR:       {cr:X8}");
        Resolver.Log.Info($"\tRCC_CFGR:     {cfg:X8}");
        Resolver.Log.Info($"\tRCC_AHB1RSTR: {ahb1rst:X8}");
        Resolver.Log.Info($"\tRCC_AHB1ENR:  {ahb1en:X8}");
        Resolver.Log.Info($"\tRCC_APB1RSTR: {apb1rst:X8}");
        Resolver.Log.Info($"\tRCC_APB1ENR:  {apb1en:X8}");
        Resolver.Log.Info($"\tRCC_APB2RSTR: {apb2rst:X8}");
        Resolver.Log.Info($"\tRCC_APB2ENR:  {apb2en:X8}");
        Resolver.Log.Info($"\tRCC_PLLCFGR:  {pllcfg:X8}");
        Resolver.Log.Info($"\tRCC_DCKCFGR1: {dckcfg1:X8}");
        Resolver.Log.Info($"\tRCC_DCKCFGR2: {dckcfg2:X8}");
    }

    public static void DumpI2cRegisters()
    {
        var cr1 = UPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR1_OFFSET);
        var cr2 = UPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR2_OFFSET);
        var isr = UPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_ISR_OFFSET);
        var timing = UPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_TIMINGR_OFFSET);
        var timeout = UPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_TIMEOUTR_OFFSET);

        Resolver.Log.Info("I2C Registers");
        Resolver.Log.Info($"\tI2C_CR1:      {cr1:X8}");
        Resolver.Log.Info($"\tI2C_CR2:      {cr2:X8}");
        Resolver.Log.Info($"\tI2C_ISR:      {isr:X8}");
        Resolver.Log.Info($"\tI2C_TIMINGR:  {timing:X8}");
        Resolver.Log.Info($"\tI2C_TIMEOUTR: {timeout:X8}");
    }

    public static void SetRegister(uint address, uint value)
    {
        Nuttx.SetRegister(DriverHandle, address, value);
    }

    public static uint GetRegister(uint address)
    {
        // we ignore the result here as I've never seen it fail, and if it does, there's zero that the app can do about it
        Nuttx.TryGetRegister(DriverHandle, address, out uint value);
        return value;
    }

    public static Nuttx.ErrorCode GetLastError()
    {
        int errno = 0;
        Nuttx.ioctl(DriverHandle, Nuttx.UpdIoctlFn.GetLastError, ref errno);
        return (Nuttx.ErrorCode)errno;
    }

    public static void UpdateRegister(uint address, uint clearBits, uint setBits)
    {
        Nuttx.UpdateRegister(DriverHandle, address, clearBits, setBits);
    }

    private static int CheckIoctlResult<TRequest>(TRequest request, int result)
    {
        if (result != 0)
        {
            var err = GetLastError();
            Resolver.Log.Error($"ioctl {request} returned {result}. Last error: {err}");
            return (int)err;
        }

        return 0;
    }

    public static int Ioctl(Nuttx.GpioIoctlFn request, ref int pinDesignator)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref pinDesignator));
    }

    public static int Ioctl(Nuttx.GpioIoctlFn request, ref Nuttx.GPIOPinState pinState)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref pinState));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdRegisterValue registerValue)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref registerValue));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdRegisterUpdate registerUpdate)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref registerUpdate));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, IntPtr.Zero));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, Nuttx.UpdSleepCommand command)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref command));
    }


    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdGpioInterruptConfiguration interruptConfig)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref interruptConfig));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdI2cCommand i2cCommand)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref i2cCommand));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdSPIDataCommand spiCommand)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref spiCommand));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdSPISpeedCommand spiCommand)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref spiCommand));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdSPIModeCommand spiCommand)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref spiCommand));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdSPIBitsCommand spiCommand)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref spiCommand));
    }

    public static int Ioctl(Nuttx.GpioIoctlFn request, ref Nuttx.GPIOConfigFlags configFlags)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref configFlags));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdEnumDirCmd cmd)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref cmd));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdEsp32Command cmd)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref cmd));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdEsp32EventDataPayload eventData)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref eventData));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdConfigurationValue configData)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref configData));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdDeviceInfo deviceInfo)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref deviceInfo));
    }

    public static int Ioctl(Nuttx.UpdIoctlFn request, ref ulong param)
    {
        return CheckIoctlResult(
            request,
            ioctl(DriverHandle, request, ref param));
    }

    public static class PWM
    {
        private static readonly List<uint> m_timersInitialized = new List<uint>();

        public static bool PwmCmd(Nuttx.UpdIoctlFn request, Nuttx.UpdPwmCmd data)
        {
            var result = Nuttx.ioctl(DriverHandle, request, ref data);
            if (result != 0)
            {
                var err = GetLastError();
                Resolver.Log.Error($"PWM setup failed {err}");
                return false;
            }

            return true;
        }

        public static bool Setup(uint timer)
        {
            if (m_timersInitialized.Contains(timer))
            {
                return true;
            }

            var data = new Nuttx.UpdPwmCmd
            {
                Timer = timer,
            };

            var result = PwmCmd(Nuttx.UpdIoctlFn.PwmSetup, data);
            if (result)
            {
                m_timersInitialized.Add(timer);
            }

            return result;
        }

        public static bool Start(IPwmChannelInfo pwmChannelInfo, uint frequency, float dutyCycle)
        {
            // just make sure we've been initialized
            if (!Setup(pwmChannelInfo.Timer))
            {
                return false;
            }

            if (dutyCycle > 1) dutyCycle = 1;
            if (dutyCycle < 0) dutyCycle = 0;

            // nuttx (well the processor) takes a 16-bit duty cycle, so 65536 == 100% == 1.0
            var sixteenBitDuty = (uint)(dutyCycle * 65535f);

            var data = new Nuttx.UpdPwmCmd
            {
                Timer = pwmChannelInfo.Timer,
                Channel = pwmChannelInfo.TimerChannel,
                Frequency = frequency,
                Duty = sixteenBitDuty
            };

            var result = UPD.PWM.PwmCmd(Nuttx.UpdIoctlFn.PwmStart, data);

            return result;
        }

        public static bool Stop(IPwmChannelInfo pwmChannelInfo)
        {
            var data = new Nuttx.UpdPwmCmd
            {
                Timer = pwmChannelInfo.Timer,
                Channel = pwmChannelInfo.TimerChannel
            };

            return UPD.PWM.PwmCmd(Nuttx.UpdIoctlFn.PwmStop, data);
        }

        public static bool Shutdown(uint channel)
        {
            var data = new Nuttx.UpdPwmCmd
            {
                Timer = channel
            };

            var result = UPD.PWM.PwmCmd(Nuttx.UpdIoctlFn.PwmShutdown, data);

            if (result)
            {
                if (m_timersInitialized.Contains(channel))
                {
                    m_timersInitialized.Remove(channel);
                }
            }
            return result;
        }
    }
}

/* ===== MEADOW GPIO PIN MAP =====
    BOARD PIN   SCHEMATIC       CPU PIN   MDW NAME  ALT FN   IMPLEMENTED?
    J301-1      RESET
    J301-2      3.3
    J301-3      VREF
    J301-4      GND
    J301-5      DAC_OUT1        PA4         A0
    J301-6      DAC_OUT2        PA5         A1
    J301-7      ADC1_IN3        PA3         A2
    J301-8      ADC1_IN7        PA7         A3
    J301-9      ADC1_IN10       PC0         A4
    J301-10     ADC1_IN11       PC1         A5
    J301-11     SPI3_CLK        PC10        SCK
    J301-12     SPI3_MOSI       PB5         MOSI    AF6
    J301-13     SPI3_MISO       PC11        MISO    AF6
    J301-14     UART4_RX        PI9         D00     AF8
    J301-15     UART4_TX        PH13        D01     AF8
    J301-16     PC6             PC6         D02                 *
    J301-17     CAN1_RX         PB8         D03     AF9
    J301-18     CAN1_TX         PB9         D04     AF9

    J302-4      PE3             PE3         D15
    J302-5      PG3             PG3         D14
    J302-6      USART1_RX       PB15        D13     AF4
    J302-7      USART1_TX       PB14        D12     AF4
    J302-8      PC9             PC9         D11
    J302-9      PH10            PH10        D10
    J302-10     PB1             PB1         D09
    J302-11     I2C1_SCL        PB6         D08     AF4
    J302-12     I2C1_SDA        PB7         D07     AF4
    J302-13     PB0             PB0         D06
    J302-14     PC7             PC7         D05

    LED_B       PA0
    LED_G       PA1
    LED_R       PA2
*/
