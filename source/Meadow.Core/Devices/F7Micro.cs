using System;
using System.Collections.Generic;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public class F7Micro : DeviceBase
    {
        static F7Micro()
        {
        }

        public F7Micro()
        {
            this.Capabilities = new DeviceCapabilities();
        }

        public PinDefinitions Pins => new PinDefinitions();

        public class PinDefinitions
        {
            public readonly PwmPin D01 = PWMChannels.Pwm01;
            public readonly PwmPin D02 = PWMChannels.Pwm02;
            public readonly PwmPin D03 = PWMChannels.Pwm03;
            public readonly PwmPin D04 = PWMChannels.Pwm04;
            public readonly PwmPin D05 = PWMChannels.Pwm05;
            public readonly PwmPin D06 = PWMChannels.Pwm06;
            public readonly PwmPin D07 = PWMChannels.Pwm07;
            public readonly PwmPin D08 = PWMChannels.Pwm08;
            public readonly PwmPin D09 = PWMChannels.Pwm09;
            public readonly PwmPin D10 = PWMChannels.Pwm10;
            public readonly PwmPin D11 = PWMChannels.Pwm11;
            public readonly PwmPin D12 = PWMChannels.Pwm12;
            public readonly PwmPin D13 = PWMChannels.Pwm13;
            public readonly PwmPin D14 = PWMChannels.Pwm14;
            public readonly PwmPin D15 = PWMChannels.Pwm15;
            public readonly PwmPin OnboardLEDRed   = PWMChannels.OnboardLEDRed;
            public readonly PwmPin OnboardLEDGreen = PWMChannels.OnboardLEDGreen; 
            public readonly PwmPin OnboardLEDBlue  = PWMChannels.OnboardLEDBlue;
            public readonly AnalogPin A01 = AnalogChannels.A01;
            public readonly AnalogPin A02 = AnalogChannels.A02;
            public readonly AnalogPin A03 = AnalogChannels.A03;
            public readonly AnalogPin A04 = AnalogChannels.A04;
            public readonly AnalogPin A05 = AnalogChannels.A05;
        }


        private static class DigitalChannels
        {
            // example for boards that have digital pins that don't have PWM timers
            // enabled on them.
            //public static readonly DigitalPin D99 = new DigitalPin("D99", 0x128);
        }

        private static class PWMChannels
        {
            public static readonly PwmPin Pwm01 = new PwmPin("Pwm01", 0x128);
            public static readonly PwmPin Pwm02 = new PwmPin("Pwm02", 0x256);
            public static readonly PwmPin Pwm03 = new PwmPin("Pwm03", 0x256);
            public static readonly PwmPin Pwm04 = new PwmPin("Pwm04", 0x256);
            public static readonly PwmPin Pwm05 = new PwmPin("Pwm05", 0x256);
            public static readonly PwmPin Pwm06 = new PwmPin("Pwm06", 0x256);
            public static readonly PwmPin Pwm07 = new PwmPin("Pwm07", 0x256);
            public static readonly PwmPin Pwm08 = new PwmPin("Pwm08", 0x256);
            public static readonly PwmPin Pwm09 = new PwmPin("Pwm09", 0x256);
            public static readonly PwmPin Pwm10 = new PwmPin("Pwm10", 0x256);
            public static readonly PwmPin Pwm11 = new PwmPin("Pwm11", 0x256);
            public static readonly PwmPin Pwm12 = new PwmPin("Pwm12", 0x256);
            public static readonly PwmPin Pwm13 = new PwmPin("Pwm13", 0x256);
            public static readonly PwmPin Pwm14 = new PwmPin("Pwm14", 0x256);
            public static readonly PwmPin Pwm15 = new PwmPin("Pwm15", 0x256);
            public static readonly PwmPin OnboardLEDRed     = new PwmPin("Pwm01", 0x256);
            public static readonly PwmPin OnboardLEDGreen = new PwmPin("Pwm01", 0x256);
            public static readonly PwmPin OnboardLEDBlue  = new PwmPin("Pwm01", 0x256);
        }

        private static class AnalogChannels
        {
            public static readonly AnalogPin A01 = new AnalogPin("A01", 0x0);
            public static readonly AnalogPin A02 = new AnalogPin("A02", 0x0);
            public static readonly AnalogPin A03 = new AnalogPin("A03", 0x0);
            public static readonly AnalogPin A04 = new AnalogPin("A04", 0x0);
            public static readonly AnalogPin A05 = new AnalogPin("A05", 0x0);
        }

    }
}
