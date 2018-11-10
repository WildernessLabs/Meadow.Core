using System;
using System.Collections.Generic;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public class F7Micro : DeviceBase
    {
        //public static IDictionary<string, IPin> DigitalPins = new Dictionary<string, IPin>();
        //protected IDictionary<string, IPin> _digitalChannels = new Dictionary<string, IPin>();
        //protected IDictionary<string, IPin> _analogChannels = new Dictionary<string, IPin>();
        //protected IDictionary<string, IPin> _pwmChannels = new Dictionary<string, IPin>();

        static F7Micro()
        {
            //DigitalPins.Add("P01", new Pin("Pin 01", 0x01));

        }

        public F7Micro()
        {
            this.Capabilities = new DeviceCapabilities();

            //// digital channels
            //_digitalChannels.Add("P01", new Pin("Pin 01", 0x01));
            //_digitalChannels.Add("P02", new Pin("Pin 02", 0x02));
            //// analog channels
            //_analogChannels.Add("A01", new Pin("Pin A1", 0x128));
            //// pwm channels
            //_pwmChannels.Add("PWM01", new PwmPin("PWM 01", 0x01));

        }


        // TODO: does this work right or instantiate every time?
        //public PinAddresses Pins => new PinAddresses();

        //public class PinAddresses {
        //    public readonly byte D01 = 0x01;
        //    public readonly byte D02 = 0x02;
        //    public readonly byte D03 = 0x48;
        //    public readonly byte OnboardLEDRed = 0x16;
        //    public readonly byte OnboardLEDGreen = 0x16;
        //    public readonly byte OnboardLEDBlue = 0x16;
        //}

        public PinDefinitions Pins => new PinDefinitions();

        public class PinDefinitions
        {
            public readonly PwmPin D01 = PWMChannels.Pwm01;
            public readonly PwmPin D02 = PWMChannels.Pwm02;
            public readonly PwmPin OnboardLEDRed   = PWMChannels.OnboardLEDRed;
            public readonly PwmPin OnboardLEDGreen = PWMChannels.OnboardLEDGreen; 
            public readonly PwmPin OnboardLEDBlue  = PWMChannels.OnboardLEDBlue;
        }

        public PwmDefinitions Pwms => new PwmDefinitions();

        public class PwmDefinitions 
        {
            public readonly PwmPin Pwm01 = PWMChannels.Pwm01;
        }

        private static class PWMChannels
        {
            public static readonly PwmPin Pwm01 = new PwmPin("Pwm01", 0x128);
            public static readonly PwmPin Pwm02 = new PwmPin("Pwm01", 0x256);
            public static readonly PwmPin OnboardLEDRed     = new PwmPin("Pwm01", 0x256);
            public static readonly PwmPin OnboardLEDGreen = new PwmPin("Pwm01", 0x256);
            public static readonly PwmPin OnboardLEDBlue  = new PwmPin("Pwm01", 0x256);
        }

    }
}
